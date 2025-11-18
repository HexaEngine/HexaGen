namespace HexaGen.Cpp2C
{
    using HexaGen.Core;
    using HexaGen.Core.Logging;
    using HexaGen.Cpp2C.Metadata;
    using HexaGen.CppAst.Diagnostics;
    using HexaGen.CppAst.Model.Metadata;
    using HexaGen.CppAst.Parsing;
    using Newtonsoft.Json;
    using System.Diagnostics.CodeAnalysis;

    public partial class Cpp2CCodeGenerator : BaseGenerator
    {
        private readonly Cpp2CGeneratorMetadata metadata = new();
        private readonly List<GenerationStep> generationSteps = [];
        private readonly List<Cpp2CGeneratorMetadata> copyFromPending = [];

        public Cpp2CCodeGenerator(Cpp2CGeneratorConfig settings) : base(settings)
        {
        }

        public IReadOnlyList<GenerationStep> GenerationSteps => generationSteps;

        public void AddGenerationStep(GenerationStep step)
        {
            generationSteps.Add(step);
        }

        public void AddGenerationStep<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : GenerationStep
        {
            var step = (T)Activator.CreateInstance(typeof(T), this, config)!;
            generationSteps.Add(step);
        }

        public T GetGenerationStep<T>() where T : GenerationStep
        {
            foreach (var step in generationSteps)
            {
                if (step is T t)
                {
                    return t;
                }
            }

            throw new InvalidOperationException($"Generation step of type {typeof(T).Name} not found.");
        }

        public void OverwriteGenerationStep<TTarget>(GenerationStep newStep) where TTarget : GenerationStep
        {
            for (int i = 0; i < generationSteps.Count; i++)
            {
                var step = generationSteps[i];
                if (step is TTarget)
                {
                    generationSteps[i] = newStep;
                }
            }
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

            for (int i = 0; i < config.AdditionalArguments.Count; i++)
            {
                options.AdditionalArguments.Add(config.AdditionalArguments[i]);
            }

            for (int i = 0; i < config.IncludeFolders.Count; i++)
            {
                options.IncludeFolders.Add(config.IncludeFolders[i]);
            }

            for (int i = 0; i < config.SystemIncludeFolders.Count; i++)
            {
                options.SystemIncludeFolders.Add(config.SystemIncludeFolders[i]);
            }

            for (int i = 0; i < config.Defines.Count; i++)
            {
                options.Defines.Add(config.Defines[i]);
            }

            options.ConfigureForWindowsMsvc(CppTargetCpu.X86_64);
            options.AdditionalArguments.Add("-std=c++17");

            return options;
        }

        public virtual void Generate(string headerFile, string outputPath, List<string>? allowedHeaders = null)
        {
            Generate([headerFile], outputPath, allowedHeaders);
        }

        public virtual void Generate(List<string> headerFiles, string outputPath, List<string>? allowedHeaders = null)
        {
            var options = PrepareSettings();

            var compilation = CppParser.ParseFiles(headerFiles, options);

            Generate(compilation, headerFiles, outputPath, allowedHeaders);
        }

        public virtual void Generate(CppCompilation compilation, List<string> headerFiles, string outputPath, List<string>? allowedHeaders)
        {
            if (Directory.Exists(outputPath))
            {
                Directory.Delete(outputPath, true);
            }
            Directory.CreateDirectory(outputPath);
            Directory.CreateDirectory(Path.Combine(outputPath, "include"));
            Directory.CreateDirectory(Path.Combine(outputPath, "src"));
            // Print diagnostic messages
            for (int i = 0; i < compilation.Diagnostics.Messages.Count; i++)
            {
                CppDiagnosticMessage? message = compilation.Diagnostics.Messages[i];
                if (message.Type == CppLogMessageType.Error && config.CppLogLevel <= LogSeverity.Error)
                {
                    LogError(message.ToString());
                }
                if (message.Type == CppLogMessageType.Warning && config.CppLogLevel <= LogSeverity.Warning)
                {
                    LogWarn(message.ToString());
                }
                if (message.Type == CppLogMessageType.Info && config.CppLogLevel <= LogSeverity.Information)
                {
                    LogInfo(message.ToString());
                }
            }

            if (compilation.HasErrors)
            {
                return;
            }

            allowedHeaders ??= [];
            allowedHeaders.AddRange(headerFiles);

            FileSet files = new(allowedHeaders.Select(PathHelper.GetPath));

            LogInfo($"Configuring Steps...");
            foreach (var step in generationSteps)
            {
                step.Configure(config);
            }

            ParseResult result = new(compilation);
            foreach (var step in generationSteps)
            {
                if (step.Enabled)
                {
                    LogInfo($"Generating {step.Name}...");
                    step.Generate(files, result, outputPath, config, metadata);
                    step.CopyToMetadata(metadata);
                }
            }
        }


        public virtual void Reset()
        {
            foreach (var step in GenerationSteps)
            {
                step.Reset();
            }
        }

        public void CopyFrom(Cpp2CGeneratorMetadata metadata)
        {
            copyFromPending.Add(metadata);
        }

        public void SaveMetadata(string path)
        {
            JsonSerializerSettings options = new() { Formatting = Formatting.Indented };
            var json = JsonConvert.SerializeObject(metadata, options);
            File.WriteAllText(path, json);
        }

        public void LoadMetadata(string path)
        {
            var json = File.ReadAllText(path);
            var metadata = JsonConvert.DeserializeObject<Cpp2CGeneratorMetadata>(json) ?? new();
            CopyFrom(metadata);
        }

        public Cpp2CGeneratorMetadata GetMetadata()
        {
            return metadata;
        }
    }
}