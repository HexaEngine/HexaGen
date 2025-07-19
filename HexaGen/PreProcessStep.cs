namespace HexaGen
{
    using HexaGen.CppAst.Model.Metadata;
    using HexaGen.Metadata;

    public abstract class PreProcessStep : LoggerBase
    {
        protected readonly CsCodeGenerator generator;
        protected readonly CsCodeGeneratorConfig config;

        public PreProcessStep(CsCodeGenerator generator, CsCodeGeneratorConfig config)
        {
            this.generator = generator;
            this.config = config;
            LogEvent += generator.Log;
        }

        public abstract void Configure(CsCodeGeneratorConfig config);

        public abstract void PreProcess(FileSet files, CppCompilation compilation, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata, ParseResult result);
    }
}