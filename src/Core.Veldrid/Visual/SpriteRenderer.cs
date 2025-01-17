﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UAlbion.Api;
using UAlbion.Core.Textures;
using UAlbion.Core.Visual;
using Veldrid;
using Veldrid.Utilities;

namespace UAlbion.Core.Veldrid.Visual
{
    public sealed class SpriteRenderer : VeldridComponent, IRenderer
    {
        // Vertex Layout
        static readonly VertexLayoutDescription VertexLayout = VertexLayoutHelper.Vertex2DTextured;

        // Instance Layout
        static readonly VertexLayoutDescription InstanceLayout = new VertexLayoutDescription(
                VertexLayoutHelper.Vector3D("iTransform1"),
                VertexLayoutHelper.Vector3D("iTransform2"),
                VertexLayoutHelper.Vector3D("iTransform3"),
                VertexLayoutHelper.Vector3D("iTransform4"),
                VertexLayoutHelper.Vector2D("iTexOffset"),
                VertexLayoutHelper.Vector2D("iTexSize"),
                VertexLayoutHelper.UIntElement("iTexLayer"),
                VertexLayoutHelper.UIntElement("iFlags")
            )
        { InstanceStepRate = 1 };

        // Resource Sets
        static readonly ResourceLayoutDescription PerSpriteLayoutDescription = new ResourceLayoutDescription(
            ResourceLayoutHelper.Texture("uSprite"),
            ResourceLayoutHelper.Sampler("uSpriteSampler"),
            ResourceLayoutHelper.Uniform("_Uniform")
        );

        static readonly ushort[] Indices = { 0, 1, 2, 2, 1, 3 };
        static readonly Vertex2DTextured[] Vertices =
        {
            new Vertex2DTextured(-0.5f, 0.0f, 0.0f, 0.0f), new Vertex2DTextured(0.5f, 0.0f, 1.0f, 0.0f),
            new Vertex2DTextured(-0.5f, 1.0f, 0.0f, 1.0f), new Vertex2DTextured(0.5f, 1.0f, 1.0f, 1.0f),
        };


        readonly DisposeCollector _disposeCollector = new DisposeCollector();
        readonly List<Shader> _shaders = new List<Shader>();
        readonly Dictionary<SpriteShaderKey, Pipeline> _pipelines = new Dictionary<SpriteShaderKey, Pipeline>();

#pragma warning disable CA2213 // Analysis doesn't know about DisposeCollector
        // Context objects
        DeviceBuffer _vertexBuffer;
        DeviceBuffer _indexBuffer;
        ResourceLayout _perSpriteResourceLayout;
#pragma warning restore CA2213 // Analysis doesn't know about DisposeCollector

        public Type[] RenderableTypes => new [] { typeof(MultiSprite) };
        public RenderPasses RenderPasses => RenderPasses.Standard;

        Pipeline BuildPipeline(VeldridRendererContext c, SpriteShaderKey key)
        {
            var shaderCache = Resolve<IVeldridShaderCache>();
            var shaderName = key.UseCylindricalShader ? "CylindricalSprite" : "Sprite"; 
            var vertexShaderName = shaderName + "SV.vert";
            var fragmentShaderName = shaderName + "SF.frag";
            var vertexShaderContent = shaderCache.GetGlsl(vertexShaderName);
            var fragmentShaderContent = shaderCache.GetGlsl(fragmentShaderName);

            if (key.UseArrayTexture)
            {
                fragmentShaderName += ".array";
                fragmentShaderContent =
                    @"#define USE_ARRAY_TEXTURE
" + fragmentShaderContent;
            }

            if (key.UsePalette)
            {
                fragmentShaderName += ".pal";
                fragmentShaderContent =
                    @"#define USE_PALETTE
" + fragmentShaderContent;
            }

            var shaders = shaderCache.GetShaderPair(
                c.GraphicsDevice.ResourceFactory,
                vertexShaderName, fragmentShaderName,
                vertexShaderContent, fragmentShaderContent);

            _shaders.AddRange(shaders);

            var depthStencilMode =
                key.PerformDepthTest
                ? DepthStencilStateDescription.DepthOnlyLessEqual
                : DepthStencilStateDescription.Disabled;

            var rasterizerMode = new RasterizerStateDescription(
                FaceCullMode.None,
                PolygonFillMode.Solid,
                FrontFace.Clockwise,
                key.PerformDepthTest, // depth test
                true); // scissor test

            var pipelineDescription = new GraphicsPipelineDescription(
                BlendStateDescription.SingleAlphaBlend,
                depthStencilMode,
                rasterizerMode,
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(new[] { VertexLayout, InstanceLayout },
                    shaders,
                    ShaderHelper.GetSpecializations(c.GraphicsDevice)),
                new[] { _perSpriteResourceLayout, c.SceneContext.CommonResourceLayout },
                ((FramebufferSource)c.Framebuffer)?.Framebuffer.OutputDescription
                ?? c.GraphicsDevice.SwapchainFramebuffer.OutputDescription);

            var pipeline = c.GraphicsDevice.ResourceFactory.CreateGraphicsPipeline(ref pipelineDescription);
            pipeline.Name = $"P_Sprite_{key}";
            return pipeline;
        }

        public override void CreateDeviceObjects(VeldridRendererContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _vertexBuffer = context.GraphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription(Vertices.SizeInBytes(), BufferUsage.VertexBuffer));
            _indexBuffer = context.GraphicsDevice.ResourceFactory.CreateBuffer(new BufferDescription(Indices.SizeInBytes(), BufferUsage.IndexBuffer));
            _vertexBuffer.Name = "SpriteVertexBuffer";
            _indexBuffer.Name = "SpriteIndexBuffer";
            context.CommandList.UpdateBuffer(_vertexBuffer, 0, Vertices);
            context.CommandList.UpdateBuffer(_indexBuffer, 0, Indices);

            _perSpriteResourceLayout = context.GraphicsDevice.ResourceFactory.CreateResourceLayout(PerSpriteLayoutDescription);
            _disposeCollector.Add(_vertexBuffer, _indexBuffer, _perSpriteResourceLayout);
        }

        public void UpdatePerFrameResources(IRendererContext context, IEnumerable<IRenderable> renderables, IList<IRenderable> results)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (renderables == null) throw new ArgumentNullException(nameof(renderables));
            if (results == null) throw new ArgumentNullException(nameof(results));

            var c = (VeldridRendererContext)context;
            var cl = c.CommandList;
            var gd = c.GraphicsDevice;
            var textureManager = Resolve<ITextureManager>();
            var objectManager = Resolve<IDeviceObjectManager>();
            var engineFlags = Resolve<IEngineSettings>().Flags;

            foreach (var renderable in renderables)
            {
                var sprite = (MultiSprite)renderable;
                if (sprite.ActiveInstances == 0)
                    continue;

                bool didSomething = false;
                var shaderKey = new SpriteShaderKey(sprite, engineFlags);
                if (!_pipelines.ContainsKey(shaderKey))
                    _pipelines.Add(shaderKey, BuildPipeline(c, shaderKey));

                uint bufferSize = (uint)sprite.Instances.Length * SpriteInstanceData.StructSize;
                var buffer = objectManager.GetDeviceObject<DeviceBuffer>((sprite, sprite, "InstanceBuffer"));
                if (buffer?.SizeInBytes != bufferSize)
                {
                    buffer = gd.ResourceFactory.CreateBuffer(new BufferDescription(bufferSize, BufferUsage.VertexBuffer));
                    buffer.Name = $"B_Inst:{sprite.Name}";
                    PerfTracker.IncrementFrameCounter("Create InstanceBuffer");
                    objectManager.SetDeviceObject((sprite, sprite, "InstanceBuffer"), buffer);
                    sprite.InstancesDirty = true;
                }

                if (sprite.InstancesDirty)
                {
                    cl.PushDebugGroup(sprite.Name);
                    didSomething = true;
                    var instances = sprite.Instances;
                    VeldridUtil.UpdateBufferSpan(cl, buffer, instances);
                    PerfTracker.IncrementFrameCounter("Update InstanceBuffers");
                }

                textureManager?.PrepareTexture(sprite.Key.Texture, context);
                TextureView textureView = (TextureView)textureManager?.GetTexture(sprite.Key.Texture);

                var uniformBuffer = objectManager.GetDeviceObject<DeviceBuffer>((sprite, textureView, "UniformBuffer"));
                if (uniformBuffer == null)
                {
                    if (!didSomething) { cl.PushDebugGroup(sprite.Name); didSomething = true; } 
                    uniformBuffer = gd.ResourceFactory.CreateBuffer(new BufferDescription((uint)Unsafe.SizeOf<SpriteUniformInfo>(), BufferUsage.UniformBuffer));
                    uniformBuffer.Name = $"B_SpriteUniform:{sprite.Name}";
                    PerfTracker.IncrementFrameCounter("Create sprite uniform buffer");
                    objectManager.SetDeviceObject((sprite, textureView, "UniformBuffer"), uniformBuffer);

                    var uniformInfo = new SpriteUniformInfo
                    {
                        Flags = sprite.Key.Flags,
                        TextureWidth = textureView?.Target.Width ?? 1,
                        TextureHeight = textureView?.Target.Height ?? 1
                    };
                    cl.UpdateBuffer(uniformBuffer, 0, uniformInfo);
                }

                var resourceSet = objectManager.GetDeviceObject<ResourceSet>((sprite, textureView, "ResourceSet"));
                if (resourceSet == null)
                {
                    if (!didSomething) { cl.PushDebugGroup(sprite.Name); didSomething = true; } 
                    resourceSet = gd.ResourceFactory.CreateResourceSet(new ResourceSetDescription(
                        _perSpriteResourceLayout,
                        textureView,
                        gd.PointSampler,
                        uniformBuffer));
                    resourceSet.Name = $"RS_Sprite:{sprite.Key.Texture.Name}";
                    PerfTracker.IncrementFrameCounter("Create ResourceSet");
                    objectManager.SetDeviceObject((sprite, textureView, "ResourceSet"), resourceSet);
                }

                sprite.InstancesDirty = false;
                results.Add(sprite);
                if (didSomething)
                    cl.PopDebugGroup();
            }

            Resolve<ISpriteManager>().Cleanup();
        }

        public void Render(IRendererContext context, RenderPasses renderPass, IRenderable renderable)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (renderable == null) throw new ArgumentNullException(nameof(renderable));

            var c = (VeldridRendererContext)context;
            var cl = c.CommandList;
            var dom = Resolve<IDeviceObjectManager>();
            var engineFlags = Resolve<IEngineSettings>().Flags;
            var textureManager = Resolve<ITextureManager>();

            var sprite = (MultiSprite)renderable;
            var shaderKey = new SpriteShaderKey(sprite, engineFlags);
            sprite.PipelineId = shaderKey.GetHashCode();

            //if (!shaderKey.UseArrayTexture)
            //    return;

            if (c.SceneContext.PaletteView == null)
                return;

            TextureView textureView = (TextureView)textureManager?.GetTexture(sprite.Key.Texture);
            var resourceSet = dom.GetDeviceObject<ResourceSet>((sprite, textureView, "ResourceSet"));
            var instanceBuffer = dom.GetDeviceObject<DeviceBuffer>((sprite, sprite, "InstanceBuffer"));

            if (resourceSet == null)
            {
                Warn($"Couldn't locate resource set for {sprite}");
                return;
            }

            cl.PushDebugGroup(sprite.Name);
            if (sprite.Key.ScissorRegion.HasValue)
            {
                IWindowManager wm = Resolve<IWindowManager>();
                var screenCoordinates = wm.UiToPixel(sprite.Key.ScissorRegion.Value);
                cl.SetScissorRect(0, (uint)screenCoordinates.X, (uint)screenCoordinates.Y, (uint)screenCoordinates.Width, (uint)screenCoordinates.Height);
            }

            cl.SetPipeline(_pipelines[shaderKey]);
            cl.SetGraphicsResourceSet(0, resourceSet);
            cl.SetGraphicsResourceSet(1, c.SceneContext.CommonResourceSet);
            cl.SetVertexBuffer(0, _vertexBuffer);
            cl.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            cl.SetVertexBuffer(1, instanceBuffer);

            cl.DrawIndexed((uint)Indices.Length, (uint)sprite.ActiveInstances, 0, 0, 0);
            if (sprite.Key.ScissorRegion.HasValue)
                cl.SetFullScissorRect(0);
            cl.PopDebugGroup();
        }

        public override void DestroyDeviceObjects()
        {
            _disposeCollector.DisposeAll();

            foreach (var pipeline in _pipelines)
                pipeline.Value.Dispose();
            _pipelines.Clear();

            foreach (var shader in _shaders)
                shader.Dispose();
            _shaders.Clear();
        }

        public void Dispose() => DestroyDeviceObjects();
    }
}

