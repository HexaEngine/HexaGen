namespace HexaGen
{
    using System.Collections.Generic;

    public partial class CsComCodeGenerator
    {
        protected override List<string> SetupFunctionUsings()
        {
            var usings = base.SetupFunctionUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }
    }
}