﻿using System;
using SerdesNet;
using UAlbion.Config;
using UAlbion.Formats.Assets.Labyrinth;

namespace UAlbion.Formats.Parsers
{
    public class LabyrinthDataLoader : IAssetLoader<LabyrinthData>
    {
        public LabyrinthData Serdes(LabyrinthData existing, AssetInfo info, AssetMapping mapping, ISerializer s)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            return LabyrinthData.Serdes(info.AssetId.ToInt32(), existing, mapping, s);
        }

        public object Serdes(object existing, AssetInfo info, AssetMapping mapping, ISerializer s)
            => Serdes(existing as LabyrinthData, info, mapping, s);
    }
}
