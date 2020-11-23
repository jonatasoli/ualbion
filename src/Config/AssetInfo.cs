﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace UAlbion.Config
{
    public class AssetInfo
    {
        [JsonIgnore] public int SubAssetId { get; internal set; } // Sub-asset offset in the container file (or 0 if not inside a container)
        [JsonIgnore] public AssetFileInfo File { get; internal set; }
        [JsonIgnore] public int EffectiveWidth => Width ?? File.Width ?? 0; // For sprites only
        [JsonIgnore] public int EffectiveHeight => Height ?? File.Height ?? 0; // For sprites only
        [JsonIgnore] public bool Transposed => File.Transposed ?? false; // For sprites only
        [JsonIgnore] public AssetId AssetId { get; internal set; }

        public int Id { get; set; } // Id of this asset in the mapped enum type.
        public string Name { get; set; } // Debug/console name of the asset, used to build the autogenerated enum types.
        public int? Width { get; set; } // For sprites only
        public int? Height { get; set; } // For sprites only
        public string SubSprites { get; set; } // For amorphous sprites only
        public bool? UseSmallGraphics { get; set; } // For sprites only
        public long? Offset { get; set; } // For core sprites only
        public Position2D Hotspot { get; set; } // For core sprites only

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Play nicely with JSON serialisation")]
        public IList<int> PaletteHints { get; set; } // Just for displaying things in ImageReverser - the game doesn't use these.

        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Play nicely with JSON serialisation")]
        public IList<string> AnimatedRanges { get; set; } // For palettes only.
    }
}
