namespace HexaGen.GenerationSteps
{
    using System.Collections.Generic;

    public class ComEnumGenerationStep : EnumGenerationStep
    {
        public ComEnumGenerationStep(CsCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
        }

        protected override List<string> SetupEnumUsings()
        {
            var usings = base.SetupEnumUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }
    }
}