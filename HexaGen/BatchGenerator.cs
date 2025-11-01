namespace HexaGen
{
    using CommandLine;
    using HexaGen.CppAst.Parsing;
    using HexaGen.Metadata;
    using HexaGen.Patching;
    using System.Diagnostics.CodeAnalysis;

    public class BatchGenerator
    {
        private CsCodeGeneratorConfig config = null!;
        private CsCodeGenerator generator = null!;
        private CLIGeneratorOptions? options;
        private readonly List<IPrePatch> prePatches = [];
        private readonly List<IPostPatch> postPatches = [];
        private DateTime start;

        public static BatchGenerator Create()
        {
            BatchGenerator generator = new();
            generator.Start();
            return generator;
        }

        public BatchGenerator Start()
        {
            start = DateTime.Now;
            return this;
        }

        public BatchGenerator Setup<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(string configPath) where T : CsCodeGenerator
        {
            config = CsCodeGeneratorConfig.Load(configPath);
            var type = typeof(T);
            var ctor = type.GetConstructor([typeof(CsCodeGeneratorConfig)]);
            generator = (T)ctor!.Invoke([config]);
            generator.LogToConsole();
            generator.CLIOptions = options;
            foreach (var patch in prePatches)
            {
                generator.PatchEngine.RegisterPrePatch(patch);
            }
            foreach (var patch in postPatches)
            {
                generator.PatchEngine.RegisterPostPatch(patch);
            }
            return this;
        }

        public BatchGenerator WithArgs(string[] args)
        {
            options = Parser.Default.ParseArguments<CLIGeneratorOptions>(args).Value;
            if (generator != null)
            {
                generator.CLIOptions = options;
            }

            return this;
        }

        public BatchGenerator AddGlobalPrePatch(IPrePatch patch)
        {
            prePatches.Add(patch);
            generator?.PatchEngine.RegisterPrePatch(patch);
            return this;
        }

        public BatchGenerator AddGlobalPostPatch(IPostPatch patch)
        {
            postPatches.Add(patch);
            generator?.PatchEngine.RegisterPostPatch(patch);
            return this;
        }

        public BatchGenerator AddPrePatch(IPrePatch patch)
        {
            generator.PatchEngine.RegisterPrePatch(patch);
            return this;
        }

        public BatchGenerator AddPostPatch(IPostPatch patch)
        {
            generator.PatchEngine.RegisterPostPatch(patch);
            return this;
        }

        public BatchGenerator Generate(List<string> sources, string output, List<string>? allowedHeaders = null)
        {
            generator.Generate(sources, output, allowedHeaders);
            return this;
        }

        public BatchGenerator Generate(string source, string output, List<string>? allowedHeaders = null)
        {
            generator.Generate(source, output, allowedHeaders);
            return this;
        }

        public BatchGenerator Generate(CppParserOptions parserOptions, List<string> sources, string output, List<string>? allowedHeaders = null)
        {
            generator.Generate(parserOptions, sources, output, allowedHeaders);
            return this;
        }

        public BatchGenerator Generate(CppParserOptions parserOptions, string source, string output, List<string>? allowedHeaders = null)
        {
            generator.Generate(parserOptions, source, output, allowedHeaders);
            return this;
        }

        public BatchGenerator GetMetadata(out CsCodeGeneratorMetadata metadata)
        {
            metadata = generator.GetMetadata();
            return this;
        }

        public BatchGenerator GetConfig(out CsCodeGeneratorConfig config)
        {
            config = this.config;
            return this;
        }

        public BatchGenerator AlterGenerator(Action<CsCodeGenerator> action)
        {
            action(generator);
            return this;
        }

        public BatchGenerator AlterConfig(Action<CsCodeGeneratorConfig> action)
        {
            action(config);
            return this;
        }

        public BatchGenerator MergeConfig(CsCodeGeneratorConfig baseConfig, MergeOptions options)
        {
            config.Merge(baseConfig, options);
            return this;
        }

        public BatchGenerator CopyFromMetadata(CsCodeGeneratorMetadata metadata)
        {
            generator.CopyFrom(metadata);
            return this;
        }

        public void Finish()
        {
            var end = DateTime.Now;
            var elapsed = end - start;
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"All Done! Generation took {elapsed.TotalSeconds:n2}s");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}