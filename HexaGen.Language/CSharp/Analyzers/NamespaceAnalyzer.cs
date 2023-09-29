using HexaGen.Language.CSharp.Nodes;

namespace HexaGen.Language.CSharp.Analyzers
{
    public class NamespaceAnalyzer : ISyntaxAnalyzer
    {
        public AnalyserResult Analyze(ParserContext context)
        {
            if (!context.SeekInBounds(2))
            {
                return AnalyserResult.Unrecognised;
            }

            if (context.CurrentToken == KeywordType.Namespace)
            {
                context.MoveNext();

                if (!context.CurrentToken.IsIdentifier)
                {
                    context.Diagnostics.Error("Syntax Error: Expected namespace identifier", context.CurrentToken.Location);
                    return AnalyserResult.Error;
                }

                NamespaceNode node = new(context.CurrentToken.AsString());
                context.MoveNext();

                return context.AnalyseScoped(node);
            }

            return AnalyserResult.Unrecognised;
        }
    }
}