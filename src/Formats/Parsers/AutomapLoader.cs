﻿using System;
using SerdesNet;
using UAlbion.Config;
using UAlbion.Formats.Assets;

namespace UAlbion.Formats.Parsers
{
    public class AutomapLoader : IAssetLoader<Automap>
    {
        public Automap Serdes(Automap existing, AssetInfo config, AssetMapping mapping, ISerializer s)
        {
            throw new NotImplementedException();
        }

        public object Serdes(object existing, AssetInfo config, AssetMapping mapping, ISerializer s)
        {
            throw new NotImplementedException();
        }
    }
}