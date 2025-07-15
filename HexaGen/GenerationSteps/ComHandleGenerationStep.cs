namespace HexaGen.Batteries.Legacy.Steps
{
    public class ComHandleGenerationStep : HandleGenerationStep
    {
        public ComHandleGenerationStep(CsCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
        }

        protected override List<string> SetupHandleUsings()
        {
            var usings = base.SetupHandleUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }
    }
}