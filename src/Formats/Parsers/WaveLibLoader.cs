﻿using System;
using SerdesNet;
using UAlbion.Config;
using UAlbion.Formats.Assets;

namespace UAlbion.Formats.Parsers
{
    public class WaveLibLoader : IAssetLoader
    {
        public object Serdes(object existing, AssetInfo config, AssetMapping mapping, ISerializer s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (config == null) throw new ArgumentNullException(nameof(config));
            return WaveLib.Serdes(null, s);
        }
    }
}