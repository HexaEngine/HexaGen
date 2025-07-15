namespace HexaGen.Batteries.Legacy.Steps
{
    public class ComDelegateGenerationStep : DelegateGenerationStep
    {
        public ComDelegateGenerationStep(CsCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
        }

        protected override List<string> SetupDelegateUsings()
        {
            List<string> usings = base.SetupDelegateUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }
    }
}