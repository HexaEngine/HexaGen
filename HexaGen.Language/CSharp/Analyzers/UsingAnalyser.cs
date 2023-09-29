namespace HexaGen.Language.CSharp.Analyzers
{
    using System;
    using HexaGen.Language;
    using HexaGen.Language.CSharp.Nodes;

    public class UsingAnalyser : ISyntaxAnalyzer
    {
        public AnalyserResult Analyze(ParserContext context)
        {
            if (!context.SeekInBounds(2))
            {
                return AnalyserResult.Unrecognised;
            }

            if (context.CurrentToken == KeywordType.Using)
            {
                context.MoveNext();

                if (!context.CurrentToken.IsIdentifier)
                {
                    context.Diagnostics.Error("Syntax Error: Expected using identifier", context.CurrentToken.Location);
                    return AnalyserResult.Error;
                }

                var name = context.CurrentToken.AsString();

                context.MoveNext();

                if (!context.CurrentToken.IsPunctuation || context.CurrentToken != ';')
                {
                    context.Diagnostics.Error("Syntax Error: ; expected", context.CurrentToken.Location);
                    return AnalyserResult.Error;
                }

                context.MoveNext();

                UsingNode node = new(name);
                context.AppendNode(node);

                return AnalyserResult.Success;
            }

            return AnalyserResult.Unrecognised;
        }
    }
}