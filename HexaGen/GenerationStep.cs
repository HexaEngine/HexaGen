namespace HexaGen
{
    using CppAst;
    using HexaGen.Metadata;

    public abstract class GenerationStep
    {
        public abstract string Name { get; }

        public abstract void Execute(CppCompilation compilation, string outputPath, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata);
    }
}