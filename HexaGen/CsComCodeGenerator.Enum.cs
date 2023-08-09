namespace HexaGen
{
    using System.Collections.Generic;

    public partial class CsComCodeGenerator
    {
        protected override List<string> SetupEnumUsings()
        {
            var usings = base.SetupEnumUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }
    }
}