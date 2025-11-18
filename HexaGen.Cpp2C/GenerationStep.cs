namespace HexaGen.Cpp2C
{
    using HexaGen.Core;
    using HexaGen.Cpp2C.Metadata;

    public abstract class GenerationStep : LoggerBase
    {
        protected readonly Cpp2CCodeGenerator generator;
        protected readonly Cpp2CGeneratorConfig config;

        public GenerationStep(Cpp2CCodeGenerator generator, Cpp2CGeneratorConfig config)
        {
            this.generator = generator;
            this.config = config;
            LogEvent += generator.Log;
        }

        public abstract string Name { get; }

        public bool Enabled { get; set; } = true;

        public abstract void Configure(Cpp2CGeneratorConfig config);

        public abstract void Generate(FileSet files, ParseResult result, string outputPath, Cpp2CGeneratorConfig config, Cpp2CGeneratorMetadata metadata);

        public abstract void CopyToMetadata(Cpp2CGeneratorMetadata metadata);

        public abstract void CopyFromMetadata(Cpp2CGeneratorMetadata metadata);

        public abstract void Reset();

        public T GetGenerationStep<T>() where T : GenerationStep
        {
            return generator.GetGenerationStep<T>();
        }
    }
}
