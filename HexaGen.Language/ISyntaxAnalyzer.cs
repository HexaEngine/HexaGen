namespace HexaGen.Language
{
    public interface ISyntaxAnalyzer
    {
        AnalyserResult Analyze(ParserContext context);
    }

    public interface IMemberSyntaxAnalyzer
    {
        AnalyserResult Analyze(ParserContext context, IReadOnlyList<KeywordType> modifiers);
    }
}