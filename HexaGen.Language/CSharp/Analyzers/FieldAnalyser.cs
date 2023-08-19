namespace HexaGen.Language.CSharp.Analyzers
{
    using System.Collections.Generic;
    using HexaGen.Language;
    using HexaGen.Language.CSharp.Nodes;

    public class FieldAnalyser : IMemberSyntaxAnalyzer
    {
        public AnalyserResult Analyze(ParserContext context, IReadOnlyList<KeywordType> modifiers)
        {
            string type = context.CurrentToken.AsString();
            context.MoveNext();
            string name = context.CurrentToken.AsString();
            context.MoveNext();

            if (context.CurrentToken.IsPunctuation && context.CurrentToken == ';')
            {
                context.MoveNext();

                FieldNode node = new(type, name, modifiers.ToArray(), null);
                context.AppendNode(node);
                return AnalyserResult.Success;
            }

            if (context.CurrentToken.IsOperator && context.CurrentToken == '=')
            {
                context.MoveNext();
                if (!context.CurrentToken.IsIdentifier)
                {
                    context.Diagnostics.Error("Syntax Error: Expected expression for field", context.CurrentToken.Location);
                    return AnalyserResult.Error;
                }

                string expression = context.CurrentToken.AsString();

                context.MoveNext();

                if (!context.CurrentToken.IsPunctuation || context.CurrentToken != ';')
                {
                    context.Diagnostics.Error("Syntax Error: ; expected", context.CurrentToken.Location);
                    return AnalyserResult.Error;
                }

                context.MoveNext();

                FieldNode node = new(type, name, modifiers.ToArray(), expression);
                context.AppendNode(node);
                return AnalyserResult.Success;
            }

            context.Diagnostics.Error("Syntax Error: ; expected or expression", context.CurrentToken.Location);
            return AnalyserResult.Error;
        }
    }
}