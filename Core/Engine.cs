﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using ImGuiNET;
using UAlbion.Core.Events;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using UAlbion.Core.Textures;
using UAlbion.Core.Visual;

namespace UAlbion.Core
{
    public class Engine : Component, IWindowState, IDisposable
    {
        static readonly IList<Handler> Handlers = new Handler[]
        {
            new Handler<Engine, LoadRenderDocEvent>((x, _) =>
            {
                if (_renderDoc == null && RenderDoc.Load(out _renderDoc))
                    x.ChangeBackend(x.GraphicsDevice.BackendType, true);
            }),
            new Handler<Engine, QuitEvent>((x, e) => x._done = true),
            new Handler<Engine, SubscribedEvent>((x, _) => x.Exchange.Register<IWindowState>(x)),
            new Handler<Engine, SetCursorPositionEvent>((x, e) => x._pendingCursorUpdate = new Vector2(e.X, e.Y)),
            new Handler<Engine, ToggleFullscreenEvent>((x, _) => x.ToggleFullscreenState()),
            new Handler<Engine, ToggleResizableEvent>((x, _) => x.Window.Resizable = !x.Window.Resizable),
            new Handler<Engine, ToggleVisibleBorderEvent>((x, _) => x.Window.BorderVisible = !x.Window.BorderVisible),
        };

        static RenderDoc _renderDoc;
        public EventExchange GlobalExchange { get; }

        readonly IDictionary<Type, IRenderer> _renderers = new Dictionary<Type, IRenderer>();
        readonly IList<Scene> _scenes = new List<Scene>();
        readonly FrameTimeAverager _frameTimeAverager = new FrameTimeAverager(0.5);
        readonly FullScreenQuad _fullScreenQuad;
        readonly DebugGuiRenderer _igRenderable;
        readonly SceneContext _sceneContext = new SceneContext();

        //public Scene Scene { get; private set; }
        CommandList _frameCommands;
        TextureSampleCount? _newSampleCount;
        bool _windowResized;
        bool _done;
        bool _recreateWindow = true;
        Vector2? _pendingCursorUpdate;

        internal GraphicsDevice GraphicsDevice { get; private set; }
        internal Sdl2Window Window { get; private set; }
        internal RenderDoc RenderDoc => _renderDoc;

        internal string FrameTimeText => _frameTimeAverager.CurrentAverageFramesPerSecond.ToString("000.0 fps / ") +
                                         _frameTimeAverager.CurrentAverageFrameTimeMilliseconds.ToString("#00.00 ms");

        public Engine(GraphicsBackend backend, bool useRenderDoc) : base(Handlers)
        {
            GlobalExchange = new EventExchange("Global");

            if (useRenderDoc)
                RenderDoc.Load(out _renderDoc);

            ChangeBackend(backend);
            CheckForErrors();
            Sdl2Native.SDL_Init(SDLInitFlags.GameController);

            _igRenderable = new DebugGuiRenderer(Window.Width, Window.Height);
            _fullScreenQuad = new FullScreenQuad();
            var duplicator = new ScreenDuplicator();

            AddRenderer(_igRenderable);
            AddRenderer(duplicator);
            AddRenderer(_fullScreenQuad);
            GlobalExchange
                .Attach(this)
                .Attach(new DebugMenus(this));
        }

        public void AddRenderer(IRenderer renderer)
        {
            _renderers.Add(renderer.GetType(), renderer);
            if (renderer is IComponent component)
                GlobalExchange.Attach(component);
        }

        public void AddScene(Scene scene) => _scenes.Add(scene);

        public void Run()
        {
            CreateAllObjects();
            ImGui.StyleColorsClassic();
            Raise(new WindowResizedEvent(Window.Width, Window.Height));
            Raise(new BeginFrameEvent());

            var frameCounter = new FrameCounter();
            while (!_done)
            {
                double deltaSeconds = frameCounter.StartFrame();
                Raise(new BeginFrameEvent());

                Sdl2Events.ProcessEvents();
                InputSnapshot snapshot = Window.PumpEvents();
                if (_pendingCursorUpdate.HasValue)
                {
                    Sdl2Native.SDL_WarpMouseInWindow(
                        Window.SdlWindowHandle,
                        (int)_pendingCursorUpdate.Value.X,
                        (int)_pendingCursorUpdate.Value.Y);

                    _pendingCursorUpdate = null;
                }

                Raise(new InputEvent(deltaSeconds, snapshot, Window.MouseDelta));

                Update((float)deltaSeconds);
                if (!Window.Exists)
                    break;

                Draw();
            }

            Window.Close();
            DestroyAllObjects();
            GraphicsDevice.Dispose();
        }

        void Update(float deltaSeconds)
        {
            _frameTimeAverager.AddTime(deltaSeconds);
            Raise(new EngineUpdateEvent(deltaSeconds));
        }

        internal void ChangeMsaa(int msaaOption)
        {
            _newSampleCount = (TextureSampleCount)msaaOption;
        }

        internal void RefreshDeviceObjects(int numTimes)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < numTimes; i++)
            {
                DestroyAllObjects();
                CreateAllObjects();
            }

            sw.Stop();
            Console.WriteLine($"Refreshing resources {numTimes} times took {sw.Elapsed.TotalSeconds} seconds.");
        }

        void ToggleFullscreenState()
        {
            bool isFullscreen = Window.WindowState == WindowState.BorderlessFullScreen;
            Window.WindowState = isFullscreen ? WindowState.Normal : WindowState.BorderlessFullScreen;
        }

        void Draw()
        {
            Debug.Assert(Window.Exists);
            int width = Window.Width;
            int height = Window.Height;

            CoreTrace.Log.Info("Engine", "Start draw");
            if (_windowResized)
            {
                _windowResized = false;

                GraphicsDevice.ResizeMainWindow((uint)width, (uint)height);
                Raise(new WindowResizedEvent(width, height));
                CommandList cl = GraphicsDevice.ResourceFactory.CreateCommandList();
                cl.Begin();
                _sceneContext.RecreateWindowSizedResources(GraphicsDevice, cl);
                cl.End();
                GraphicsDevice.SubmitCommands(cl);
                cl.Dispose();
                CoreTrace.Log.Info("Engine", "Resize finished");
            }

            if (_newSampleCount != null)
            {
                _sceneContext.MainSceneSampleCount = _newSampleCount.Value;
                _newSampleCount = null;
                DestroyAllObjects();
                CreateAllObjects();
            }

            _frameCommands.Begin();
            foreach (var scene in _scenes)
                scene.RenderAllStages(GraphicsDevice, _frameCommands, _sceneContext, _renderers);
            CoreTrace.Log.Info("Engine", "Swapping buffers...");
            GraphicsDevice.SwapBuffers();
            CoreTrace.Log.Info("Engine", "Draw complete");
        }

        internal void ChangeBackend(GraphicsBackend backend) => ChangeBackend(backend, false);

        void ChangeBackend(GraphicsBackend backend, bool forceRecreateWindow)
        {
            if (GraphicsDevice != null)
            {
                DestroyAllObjects();
                GraphicsDevice.Dispose();
            }

            if (Window == null || _recreateWindow || forceRecreateWindow)
            {
                Window?.Close();

                WindowCreateInfo windowInfo = new WindowCreateInfo
                {
                    X = Window?.X ?? 684,
                    Y = Window?.Y ?? 456,
                    WindowWidth = Window?.Width ?? 684,
                    WindowHeight = Window?.Height ?? 456,
                    WindowInitialState = Window?.WindowState ?? WindowState.Normal,
                    WindowTitle = "UAlbion"
                };

                Window = VeldridStartup.CreateWindow(ref windowInfo);
                Window.BorderVisible = false;
                Window.CursorVisible = true;
                Window.Resized += () => _windowResized = true;
            }

            GraphicsDeviceOptions gdOptions = new GraphicsDeviceOptions(false, null, false,
                ResourceBindingModel.Improved, true, true, false /*, true*/)
            {
                Debug = true,
                SyncToVerticalBlank = true
            };

            GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window, gdOptions, backend);
            CheckForErrors();
            Window.Title = GraphicsDevice.BackendType.ToString();

            Raise(new BackendChangedEvent(GraphicsDevice));
            CreateAllObjects();
        }

        void CreateAllObjects()
        {
            _frameCommands = GraphicsDevice.ResourceFactory.CreateCommandList();
            _frameCommands.Name = "Frame Commands List";

            CommandList initCL = GraphicsDevice.ResourceFactory.CreateCommandList();
            initCL.Name = "Recreation Initialization Command List";
            initCL.Begin();
            _sceneContext.CreateDeviceObjects(GraphicsDevice, initCL, _sceneContext);

            foreach (var r in _renderers.Values)
                r.CreateDeviceObjects(GraphicsDevice, initCL, _sceneContext);

            foreach (var scene in _scenes)
                scene.CreateAllDeviceObjects(GraphicsDevice, initCL, _sceneContext);
            initCL.End();
            GraphicsDevice.SubmitCommands(initCL);
            initCL.Dispose();
        }

        void DestroyAllObjects()
        {
            GraphicsDevice.WaitForIdle();
            _frameCommands.Dispose();
            _sceneContext.DestroyDeviceObjects();
            foreach (var r in _renderers.Values)
                r.DestroyDeviceObjects();

            foreach (var scene in _scenes)
                scene.DestroyAllDeviceObjects();
            StaticResourceCache.DestroyAllDeviceObjects();
            Exchange.Resolve<ITextureManager>()?.DestroyDeviceObjects();
            GraphicsDevice.WaitForIdle();
        }

        public void Dispose()
        {
            _igRenderable?.Dispose();
            _frameCommands?.Dispose();
            _fullScreenQuad?.Dispose();
            //_graphicsDevice?.Dispose();
        }

        public void CheckForErrors()
        {
            /*GraphicsDevice?.CheckForErrors();*/
        }

        int IWindowState.Width => Window.Width;
        int IWindowState.Height => Window.Height;
        Vector2 IWindowState.Size => new Vector2(Window.Width, Window.Height);

        int IWindowState.GuiScale
        {
            get
            {
                const int NominalWidth = 360;
                const int NominalHeight = 240;
                float widthRatio = (float)Window.Width / NominalWidth;
                float heightRatio = (float)Window.Height / NominalHeight;
                return (int)(Math.Min(widthRatio, heightRatio) + 0.5f);
            }
        }
    }
}

