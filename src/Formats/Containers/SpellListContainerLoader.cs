﻿using System;
using System.IO;
using SerdesNet;
using UAlbion.Config;
using UAlbion.Formats.Assets;

namespace UAlbion.Formats.Containers
{
    [ContainerLoader(ContainerFormat.SpellList)]
    public class SpellListContainerLoader : IContainerLoader
    {
        public ISerializer Open(string file, AssetInfo info)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            var stream = File.OpenRead(file);
            var br = new BinaryReader(stream);
            stream.Position = info.SubAssetId * SpellData.SizeOnDisk;
            return new AlbionReader(br, SpellData.SizeOnDisk);
        }
    }
}