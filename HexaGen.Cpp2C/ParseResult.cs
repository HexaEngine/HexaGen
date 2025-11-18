namespace HexaGen.Cpp2C
{
    using HexaGen.CppAst.Model.Metadata;

    public class ParseResult
    {
        public ParseResult(CppCompilation compilation)
        {
            Compilation = compilation;
        }

        public CppCompilation Compilation { get; set; }
    }
}
