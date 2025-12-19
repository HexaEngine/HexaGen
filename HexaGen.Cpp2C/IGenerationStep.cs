namespace HexaGen.Cpp2C
{
    using HexaGen.Core;
    using HexaGen.Cpp2C.Metadata;

    public interface IGenerationStep
    {
        bool Enabled { get; set; }
        string Name { get; }

        void Configure(Cpp2CGeneratorConfig config);
        void CopyFromMetadata(Cpp2CGeneratorMetadata metadata);
        void CopyToMetadata(Cpp2CGeneratorMetadata metadata);
        void Generate(FileSet files, ParseResult result, string outputPath, Cpp2CGeneratorConfig config, Cpp2CGeneratorMetadata metadata);
        T GetGenerationStep<T>() where T : GenerationStep;
        void Reset();
    }
}