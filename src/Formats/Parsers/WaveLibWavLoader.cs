﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;
using SerdesNet;
using UAlbion.Config;
using UAlbion.Formats.Assets;
using static System.FormattableString;

namespace UAlbion.Formats.Parsers
{
    public class WaveLibWavLoader : IAssetLoader<WaveLib>
    {
        static readonly WavLoader WavLoader = new WavLoader();
        static readonly Regex NameRegex = new Regex(@"i(\d+)t(\d+)");
        public WaveLib Serdes(WaveLib existing, AssetInfo info, AssetMapping mapping, ISerializer s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            return s.IsWriting() 
                ? Write(existing, info, s) 
                : Read(s);
        }

        static WaveLib Read(ISerializer s)
        {
            var lib = new WaveLib();
            int i = 0;
            foreach(var (bytes, name) in PackedChunks.Unpack(s))
            {
                if (bytes == null || bytes.Length == 0)
                {
                    lib.Samples[i] = new WaveLibSample();
                    i++;
                    continue;
                }

                var m = NameRegex.Match(name);
                if (!m.Success)
                {
                    throw new FormatException(
                        $"Invalid wavelib entry name \"{name}\". " +
                        "WaveLib entries must be named like A_B_iCtD.wav " +
                        "where A is the wavelib id, B is the sub-id, C is the " +
                        "instrument and D is the type, e.g. 0_5_i129t63.wav");
                }

                var instrument = int.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
                var type = int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
                var sample = FormatUtil.DeserializeFromBytes(bytes, s2 => WavLoader.Serdes(null, null, null, s2));
                lib.Samples[i] = new WaveLibSample
                {
                    Active = true,
                    Type = type,
                    Instrument = instrument,
                    SampleRate = sample.SampleRate,
                    BytesPerSample = sample.BytesPerSample,
                    Channels = sample.Channels,
                    Samples = sample.Samples
                };
                i++;
            }

            for (; i < WaveLib.MaxSamples; i++)
                lib.Samples[i] = new WaveLibSample();

            return lib;
        }

        static WaveLib Write(WaveLib existing, AssetInfo info, ISerializer s)
        {
            if (existing == null) throw new ArgumentNullException(nameof(existing));

            PackedChunks.PackNamed(s, WaveLib.MaxSamples, i =>
            {
                var sample = existing.Samples[i];
                if (!sample.Active)
                    return (Array.Empty<byte>(), null);

                string extension = Invariant($"i{sample.Instrument}t{sample.Type}");
                var pattern = info.Get(AssetProperty.Pattern, "{0}_{1}_{2}.dat");
                var name = info.BuildFilename(pattern, i, extension);
                var bytes= FormatUtil.SerializeToBytes(s2 => WavLoader.Serdes(sample, null, null, s2));
                return (bytes, name);
            });
            return existing;
        }

        public object Serdes(object existing, AssetInfo info, AssetMapping mapping, ISerializer s)
            => Serdes((WaveLib)existing, info, mapping, s);
    }
}