namespace HexaGen.Cpp2C.GenerationSteps
{
    using HexaGen.Core;
    using HexaGen.Cpp2C.Metadata;
    using System;

    public class ClassGenerationStep : GenerationStep
    {
        public ClassGenerationStep(Cpp2CCodeGenerator generator, Cpp2CGeneratorConfig config) : base(generator, config)
        {
        }

        public override string Name { get; } = "Class Generation Step";

        public override void Configure(Cpp2CGeneratorConfig config)
        {
            throw new NotImplementedException();
        }

        public override void CopyFromMetadata(Cpp2CGeneratorMetadata metadata)
        {
            throw new NotImplementedException();
        }

        public override void CopyToMetadata(Cpp2CGeneratorMetadata metadata)
        {
            throw new NotImplementedException();
        }

        public override void Generate(FileSet files, ParseResult result, string outputPath, Cpp2CGeneratorConfig config, Cpp2CGeneratorMetadata metadata)
        {
            throw new NotImplementedException();
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
