﻿using System.Numerics;
using UAlbion.Api;
using UAlbion.Core;
using UAlbion.Core.Events;
using UAlbion.Core.Visual;
using UAlbion.Formats.AssetIds;
using UAlbion.Game.Events;
using UAlbion.Game.State;

namespace UAlbion.Game.Entities
{
    public class Skybox : Component
    {
        readonly TileMap _tilemap;

        static readonly Handler[] Handlers =
        {
            new Handler<Skybox, RenderEvent>((x, e) => x.Render(e)),
            new Handler<Skybox, PostUpdateEvent>((x, _) => x.PostUpdate()),
        };

        void PostUpdate()
        {
            _tilemap.Position = Exchange.Resolve<IStateManager>().CameraPosition;
        }

        void Render(RenderEvent renderEvent)
        {
            renderEvent.Add(_tilemap);
        }

        public Skybox(IAssetManager assets, DungeonBackgroundId id, PaletteId paletteId) : base(Handlers)
        {
            var palette = assets.LoadPalette(paletteId);
            float size = 512.0f * 256.0f;
            _tilemap = new TileMap(
                (int)DrawLayer.Background, 
                new Vector3(-size, size / 3, size), // Just make sure it's bigger than the largest map
                1, 1, 
                palette.GetCompletePalette());
            var texture = assets.LoadTexture(id);
            _tilemap.DefineWall(1, texture, 0, 0, 0, false);
            _tilemap.DefineWall(1, texture, texture.Width, 0, 0, false);
            _tilemap.DefineWall(1, texture, texture.Width * 2, 0, 0, false);
            _tilemap.Set(0, 0, 0, 0, 0, 1, 0);
        }
    }
}