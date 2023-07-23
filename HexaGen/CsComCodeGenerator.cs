namespace HexaGen
{
    using CppAst;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public partial class CsComCodeGenerator : BaseGenerator
    {
        public CsComCodeGenerator(CsCodeGeneratorSettings settings) : base(settings)
        {
        }

        private List<(string, Guid)> _guids = new();
        private Dictionary<string, Guid> _guidMap = new();
        private Regex regex = RegexExtraceGUID();

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

                Guid guid = new(a, b, c, d, e, f, g, h, i, j, k);
                if (_guidMap.ContainsKey(name))
                {
                    var other = _guidMap[name];
                    if (other != guid)
                    {
                        LogWarn($"overwriting GUID {other} with {guid}");
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
        }

        public void Generate(List<string> headerFiles, string outputPath)
        {
            var options = new CppParserOptions
            {
                ParseMacros = true,
                ParseAttributes = false,
                ParseComments = true,
                ParseSystemIncludes = true,
                ParseAsCpp = true,
            };

            options.ConfigureForWindowsMsvc(CppTargetCpu.X86_64);
            options.AdditionalArguments.Add("-std=c++17");

            for (int i = 0; i < headerFiles.Count; i++)
            {
                string text = File.ReadAllText(headerFiles[i]);
                ExtractGuids(text);
            }

            CppCompilation compilation = CppParser.ParseFiles(headerFiles, options);

            Generate(compilation, outputPath);
        }

        public void Generate(string headerFile, string outputPath)
        {
            var options = new CppParserOptions
            {
                ParseMacros = true,
                ParseAttributes = false,
                ParseComments = true,
                ParseSystemIncludes = true,
                ParseAsCpp = true,
            };

            options.ConfigureForWindowsMsvc(CppTargetCpu.X86_64);
            options.AdditionalArguments.Add("-std=c++17");

            string text = File.ReadAllText(headerFile);
            ExtractGuids(text);

            CppCompilation compilation = CppParser.ParseFile(headerFile, options);

            Generate(compilation, outputPath);
        }

        private void Generate(CppCompilation compilation, string outputPath)
        {
            // Print diagnostic messages
            for (int i = 0; i < compilation.Diagnostics.Messages.Count; i++)
            {
                CppDiagnosticMessage? message = compilation.Diagnostics.Messages[i];
                if (message.Type == CppLogMessageType.Error)
                {
                    LogError(message.ToString());
                }
                if (message.Type == CppLogMessageType.Warning)
                {
                    LogWarn(message.ToString());
                }
                if (message.Type == CppLogMessageType.Info)
                {
                    LogInfo(message.ToString());
                }
            }

            if (compilation.HasErrors)
            {
                return;
            }

            GenerateEnums(compilation, outputPath);
            GenerateTypes(compilation, outputPath);

            /*
            for (int i = 0; i < compilation.Classes.Count; i++)
            {
                var @class = compilation.Classes[i];
                var has = _guidMap.TryGetValue(@class.Name, out var guid);

                Console.WriteLine(@class.ToString() + (has ? guid.ToString() : string.Empty));
            }
            for (int i = 0; i < compilation.Fields.Count; i++)
            {
                var field = compilation.Fields[i];

                Console.WriteLine(field);
            }
            for (int i = 0; i < compilation.Macros.Count; i++)
            {
                var macro = compilation.Macros[i];

                Console.WriteLine(macro);
            }
            for (int i = 0; i < compilation.Typedefs.Count; i++)
            {
                var typedef = compilation.Typedefs[i];

                Console.WriteLine(typedef);
            }
            for (int i = 0; i < _guids.Count; i++)
            {
                var guid = _guids[i];

                Console.WriteLine(guid);
            }*/
        }

        [GeneratedRegex("DEFINE_GUID\\((.*?)\\)", RegexOptions.Compiled)]
        private static partial Regex RegexExtraceGUID();
    }
}