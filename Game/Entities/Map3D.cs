﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UAlbion.Core;
using UAlbion.Core.Events;
using UAlbion.Formats.AssetIds;
using UAlbion.Formats.Parsers;
using UAlbion.Game.Events;
using Veldrid;

namespace UAlbion.Game.Entities
{
    public class Map3D : Component, IMap
    {
        readonly Skybox _skybox;
        readonly MapRenderable3D _renderable;
        static readonly IList<Handler> Handlers = new Handler[]
        {
            new Handler<Map3D, SubscribedEvent>((x, e) => x.Subscribed()),
            new Handler<Map3D, WorldCoordinateSelectEvent>((x, e) => x.Select(e)),
            // new Handler<Map3D, UnloadMapEvent>((x, e) => x.Unload()),
        };

        readonly Assets _assets;
        readonly LabyrinthData _labyrinthData;
        readonly MapData3D _mapData;
        readonly RgbaFloat _backgroundColour;

        void Select(WorldCoordinateSelectEvent worldCoordinateSelectEvent)
        {
        }

        public Map3D(Assets assets, MapDataId mapId) : base(Handlers)
        {
            _assets = assets;
            MapId = mapId;
            _mapData = assets.LoadMap3D(mapId);

            _labyrinthData = assets.LoadLabyrinthData(_mapData.LabDataId);
            if (_labyrinthData != null)
            {
                _renderable = new MapRenderable3D(assets, _mapData, _labyrinthData);
                if(_labyrinthData.BackgroundId.HasValue)
                    _skybox = new Skybox(assets, _labyrinthData.BackgroundId.Value, _mapData.PaletteId);
                TileSize = new Vector3(64.0f, _labyrinthData.WallHeight, 64.0f);

                var palette = assets.LoadPalette(_mapData.PaletteId);
                uint backgroundColour = palette.GetPaletteAtTime(0)[_labyrinthData.BackgroundColour];
                var r = backgroundColour & 0xff;
                var g = backgroundColour & 0xff00 >> 8;
                var b = backgroundColour & 0xff0000 >> 16;
                _backgroundColour = new RgbaFloat(r / 255.0f, g / 255.0f, b / 255.0f, 1.0f);
            }
            else
                TileSize = new Vector3(64.0f, 64.0f, 64.0f);
        }

        public override string ToString() => $"Map3D:{MapId} {LogicalSize.X}x{LogicalSize.Y} TileSize: {TileSize}";
        public MapDataId MapId { get; }
        public Vector2 LogicalSize { get; }
        public Vector3 TileSize { get; }

        void Subscribed()
        {
            if (_skybox != null) Exchange.Attach(_skybox);
            if (_renderable != null) Exchange.Attach(_renderable);

            Raise(new SetClearColourEvent(_backgroundColour.R, _backgroundColour.G, _backgroundColour.B));
            Raise(new SetTileSizeEvent(TileSize, _labyrinthData.CameraHeight != 0 ? _labyrinthData.CameraHeight : 32));

            foreach (var npc in _mapData.Npcs)
            {
                var objectData = _labyrinthData.ObjectGroups[npc.ObjectNumber - 1];
                foreach (var subObject in objectData.SubObjects)
                {
                    var sprite = BuildSprite(npc.Waypoints[0].X, npc.Waypoints[0].Y, subObject);
                    if (sprite == null)
                        continue;

                    Exchange.Attach(sprite);
                }
            }

            for (int y = 0; y < _mapData.Height; y++)
            {
                for (int x = 0; x < _mapData.Width; x++)
                {
                    var contents = _mapData.Contents[y * _mapData.Width + x];
                    if (contents == 0 || contents >= _labyrinthData.ObjectGroups.Count)
                        continue;

                    var objectInfo = _labyrinthData.ObjectGroups[contents - 1];
                    foreach (var subObject in objectInfo.SubObjects)
                    {
                        var sprite = BuildSprite(x, y, subObject);
                        if (sprite == null)
                            continue;

                        Exchange.Attach(sprite);
                    }
                }
            }
        }

        MapObjectSprite BuildSprite(int tileX, int tileY, LabyrinthData.SubObject subObject)
        {
            var definition = _labyrinthData.Objects[subObject.ObjectInfoNumber];
            if (definition.TextureNumber == null)
                return null;

            bool onFloor = (definition.Properties & LabyrinthData.Object.ObjectFlags.FloorObject) != 0;

            var tilePosition = new Vector3(tileX - 0.5f, 0, tileY - 0.5f) * TileSize;
            var offset = new Vector3(subObject.X, subObject.Y, subObject.Z) / 8.0f;
            var smidgeon = onFloor ? new Vector3(0, 0.001f, 0) : Vector3.Zero;
            var position = tilePosition + offset + smidgeon;

            return new MapObjectSprite(
                definition.TextureNumber.Value,
                position,
                new Vector2(definition.MapWidth, definition.MapHeight),
                (definition.Properties & LabyrinthData.Object.ObjectFlags.FloorObject) != 0
            );
        }
    }
}
