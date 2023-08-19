namespace HexaGen.Language.CSharp.Analyzers
{
    using HexaGen.Language.CSharp.Nodes;
    using System.Collections.Generic;

    public class ClassAnalyzer : ISyntaxAnalyzer
    {
        private readonly List<KeywordType> modifiers = new();

        public AnalyserResult Analyze(ParserContext context)
        {
            if (!context.SeekInBounds(2))
            {
                return AnalyserResult.Unrecognised;
            }

            var start = context.CurrentTokenIndex;
            while (context.TryMoveNext(out var current))
            {
                if (current.IsKeyword)
                {
                    if (current == KeywordType.Class)
                    {
                        break;
                    }
                    else if (current == KeywordType.Public || current == KeywordType.Internal || current == KeywordType.Protected || current == KeywordType.Private || current == KeywordType.Static || current == KeywordType.Unsafe)
                    {
                        modifiers.Add(current.KeywordType);
                    }
                    else
                    {
                        modifiers.Clear();
                        context.MoveTo(start);
                        return AnalyserResult.Unrecognised;
                    }
                }
                else
                {
                    modifiers.Clear();
                    context.MoveTo(start);
                    return AnalyserResult.Unrecognised;
                }
            }

            if (context.IsEnd || !context.CurrentToken.IsIdentifier)
            {
                context.Diagnostics.Error("Syntax Error: Expected class identifier", context.IsEnd ? null : context.CurrentToken.Location);
                return AnalyserResult.Error;
            }

            ClassNode node = new(context.CurrentToken.AsString(), modifiers.ToArray());
            modifiers.Clear();
            context.MoveNext();

            return context.AnalyseScoped(node);
        }
    }
}