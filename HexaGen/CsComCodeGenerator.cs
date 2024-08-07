namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.Logging;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public partial class CsComCodeGenerator : CsCodeGenerator
    {
        private FunctionGenerator funcGen;

        public CsComCodeGenerator(CsCodeGeneratorSettings settings) : base(settings)
        {
            funcGen = new(settings);
        }

        private List<(string, Guid)> _guids = new();
        private Dictionary<string, Guid> _guidMap = new();
        private Regex regex = RegexExtraceGUID();

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

                if (settings.IIDMappings.ContainsKey(name))
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

            foreach (var item in settings.IIDMappings)
            {
                if (!_guidMap.ContainsKey(item.Key))
                {
                    _guidMap.Add(item.Key, new(item.Value));
                    _guids.Add((item.Key, new(item.Value)));
                }
            }
        }

        public override bool Generate(List<string> headerFiles, string outputPath)
        {
            var options = PrepareSettings();

            for (int i = 0; i < headerFiles.Count; i++)
            {
                string text = File.ReadAllText(headerFiles[i]);
                ExtractGuids(text);
            }

            CppCompilation compilation = CppParser.ParseFiles(headerFiles, options);

            return Generate(compilation, headerFiles, outputPath);
        }

        public override bool Generate(string headerFile, string outputPath)
        {
            var options = PrepareSettings();

            string text = File.ReadAllText(headerFile);
            ExtractGuids(text);

            CppCompilation compilation = CppParser.ParseFile(headerFile, options);

            return Generate(compilation, [headerFile], outputPath);
        }

        public override bool Generate(CppCompilation compilation, List<string> headerFiles, string outputPath)
        {
            Directory.CreateDirectory(outputPath);
            // Print diagnostic messages
            for (int i = 0; i < compilation.Diagnostics.Messages.Count; i++)
            {
                CppDiagnosticMessage? message = compilation.Diagnostics.Messages[i];
                if (message.Type == CppLogMessageType.Error && settings.CppLogLevel <= LogSeverity.Error)
                {
                    LogError(message.ToString());
                }
                if (message.Type == CppLogMessageType.Warning && settings.CppLogLevel <= LogSeverity.Warning)
                {
                    LogWarn(message.ToString());
                }
                if (message.Type == CppLogMessageType.Info && settings.CppLogLevel <= LogSeverity.Information)
                {
                    LogInfo(message.ToString());
                }
            }

            if (compilation.HasErrors)
            {
                return false;
            }

            List<Task> tasks = new();

            if (settings.GenerateEnums)
            {
                Task taskEnums = new(() => GenerateEnums(compilation, outputPath));
                tasks.Add(taskEnums);
                taskEnums.Start();
            }

            if (settings.GenerateConstants)
            {
                Task taskConstants = new(() => GenerateConstants(compilation, outputPath));
                tasks.Add(taskConstants);
                taskConstants.Start();
            }

            if (settings.GenerateHandles)
            {
                Task taskHandles = new(() => GenerateHandles(compilation, outputPath));
                tasks.Add(taskHandles);
                taskHandles.RunSynchronously();
            }

            if (settings.GenerateTypes)
            {
                Task taskTypes = new(() => GenerateTypes(compilation, outputPath));
                tasks.Add(taskTypes);
                taskTypes.RunSynchronously();
            }

            if (settings.GenerateFunctions)
            {
                Task taskFuncs = new(() => GenerateFunctions(compilation, outputPath));
                tasks.Add(taskFuncs);
                taskFuncs.Start();
            }

            if (settings.GenerateExtensions)
            {
                Task taskExtensions = new(() => GenerateExtensions(compilation, outputPath));
                tasks.Add(taskExtensions);
                taskExtensions.Start();
            }

            if (settings.GenerateDelegates)
            {
                Task taskDelegates = new(() => GenerateDelegates(compilation, outputPath));
                tasks.Add(taskDelegates);
                taskDelegates.Start();
            }

            bool failed = false;
            for (int i = 0; i < tasks.Count; i++)
            {
                var task = tasks[i];
                if (task.Exception != null)
                {
                    LogError(task.Exception.ToString());
                    failed = true;
                }
            }

            return !failed;
        }
    }
}