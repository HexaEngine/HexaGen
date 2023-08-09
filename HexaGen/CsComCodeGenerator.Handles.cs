namespace HexaGen
{
    using CppAst;
    using HexaGen;
    using System.IO;

    public partial class CsComCodeGenerator
    {
        protected override List<string> SetupHandleUsings()
        {
            var usings = base.SetupHandleUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }
    }
}