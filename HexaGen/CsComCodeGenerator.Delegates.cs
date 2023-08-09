namespace HexaGen
{
    public partial class CsComCodeGenerator
    {
        protected override List<string> SetupDelegateUsings()
        {
            List<string> usings = base.SetupDelegateUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }
    }
}