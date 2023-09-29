namespace HexaGen.Language.CSharp.Analyzers
{
    using System.Collections.Generic;
    using HexaGen.Language;

    public class ClassMemberAnalyzer : ISyntaxAnalyzer
    {
        private readonly List<KeywordType> modifiers = new();

        private FieldAnalyser fieldAnalyser = new();
        private MethodAnalyser methodAnalyser = new();

        public enum MemberType
        {
            Unrecognised,
            Field,
            Constructor,
            Destructor,
            Property,
            Method,
        }

        public AnalyserResult Analyze(ParserContext context)
        {
            if (!context.SeekInBounds(2))
            {
                return AnalyserResult.Unrecognised;
            }

            MemberType memberType = MemberType.Unrecognised;
            var start = context.CurrentTokenIndex;
            while (context.TryMoveNext(out var current))
            {
                if (current.IsKeyword)
                {
                    if (current == KeywordType.Void)
                    {
                        context.MoveBack();
                        memberType = MemberType.Method;
                        break;
                    }
                    else if (current == KeywordType.Public || current == KeywordType.Internal || current == KeywordType.Protected || current == KeywordType.Private || current == KeywordType.Readonly || current == KeywordType.Static || current == KeywordType.Const || current == KeywordType.Unsafe)
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
                else if (current.IsIdentifier || current.IsOperator)
                {
                    context.MoveBack();
                    if (context.SeekInBounds(3))
                    {
                        var n0 = context.Seek(1);
                        var n1 = context.Seek(2);
                        var n2 = context.Seek(3);
                        if (n0.IsIdentifier && n1.IsOperator && n1 == '=' && n2.IsOperator && n2 == '>')
                        {
                            memberType = MemberType.Property;
                            break;
                        }
                        if (n0.IsIdentifier && n1.IsOperator && n1 == '=')
                        {
                            memberType = MemberType.Field;
                            break;
                        }
                        if (n0.IsIdentifier && n1 == '{')
                        {
                            memberType = MemberType.Property;
                            break;
                        }
                        if (n0.IsIdentifier && n1 == '(')
                        {
                            memberType = MemberType.Method;
                            break;
                        }
                        if (n0 == '(')
                        {
                            memberType = MemberType.Constructor;
                            break;
                        }
                        if (n0.IsOperator && n0 == '~' && n1.IsIdentifier && n2 == '(')
                        {
                            memberType = MemberType.Destructor;
                            break;
                        }
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

            AnalyserResult result = AnalyserResult.Unrecognised;

            switch (memberType)
            {
                case MemberType.Unrecognised:
                    return AnalyserResult.Unrecognised;

                case MemberType.Field:
                    result = fieldAnalyser.Analyze(context, modifiers);
                    break;

                case MemberType.Constructor:
                    return AnalyserResult.Success;

                case MemberType.Destructor:
                    return AnalyserResult.Success;

                case MemberType.Property:
                    return AnalyserResult.Success;

                case MemberType.Method:
                    result = methodAnalyser.Analyze(context, modifiers);
                    break;
            }

            modifiers.Clear();

            return result;
        }
    }
}