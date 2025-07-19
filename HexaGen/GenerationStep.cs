namespace HexaGen
{
    using HexaGen.Metadata;

    public abstract class GenerationStep : LoggerBase
    {
        protected readonly CsCodeGenerator generator;
        protected readonly CsCodeGeneratorConfig config;

        public GenerationStep(CsCodeGenerator generator, CsCodeGeneratorConfig config)
        {
            this.generator = generator;
            this.config = config;
            LogEvent += generator.Log;
        }

        public abstract string Name { get; }

        public bool Enabled { get; set; } = true;

        public abstract void Configure(CsCodeGeneratorConfig config);

        public abstract void Generate(FileSet files, ParseResult result, string outputPath, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata);

        public abstract void CopyToMetadata(CsCodeGeneratorMetadata metadata);

        public abstract void CopyFromMetadata(CsCodeGeneratorMetadata metadata);

        public abstract void Reset();

        public T GetGenerationStep<T>() where T : GenerationStep
        {
            foreach (var step in generator.GenerationSteps)
            {
                if (step is T t)
                {
                    return t;
                }
            }

            throw new InvalidOperationException($"Step of type '{typeof(T)}' was not found.");
        }
    }
}