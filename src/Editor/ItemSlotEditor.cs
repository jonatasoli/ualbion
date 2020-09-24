﻿using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using UAlbion.Formats.AssetIds;
using UAlbion.Formats.Assets;
using UAlbion.Game.Assets;

namespace UAlbion.Editor
{
    public class ItemSlotEditor : AssetEditor
    {
        static readonly string[] _itemNames;
        static readonly IDictionary<ItemId, int> _itemNameIndexLookup;
        readonly ItemSlot _slot;

        // TODO: Nice visual editor with icons etc

        static ItemSlotEditor()
        {
            var itemIdInfo =
                Enum.GetValues(typeof(ItemId))
                    .OfType<ItemId>()
                    .Select(x => (x, x.ToString()))
                    .OrderBy(x => x.Item2)
                    .Select((x,i) => (x.x, i, x.Item2))
                    .ToArray();

            _itemNames = new[] { "Empty" }.Concat(itemIdInfo.Select(x => x.Item3)).ToArray();
            _itemNameIndexLookup = itemIdInfo.ToDictionary(x => x.x, x => x.i + 1);
        }

        static int IndexForItemId(ItemId? x) => x == null ? 0 : _itemNameIndexLookup[x.Value];
        static ItemId? ItemIdForIndex(int x) => x == 0 ? (ItemId?)null : Enum.Parse<ItemId>(_itemNames[x]);

        public ItemSlotEditor(ItemSlot slot) : base(slot)
        {
            _slot = slot ?? throw new ArgumentNullException(nameof(slot));
        }

        public override void Render()
        {
            int index = IndexForItemId(_slot.ItemId);
            int oldIndex = index;
            ImGui.Combo(_slot.Id.Slot.ToString(), ref index, _itemNames, _itemNames.Length);
            if (index != oldIndex)
            {
                var assetId = Resolve<IEditorAssetManager>().GetIdForAsset(Asset);
                var itemId = ItemIdForIndex(index);
                var newItem = itemId switch
                {
                    null => (IContents)null,
                    ItemId.Gold => new Gold(),
                    ItemId.Rations => new Rations(),
                    _ => Resolve<IRawAssetManager>().LoadItem(itemId.Value)
                };
                Raise(new EditorSetPropertyEvent(assetId, nameof(_slot.Item), _slot.Item, newItem));
            }

            if (_slot.Item != null)
            {
                if (_slot.Item is ItemData item)
                {
                    if (item.IsStackable)
                    {
                        ImGui.SameLine();
                        UInt16Slider(nameof(_slot.Amount), _slot.Amount, 1, ItemSlot.MaxItemCount);
                    }

                    if (item.MaxCharges > 0)
                    {
                        ImGui.SameLine();
                        UInt8Slider(nameof(_slot.Charges), _slot.Charges, 0, item.MaxCharges);
                    }

                    if (item.MaxEnchantmentCount > 0)
                    {
                        ImGui.SameLine();
                        UInt8Slider(nameof(_slot.Enchantment), _slot.Enchantment, 0, item.MaxEnchantmentCount);
                    }
                    // TODO Flags: Broken, Cursed, Extra Info, unk, show as icons?
                    // TODO: Clear button?
                }
                else
                {
                    ImGui.SameLine();
                    UInt16Slider(nameof(_slot.Amount), _slot.Amount, 1, ItemSlot.MaxItemCount);
                }
            }
        }
    }
}