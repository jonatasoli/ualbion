// Note: This file was automatically generated using Tools/GenerateEnums.
// No changes should be made to this file by hand. Instead, the relevant json
// files should be modified and then GenerateEnums should be used to regenerate
// the various types.
using System;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;
using SerdesNet;
using UAlbion.Api;
using UAlbion.Config;

namespace UAlbion.Formats.Assets
{
    [JsonConverter(typeof(ToStringJsonConverter))]
    [TypeConverter(typeof(TickerIdConverter))]
    public readonly struct TickerId : IEquatable<TickerId>, IEquatable<AssetId>, IComparable, ITextureId
    {
        readonly uint _value;
        public TickerId(AssetType type, int id = 0)
        {
            if (!(type == AssetType.None || type == AssetType.Ticker))
                throw new ArgumentOutOfRangeException($"Tried to construct a TickerId with a type of {type}");
#if DEBUG
            if (id < 0 || id > 0xffffff)
                throw new ArgumentOutOfRangeException($"Tried to construct a TickerId with out of range id {id}");
#endif
            _value = (uint)type << 24 | (uint)id;
        }

        TickerId(uint id) 
        {
            _value = id;
            if (!(Type == AssetType.None || Type == AssetType.Ticker))
                throw new ArgumentOutOfRangeException($"Tried to construct a TickerId with a type of {Type}");
        }

        public static TickerId From<T>(T id) where T : unmanaged, Enum => (TickerId)AssetMapping.Global.EnumToId(id);

        public int ToDisk(AssetMapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));
            var (enumType, enumValue) = AssetMapping.Global.IdToEnum(this);
            return mapping.EnumToId(enumType, enumValue).Id;
        }

        public static TickerId FromDisk(int disk, AssetMapping mapping)
        {
            if (mapping == null) throw new ArgumentNullException(nameof(mapping));
            

            var (enumType, enumValue) = mapping.IdToEnum(new TickerId(AssetType.Ticker, disk));
            return (TickerId)AssetMapping.Global.EnumToId(enumType, enumValue);
        }

        public static TickerId SerdesU8(string name, TickerId id, AssetMapping mapping, ISerializer s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            byte diskValue = (byte)id.ToDisk(mapping);
            diskValue = s.UInt8(name, diskValue);
            return FromDisk(diskValue, mapping);
        }

        public static TickerId SerdesU16(string name, TickerId id, AssetMapping mapping, ISerializer s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            ushort diskValue = (ushort)id.ToDisk(mapping);
            diskValue = s.UInt16(name, diskValue);
            return FromDisk(diskValue, mapping);
        }

        public readonly AssetType Type => (AssetType)((_value & 0xff00_0000) >> 24);
        public readonly int Id => (int)(_value & 0xffffff);
        public static TickerId None => new TickerId(AssetType.None);
        public bool IsNone => Type == AssetType.None;

        public override string ToString() => AssetMapping.Global.IdToName(this);
        static AssetType[] _validTypes = { AssetType.Ticker };
        public static TickerId Parse(string s) => AssetMapping.Global.Parse(s, _validTypes);

        public static implicit operator AssetId(TickerId id) => AssetId.FromUInt32(id._value);
        public static implicit operator TickerId(AssetId id) => new TickerId(id.ToUInt32());
        public static implicit operator TickerId(UAlbion.Base.Ticker id) => TickerId.From(id);

        public readonly int ToInt32() => unchecked((int)_value);
        public readonly uint ToUInt32() => _value;
        public static TickerId FromInt32(int id) => new TickerId(unchecked((uint)id));
        public static TickerId FromUInt32(uint id) => new TickerId(id);
        public static bool operator ==(TickerId x, TickerId y) => x.Equals(y);
        public static bool operator !=(TickerId x, TickerId y) => !(x == y);
        public static bool operator ==(TickerId x, AssetId y) => x.Equals(y);
        public static bool operator !=(TickerId x, AssetId y) => !(x == y);
        public static bool operator <(TickerId x, TickerId y) => x.CompareTo(y) == -1;
        public static bool operator >(TickerId x, TickerId y) => x.CompareTo(y) == 1;
        public static bool operator <=(TickerId x, TickerId y) => x.CompareTo(y) != 1;
        public static bool operator >=(TickerId x, TickerId y) => x.CompareTo(y) != -1;
        public bool Equals(TickerId other) => _value == other._value;
        public bool Equals(AssetId other) => _value == other.ToUInt32();
        public override bool Equals(object obj) => obj is ITextureId other && other.ToUInt32() == _value;
        public int CompareTo(object obj) => (obj is ITextureId other) ? _value.CompareTo(other.ToUInt32()) : -1;
        public override int GetHashCode() => unchecked((int)_value);
    }

    public class TickerIdConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) 
            => sourceType == typeof(string) ? true : base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) 
            => value is string s ? TickerId.Parse(s) : base.ConvertFrom(context, culture, value);

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
            destinationType == typeof(string) ? value.ToString() : base.ConvertTo(context, culture, value, destinationType);
    }
}