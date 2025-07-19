namespace TestApp
{
    using HexaGen;
    using HexaGen.CppAst.Parsing;
    using System.Security.Cryptography;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var opt = new CppParserOptions() { AutoSquashTypedef = false };
            opt.ConfigureForWindowsMsvc(CppTargetCpu.X86_64);
            var res = CppParser.Parse("#include <stddef.h>\nsize_t Foo();", opt);
            var config = CsCodeGeneratorConfig.Load("config.json");

            Console.WriteLine("Hello, World!");
        }
    }
}