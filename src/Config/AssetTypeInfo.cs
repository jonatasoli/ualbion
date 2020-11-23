﻿using System.Collections.Generic;
using System.Linq;

namespace UAlbion.Config
{
    public class AssetTypeInfo
    {
        public string CopiedFrom { get; set; }
        public string EnumType { get; set; }
        public AssetType AssetType { get; set; }
        public IDictionary<string, AssetFileInfo> Files { get; } = new Dictionary<string, AssetFileInfo>();
        public string Loader { get; set; }
        public string Locator { get; set; }

        public void PostLoad()
        {
            foreach (var file in Files)
            {
                file.Value.Filename = file.Key;
                file.Value.EnumType = this;
                file.Value.PostLoad();
            }
        }

        public void PreSave()
        {
            foreach (var xld in Files)
            {
                if (xld.Value.Transposed == false)
                    xld.Value.Transposed = null;

                foreach (var asset in xld.Value.Assets)
                {
                    if (string.IsNullOrWhiteSpace(asset.Value.Name))
                        asset.Value.Name = null;

                    if (asset.Value.PaletteHints != null && !asset.Value.PaletteHints.Any())
                        asset.Value.PaletteHints = null;
                }
            }
        }

        public void PostSave()
        {
            foreach (var file in Files)
            foreach (var asset in file.Value.Assets)
                asset.Value.PaletteHints ??= new List<int>();
        }
    }
}
