namespace HexaGen.Language.Cpp
{
    using HexaGen.Language.Cpp.Analysers;

    public class CppMacroParser : ParserBase
    {
        public CppMacroParser() : this(ParserOptions.Default)
        {
        }

        public CppMacroParser(ParserOptions options) : base(options)
        {
            analyzers.Add(new ExpressionAnalyser());
            analyzers.Add(new FunctionCallAnalyser());
        }
    }
}