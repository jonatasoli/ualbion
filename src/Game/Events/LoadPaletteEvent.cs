﻿using UAlbion.Api;
using UAlbion.Formats.Assets;

namespace UAlbion.Game.Events
{
    [Event("load_pal", "Set the current palette", "palette")] // USED IN SCRIPT (as load_paL)
    public class LoadPaletteEvent : GameEvent
    {
        public LoadPaletteEvent(PaletteId paletteId) { PaletteId = paletteId; }
        [EventPart("paletteId")] public PaletteId PaletteId { get; }
    }
}
