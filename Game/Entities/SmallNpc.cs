﻿using System;
using System.Collections.Generic;
using System.Numerics;
using UAlbion.Api;
using UAlbion.Core;
using UAlbion.Core.Events;
using UAlbion.Core.Visual;
using UAlbion.Formats.AssetIds;
using UAlbion.Formats.Assets;
using UAlbion.Game.Events;
using UAlbion.Game.Gui;
using UAlbion.Game.Scenes;

namespace UAlbion.Game.Entities
{
    public class SmallNpc : Component
    {
        static readonly HandlerSet Handlers = new HandlerSet(
            H<SmallNpc, SlowClockEvent>((x, e) => { x._sprite.Frame = e.FrameCount; }),
            H<SmallNpc, RightClickEvent>((x, e) => x.OnRightClick(e))
        );

        readonly MapNpc _npc;
        readonly MapSprite<SmallNpcId> _sprite;
        public override string ToString() => $"SNpc {_sprite.Id}";

        public SmallNpc(MapNpc npc) : base(Handlers)
        {
            _npc = npc ?? throw new ArgumentNullException(nameof(npc));
            _sprite = AttachChild(new MapSprite<SmallNpcId>((SmallNpcId)npc.ObjectNumber, DrawLayer.Characters1, 0, SpriteFlags.BottomAligned));
            _sprite.Selected += (sender, e) => e.SelectEvent.RegisterHit(e.HitPosition, this);
        }

        public override void Subscribed()
        {
            _sprite.TilePosition = new Vector3(_npc.Waypoints[0].X, _npc.Waypoints[0].Y, DrawLayer.Characters1.ToZCoordinate(_npc.Waypoints[0].Y));
            base.Subscribed();
        }

        void OnRightClick(RightClickEvent rightClickEvent)
        {
            if (_npc.EventChain == null)
                return;

            var window = Resolve<IWindowManager>();
            var camera = Resolve<ICamera>();
            var assets = Resolve<IAssetManager>();
            var settings = Resolve<ISettings>();

            ITextSource S(SystemTextId textId) => new DynamicText(() =>
                {
                    var template = assets.LoadString(textId, settings.Gameplay.Language);
                    return new TextFormatter(assets, settings.Gameplay.Language).Centre().Format(template).Blocks;
                });

            var normPosition = camera.ProjectWorldToNorm(_sprite.Position);
            var uiPosition = window.NormToUi(new Vector2(normPosition.X, normPosition.Y));
            var heading = S(SystemTextId.MapPopup_Person);

            var options = new List<ContextMenuOption>();

            options.Add(new ContextMenuOption(
                S(SystemTextId.MapPopup_TalkTo),
                new TriggerChainEvent(_npc.EventChain, TriggerType.TalkTo),
                ContextMenuGroup.Actions));

            options.Add(new ContextMenuOption(
                    S(SystemTextId.MapPopup_MainMenu),
                    new PushSceneEvent(SceneId.MainMenu), 
                    ContextMenuGroup.System
                ));

            Raise(new ContextMenuEvent(uiPosition, heading, options));
            rightClickEvent.Propagating = false;
        }
    }
}