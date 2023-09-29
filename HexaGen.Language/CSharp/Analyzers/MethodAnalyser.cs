namespace HexaGen.Language.CSharp.Analyzers
{
    using HexaGen.Language.CSharp.Nodes;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class MethodAnalyser : IMemberSyntaxAnalyzer
    {
        private readonly List<Token> parameters = new();

        public AnalyserResult Analyze(ParserContext context, IReadOnlyList<KeywordType> modifiers)
        {
            if (!context.SeekInBounds(2))
            {
                return AnalyserResult.Unrecognised;
            }

            string returnType = context.CurrentToken.AsString();

            context.MoveNext();

            if (context.IsEnd || !context.CurrentToken.IsIdentifier)
            {
                context.Diagnostics.Error("Syntax Error: Expected method identifier", context.IsEnd ? null : context.CurrentToken.Location);
                return AnalyserResult.Error;
            }

            string name = context.CurrentToken.AsString();

            context.MoveNext();

            if (context.IsEnd || !context.CurrentToken.IsPunctuation || context.CurrentToken != '(')
            {
                context.Diagnostics.Error("Syntax Error: Expected token (", context.IsEnd ? null : context.CurrentToken.Location);
                return AnalyserResult.Error;
            }

            context.MoveNext();

            while (context.TryMoveNext(out var current))
            {
                if (current.IsIdentifier)
                {
                    parameters.Add(current);
                }
                else if (current.IsPunctuation && current == ',')
                {
                    continue;
                }
                else if (current.IsPunctuation && current == ')')
                {
                    break;
                }
                else
                {
                    parameters.Clear();
                    context.Diagnostics.Error("Syntax Error: Expected token ) or parameter", current.Location);
                    return AnalyserResult.Error;
                }
            }

            string[] @params = new string[parameters.Count];
            for (int i = 0; i < @params.Length; i++)
            {
                @params[i] = parameters[i].AsString();
            }
            parameters.Clear();

            MethodNode node = new(name, modifiers.ToArray(), @params, returnType);

            return context.AnalyseScoped(node);
        }
    }
}