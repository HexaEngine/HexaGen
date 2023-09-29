namespace HexaGen.Language.CSharp
{
    using HexaGen.Language.CSharp.Analyzers;

    public class CSharpParser : ParserBase
    {
        public CSharpParser() : this(ParserOptions.Default)
        {
        }

        public CSharpParser(ParserOptions options) : base(options)
        {
            analyzers.Add(new NamespaceAnalyzer());
            analyzers.Add(new ClassAnalyzer());
            analyzers.Add(new ClassMemberAnalyzer());
            analyzers.Add(new UsingAnalyser());
        }
    }
}