namespace HexaGen
{
    using System.Collections.Generic;

    public partial class CsComCodeGenerator
    {
        protected override List<string> SetupConstantUsings()
        {
            var usings = base.SetupConstantUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }
    }
}