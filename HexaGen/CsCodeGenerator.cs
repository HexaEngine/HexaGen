namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.Logging;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public partial class CsCodeGenerator : BaseGenerator
    {
        private readonly FunctionGenerator funcGen;

        public CsCodeGenerator(CsCodeGeneratorSettings settings) : base(settings)
        {
            funcGen = new(settings);
        }

        public void Generate(string headerFile, string outputPath)
        {
            var options = new CppParserOptions
            {
                ParseMacros = true,
            };

            var compilation = CppParser.ParseFile(headerFile, options);

            Generate(compilation, outputPath);
        }

        public void Generate(List<string> headerFiles, string outputPath)
        {
            var options = new CppParserOptions
            {
                ParseMacros = true,
            };

            var compilation = CppParser.ParseFiles(headerFiles, options);

            Generate(compilation, outputPath);
        }

        public void Generate(CppCompilation compilation, string outputPath)
        {
            // Print diagnostic messages
            for (int i = 0; i < compilation.Diagnostics.Messages.Count; i++)
            {
                CppDiagnosticMessage? message = compilation.Diagnostics.Messages[i];
                if (message.Type == CppLogMessageType.Error && settings.CppLogLevel <= LogSevertiy.Error)
                {
                    LogError(message.ToString());
                }
                if (message.Type == CppLogMessageType.Warning && settings.CppLogLevel <= LogSevertiy.Warning)
                {
                    LogWarn(message.ToString());
                }
                if (message.Type == CppLogMessageType.Info && settings.CppLogLevel <= LogSevertiy.Information)
                {
                    LogInfo(message.ToString());
                }
            }

            if (compilation.HasErrors)
            {
                return;
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

            Task.WaitAll(tasks.ToArray());
        }

        public void Reset()
        {
            DefinedConstants.Clear();
            DefinedEnums.Clear();
            DefinedExtensions.Clear();
            DefinedFunctions.Clear();
            DefinedTypedefs.Clear();
            DefinedTypes.Clear();
            DefinedDelegates.Clear();
        }

        public void CopyFrom(List<string> constants, List<string> enums, List<string> extensions, List<string> functions, List<string> typedefs, List<string> types, List<string> delegates)
        {
            for (int i = 0; i < constants.Count; i++)
            {
                LibDefinedConstants.Add(constants[i]);
            }
            for (int i = 0; i < enums.Count; i++)
            {
                LibDefinedEnums.Add(enums[i]);
            }
            for (int i = 0; i < extensions.Count; i++)
            {
                LibDefinedExtensions.Add(extensions[i]);
            }
            for (int i = 0; i < functions.Count; i++)
            {
                LibDefinedFunctions.Add(functions[i]);
            }
            for (int i = 0; i < typedefs.Count; i++)
            {
                LibDefinedTypedefs.Add(typedefs[i]);
            }
            for (int i = 0; i < types.Count; i++)
            {
                LibDefinedTypes.Add(types[i]);
            }
            for (int i = 0; i < delegates.Count; i++)
            {
                LibDefinedDelegates.Add(delegates[i]);
            }
        }

        private CppFunction FindFunction(CppCompilation compilation, string name)
        {
            for (int i = 0; i < compilation.Functions.Count; i++)
            {
                var function = compilation.Functions[i];
                if (function.Name == name)
                    return function;
            }
            return null;
        }
    }
}