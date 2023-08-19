namespace HexaGen.Language.Cpp
{
    public class CppParser : ParserBase
    {
        public CppParser() : this(ParserOptions.Default)
        {
        }

        public CppParser(ParserOptions options) : base(options)
        {
        }
    }
}