namespace HexaGen
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public delegate (Guid, string)? GuidSelector(string text, Match match);

    public partial class ComGUIDExtractor
    {

        private readonly List<(Regex regex, GuidSelector selector)> patterns = [];
        private readonly Regex regex = RegexExtraceGUID();

        [GeneratedRegex("DEFINE_GUID\\((.*?)\\)", RegexOptions.Compiled | RegexOptions.Singleline)]
        private static partial Regex RegexExtraceGUID();
        public ComGUIDExtractor()
        {
            AddPattern(regex, DefaultSelector);
        }

        public virtual void AddPattern(Regex regex, GuidSelector? selector)
        {
            patterns.Add((regex, selector ?? DefaultSelector));
        }

        private static (Guid, string)? DefaultSelector(string text, Match match)
        {
            if (match.Groups.Count < 2) return null;
            var group = match.Groups[1];
            var parts = group.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length < 12 || parts.Any(x => x.Length < 2 || x.Any(x => !char.IsAsciiHexDigit(x)))) return null;

            var name = parts[0].Replace("IID_", string.Empty);
            var a = uint.Parse(parts[1].AsSpan(2), NumberStyles.HexNumber);
            var b = ushort.Parse(parts[2].AsSpan(2), NumberStyles.HexNumber);
            var c = ushort.Parse(parts[3].AsSpan(2), NumberStyles.HexNumber);
            var d = byte.Parse(parts[4].AsSpan(2), NumberStyles.HexNumber);
            var e = byte.Parse(parts[5].AsSpan(2), NumberStyles.HexNumber);
            var f = byte.Parse(parts[6].AsSpan(2), NumberStyles.HexNumber);
            var g = byte.Parse(parts[7].AsSpan(2), NumberStyles.HexNumber);
            var h = byte.Parse(parts[8].AsSpan(2), NumberStyles.HexNumber);
            var i = byte.Parse(parts[9].AsSpan(2), NumberStyles.HexNumber);
            var j = byte.Parse(parts[10].AsSpan(2), NumberStyles.HexNumber);
            var k = byte.Parse(parts[11].AsSpan(2), NumberStyles.HexNumber);

            return (new(a, b, c, d, e, f, g, h, i, j, k), name);
        }

        public virtual void ExtractGuidsFromFiles(IEnumerable<string> headerFiles, CsComCodeGenerator generator, List<(string, Guid)> guids, Dictionary<string, Guid> guidMap)
        {
            foreach (var file in headerFiles)
            {
                string text = File.ReadAllText(file);
                ExtractGuids(text, generator.Settings, generator, guids, guidMap);
            }
        }

        public virtual void ExtractGuidsFromFile(string file, CsComCodeGenerator generator, List<(string, Guid)> guids, Dictionary<string, Guid> guidMap)
        {
            string text = File.ReadAllText(file);
            ExtractGuids(text, generator.Settings, generator, guids, guidMap);
        }

        public virtual void ExtractGuids(string text, CsCodeGeneratorConfig config, CsComCodeGenerator generator, List<(string, Guid)> guids, Dictionary<string, Guid> guidMap)
        {
            foreach (var (regex, selector) in patterns)
            {
                DoMatch(text, config, generator, guids, guidMap, regex, selector);
            }
        }

        public virtual void DoMatch(string text, CsCodeGeneratorConfig config, CsComCodeGenerator generator, List<(string, Guid)> guids, Dictionary<string, Guid> guidMap, Regex regex, GuidSelector selector)
        {
            var match = regex.Matches(text);
            for (int x = 0; x < match.Count; x++)
            {
                (Guid guid, string name)? item = selector(text, match[x]);

                if (!item.HasValue)
                {
                    continue;
                }

                (Guid guid, string name) = item.Value;

                if (config.IIDMappings.ContainsKey(name))
                    continue;


                if (guidMap.TryGetValue(name, out Guid other))
                {
                    if (other != guid)
                    {
                        generator.LogWarn($"overwriting GUID {other} with {guid} for {name}");
                        guidMap[name] = guid;
                        guids.Remove((name, other));
                        guids.Add((name, guid));
                    }
                }
                else
                {
                    guids.Add((name, guid));
                    guidMap.Add(name, guid);
                }
            }
        }
    }
}