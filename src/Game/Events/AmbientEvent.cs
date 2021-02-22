﻿using UAlbion.Api;
using UAlbion.Formats.Assets;

namespace UAlbion.Game.Events
{
    [Event("ambient")] // USED IN SCRIPT
    public class AmbientEvent : GameEvent
    {
        public AmbientEvent(SongId songId) { SongId = songId; }
        [EventPart("songId")] public SongId SongId { get; }
    }
}
