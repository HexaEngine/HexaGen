namespace HexaGen.Language.Cpp.Analysers
{
    public class ExpressionNode : SyntaxNode
    {
    }

    public class FunctionCallNode : SyntaxNode
    {
        public FunctionCallNode(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class CastNode : SyntaxNode
    {
        public CastNode(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class OperatorNode : SyntaxNode
    {
        public OperatorNode(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class TypeNode : SyntaxNode
    {
        public TypeNode(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class VariableNode : SyntaxNode
    {
        public VariableNode(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class ValueNode : SyntaxNode
    {
        public ValueNode(string value, LiteralType type, NumberType numberType)
        {
            Value = value;
            Type = type;
            NumberType = numberType;
        }

        public string Value { get; set; }

        public LiteralType Type { get; set; }

        public NumberType NumberType { get; set; }
    }

    public class FunctionCallAnalyser : ISyntaxAnalyzer
    {
        public AnalyserResult Analyze(ParserContext context)
        {
            if (!context.CurrentToken.IsIdentifier)
            {
                return AnalyserResult.Unrecognised;
            }

            string name = context.CurrentToken.AsString();

            context.MoveNext();

            if (context.IsEnd)
            {
                context.MoveBack();
                return AnalyserResult.Unrecognised;
            }

            if (!context.CurrentToken.IsPunctuation || context.CurrentToken != '(')
            {
                return AnalyserResult.Unrecognised;
            }

            FunctionCallNode node = new(name);

            context.PushScope(node);

            ExpressionNode expression = new();
            context.PushScope(expression);

            context.MoveNext();

            while (!context.IsEnd)
            {
                if (context.CurrentToken.IsPunctuation && context.CurrentToken == ')')
                {
                    context.PopScope(context.CurrentToken.Location);
                    context.PopScope(context.CurrentToken.Location);
                    context.MoveNext();
                    return AnalyserResult.Success;
                }

                if (context.CurrentToken.IsPunctuation && context.CurrentToken == ',')
                {
                    context.PopScope(context.CurrentToken.Location);
                    expression = new();
                    context.PushScope(expression);
                    context.MoveNext();
                }

                if (context.CurrentToken.IsPunctuation && context.CurrentToken == '(')
                {
                    context.MoveNext();
                    if (!context.CurrentToken.IsIdentifier)
                    {
                    }

                    var cast = context.CurrentToken.AsString();
                    CastNode castNode = new(cast);
                    context.AppendNode(castNode);
                    context.MoveNext();

                    if (!context.CurrentToken.IsPunctuation || context.CurrentToken != ')')
                    {
                        context.Diagnostics.Error("Syntax Error: ) expected", context.CurrentToken.Location);
                        return AnalyserResult.Error;
                    }

                    context.MoveNext();
                }

                if (context.CurrentToken.IsOperator)
                {
                    OperatorNode operatorNode = new(context.CurrentToken.AsString());
                    context.AppendNode(operatorNode);
                }

                if (context.CurrentToken.IsIdentifier)
                {
                    VariableNode variableNode = new(context.CurrentToken.AsString());
                    context.AppendNode(variableNode);
                }

                if (context.CurrentToken.IsLiteral)
                {
                    ValueNode variableNode = new(context.CurrentToken.AsString(), context.CurrentToken.LiteralType, context.CurrentToken.NumberType);
                    context.AppendNode(variableNode);
                }

                context.MoveNext();
            }

            context.Diagnostics.Error("Syntax Error: ) expected", context.CurrentToken.Location);
            return AnalyserResult.Error;
        }
    }

    public class ExpressionAnalyser : ISyntaxAnalyzer
    {
        private FunctionCallAnalyser functionCallAnalyser = new();

        public AnalyserResult Analyze(ParserContext context)
        {
            if (context.CurrentToken.IsPunctuation && context.CurrentToken == '(')
            {
                context.MoveNext();
            }

            if (context.IsEnd)
            {
                context.MoveBack();
                return AnalyserResult.Unrecognised;
            }

            ExpressionNode expression = new();
            context.PushScope(expression);

            while (!context.IsEnd)
            {
                if (context.CurrentToken.IsPunctuation && context.CurrentToken == ')')
                {
                    context.PopScope(context.CurrentToken.Location);
                    context.MoveNext();
                    return AnalyserResult.Success;
                }

                if (context.CurrentToken.IsPunctuation && context.CurrentToken == '(')
                {
                    context.MoveNext();
                    if (!context.CurrentToken.IsIdentifier)
                    {
                    }

                    var name = context.CurrentToken.AsString();
                    CastNode node = new(name);
                    context.AppendNode(node);
                    context.MoveNext();

                    if (!context.CurrentToken.IsPunctuation || context.CurrentToken != ')')
                    {
                        context.Diagnostics.Error("Syntax Error: ) expected", context.CurrentToken.Location);
                        return AnalyserResult.Error;
                    }

                    context.MoveNext();
                }

                if (context.CurrentToken.IsOperator)
                {
                    OperatorNode operatorNode = new(context.CurrentToken.AsString());
                    context.AppendNode(operatorNode);
                }

                if (context.CurrentCompare(x => x.IsIdentifier) && context.SeekCompare(1, x => x.IsPunctuation && x == '('))
                {
                    functionCallAnalyser.Analyze(context);
                    continue;
                }

                if (context.CurrentToken.IsIdentifier)
                {
                    VariableNode variableNode = new(context.CurrentToken.AsString());
                    context.AppendNode(variableNode);
                }

                if (context.CurrentToken.IsLiteral)
                {
                    ValueNode variableNode = new(context.CurrentToken.AsString(), context.CurrentToken.LiteralType, context.CurrentToken.NumberType);
                    context.AppendNode(variableNode);
                }

                context.MoveNext();
            }

            context.PopScope(null);
            context.MoveNext();
            return AnalyserResult.Success;
        }
    }
}