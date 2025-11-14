namespace HexaGen.Cpp2C
{
    using CppAst;
    using HexaGen.Core.Logging;
    using HexaGen.CppAst.Diagnostics;
    using HexaGen.CppAst.Model.Metadata;
    using HexaGen.CppAst.Parsing;

    public partial class Cpp2CCodeGenerator : BaseGenerator
    {
        private Metadata metadata = new();

        public Cpp2CCodeGenerator(Cpp2CCodeGeneratorConfig settings) : base(settings)
        {
        }

        protected virtual CppParserOptions PrepareSettings()
        {
            var options = new CppParserOptions
            {
                ParseMacros = true,
                ParseComments = true,
                ParseSystemIncludes = true,
                ParserKind = CppParserKind.Cpp,
                AutoSquashTypedef = true,
            };

            for (int i = 0; i < settings.AdditionalArguments.Count; i++)
            {
                options.AdditionalArguments.Add(settings.AdditionalArguments[i]);
            }

            for (int i = 0; i < settings.IncludeFolders.Count; i++)
            {
                options.IncludeFolders.Add(settings.IncludeFolders[i]);
            }

            for (int i = 0; i < settings.SystemIncludeFolders.Count; i++)
            {
                options.SystemIncludeFolders.Add(settings.SystemIncludeFolders[i]);
            }

            for (int i = 0; i < settings.Defines.Count; i++)
            {
                options.Defines.Add(settings.Defines[i]);
            }

            options.ConfigureForWindowsMsvc(CppTargetCpu.X86_64);
            options.AdditionalArguments.Add("-std=c++17");

            return options;
        }

        public virtual void Generate(string headerFile, string outputPath)
        {
            var options = PrepareSettings();

            var compilation = CppParser.ParseFile(headerFile, options);

            Generate(compilation, outputPath);
        }

        public virtual void Generate(List<string> headerFiles, string outputPath)
        {
            var options = PrepareSettings();

            var compilation = CppParser.ParseFiles(headerFiles, options);

            Generate(compilation, outputPath);
        }

        public virtual void Generate(CppCompilation compilation, string outputPath)
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
                return;
            }

            List<Task> tasks = new();

            tasks.Add(Task.Run(() => GenerateClasses(compilation, outputPath)));

            Task.WaitAll([.. tasks]);
        }
    }
}