namespace HexaGen
{
    using CppAst;
    using HexaGen.Batteries.Legacy.Steps;
    using HexaGen.FunctionGeneration;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public partial class CsComCodeGenerator : CsCodeGenerator
    {
        public CsComCodeGenerator(CsCodeGeneratorConfig settings) : base(settings)
        {
        }

        private readonly List<(string, Guid)> _guids = [];
        private readonly Dictionary<string, Guid> _guidMap = [];
        private readonly Regex regex = RegexExtraceGUID();

        [GeneratedRegex("DEFINE_GUID\\((.*?)\\)", RegexOptions.Compiled | RegexOptions.Singleline)]
        private static partial Regex RegexExtraceGUID();

        public Guid? GetGUID(string name)
        {
            if (_guidMap.TryGetValue(name, out Guid guid))
            {
                return guid;
            }
            return null;
        }

        public bool TryGetGUID(string name, out Guid guid)
        {
            return _guidMap.TryGetValue(name, out guid);
        }

        public bool HasGUID(string name)
        {
            return _guidMap.ContainsKey(name);
        }

        private void ExtractGuids(string text)
        {
            var match = regex.Matches(text);
            for (int x = 0; x < match.Count; x++)
            {
                var group = match[x].Groups[1];
                var parts = group.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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

                if (config.IIDMappings.ContainsKey(name))
                    continue;

                Guid guid = new(a, b, c, d, e, f, g, h, i, j, k);
                if (_guidMap.ContainsKey(name))
                {
                    var other = _guidMap[name];
                    if (other != guid)
                    {
                        LogWarn($"overwriting GUID {other} with {guid} for {name}");
                        _guidMap[name] = guid;
                        _guids.Remove((name, other));
                        _guids.Add((name, guid));
                    }
                }
                else
                {
                    _guids.Add((name, guid));
                    _guidMap.Add(name, guid);
                }
            }

            foreach (var item in config.IIDMappings)
            {
                if (!_guidMap.ContainsKey(item.Key))
                {
                    _guidMap.Add(item.Key, new(item.Value));
                    _guids.Add((item.Key, new(item.Value)));
                }
            }
        }

        protected override void ConfigureGeneratorCore(List<GenerationStep> generationSteps, out FunctionGenerator functionGenerator)
        {
            functionGenerator = FunctionGenerator.CreateForCOM(config);
            generationSteps.Add(new ComEnumGenerationStep(this, config));
            generationSteps.Add(new ComConstantGenerationStep(this, config));
            generationSteps.Add(new ComHandleGenerationStep(this, config));
            generationSteps.Add(new ComTypeGenerationStep(this, config));
            generationSteps.Add(new ComFunctionGenerationStep(this, config));
            generationSteps.Add(new ComExtensionGenerationStep(this, config));
            generationSteps.Add(new ComDelegateGenerationStep(this, config));
        }

        protected override CppCompilation ParseFiles(List<string> cppFiles)
        {
            for (int i = 0; i < cppFiles.Count; i++)
            {
                string text = File.ReadAllText(cppFiles[i]);
                ExtractGuids(text);
            }

            return base.ParseFiles(cppFiles);
        }
    }
}