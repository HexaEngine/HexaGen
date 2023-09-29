namespace HexaGen.Language
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class Lexer
    {
        protected readonly List<string> operators = new();
        protected readonly List<char> punctuations = new();
        protected readonly List<string> keywords;
        protected readonly Dictionary<string, KeywordType> keywordMap = new();
        protected readonly List<string> numberNotations = new();

        public Lexer()
        {
            operators.Add("+");
            operators.Add("-");
            operators.Add("*");
            operators.Add("/");
            operators.Add("%");
            operators.Add("<<");
            operators.Add(">>");
            operators.Add("|");
            operators.Add("&");
            operators.Add("^");
            operators.Add("~");
            operators.Add("!");
            operators.Add("=");
            operators.Add("++");
            operators.Add("--");
            operators.Add("&&");
            operators.Add("||");
            operators = operators.OrderByDescending(x => x.Length).ToList();
            numberNotations.Add("e+");
            numberNotations.Add("e-");
            punctuations.Add('(');
            punctuations.Add(')');
            punctuations.Add('{');
            punctuations.Add('}');
            punctuations.Add('[');
            punctuations.Add(']');
            punctuations.Add('<');
            punctuations.Add('>');
            punctuations.Add(';');
            punctuations.Add(',');
            keywords = new(Enum.GetNames<KeywordType>().Skip(1).Select(x => x.ToLowerInvariant()));
            for (int i = 0; i < keywords.Count; i++)
            {
                var keyword = keywords[i];
                keywordMap.Add(keyword, Enum.Parse<KeywordType>(keyword, true));
            }
        }

        private static bool StartsWith(ReadOnlySpan<char> text, string value)
        {
            if (text.Length < value.Length)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (text[i] != value[i])
                    return false;
            }
            return true;
        }

        private static bool StartsWithIgnoreCase(ReadOnlySpan<char> text, string value)
        {
            if (text.Length < value.Length)
                return false;

            for (int i = 0; i < value.Length; i++)
            {
                if (char.ToLower(text[i]) != char.ToLower(value[i]))
                    return false;
            }
            return true;
        }

        public virtual LexerResult Tokenize(string input, string filename)
        {
            DiagnosticBag diagnostics = new();
            List<Token> tokens = new();

            int captureStart = 0;
            int captureEnd = 0;
            bool captured = false;
            bool captureString = false;
            bool captureComment = false;
            bool lineComment = false;
            bool insertBefore = false;

            var length = input.Length;
            char last = input[0];

            int line = 0;
            int column = 0;
            int lastI = 0;
            for (int i = 0; i < length;)
            {
                if (i > 0)
                {
                    last = input[i - 1];
                }

                if (i != lastI && IsNewLine(input, i))
                {
                    lastI = i;
                    line++;
                    column = 0;
                }
                else if (i != lastI)
                {
                    column += i - lastI;
                    lastI = i;
                }

                if (!captured && !captureString && StartsWith(input.AsSpan(i), "//"))
                {
                    lineComment = true;
                    captureComment = true;
                    i += 2;
                    captureStart = i;
                    continue;
                }

                if (!captured && !captureString && captureComment && lineComment && IsNewLineOrEof(input, i, out var nlLen))
                {
                    lineComment = false;
                    captured = true;
                    captureEnd = nlLen == 0 ? input.Length : i;
                    i += nlLen;
                }

                if (!captured && !captureString && !captureComment && StartsWith(input.AsSpan(i), "/*"))
                {
                    captureComment = true;
                    i += 2;
                    captureStart = i;
                    continue;
                }

                if (!captured && !captureString && captureComment && StartsWith(input.AsSpan(i), "*/"))
                {
                    captured = true;
                    captureEnd = i;
                    i += 2;
                }

                if (!captured && !captureComment && !captureString && IsWhiteSpaceOrNewLine(input, i, out var hiddenSepLen, out var isNewLine))
                {
                    captured = true;
                    captureEnd = i;
                    i += hiddenSepLen;
                }

                if (!captured && !captureComment && !captureString && input[i] == '"' && last != '\\')
                {
                    captureString = true;
                    i++;
                    captureStart = i;
                    continue;
                }

                if (!captured && !captureComment && captureString && input[i] == '"' && last != '\\')
                {
                    captured = true;
                    captureEnd = i;
                    i++;
                }

                if (!captured && !captureComment && !captureString && input[i] == '\'' && last != '\\')
                {
                    captured = true;
                    captureEnd = i;
                    if (i + 2 < input.Length)
                    {
                        ReadOnlySpan<char> e = input.AsSpan(i);
                        ReadOnlySpan<char> e2 = input.AsSpan(i - 1);
                        i++;
                        column++;
                        if (input[i] == '\\' && i + 2 >= input.Length)
                        {
                            diagnostics.Error("Unexpected end of file.", new(filename, captureStart, line, column));
                            return new(diagnostics, null);
                        }

                        if (input[i] == '\\')
                        {
                            i++;
                        }

                        tokens.Add(new(i, 1, input, new(filename, i, line, column), LiteralType.Char));
                        i++;

                        if (input[i] != '\'')
                        {
                            diagnostics.Error("Invalid token.", new(filename, captureStart, line, column));
                            return new(diagnostics, null);
                        }
                        i++;
                    }
                    else
                    {
                        diagnostics.Error("Unexpected end of file.", new(filename, captureStart, line, column));
                        return new(diagnostics, null);
                    }
                }

                if (!captured && !captureComment && !captureString && IsOperator(input, i, out var opLen))
                {
                    tokens.Add(new(TokenType.Operator, i, opLen, input, new(filename, i, line, column)));
                    insertBefore = true;
                    captured = true;
                    captureEnd = i;
                    i += opLen;
                }

                if (!captured && !captureComment && !captureString && IsPunctuation(input, i, out var sepLen))
                {
                    tokens.Add(new(TokenType.Punctuation, i, sepLen, input, new(filename, i, line, column)));
                    insertBefore = true;
                    captured = true;
                    captureEnd = i;
                    i += sepLen;
                }

                if (captured || captureStart == 0 && i + 1 == input.Length)
                {
                    if (captureStart == 0 && i + 1 == input.Length)
                    {
                        captureEnd = i;
                    }
                    if (!captureComment)
                        Trim(input, ref captureStart, ref captureEnd);
                    ReadOnlySpan<char> span = input.AsSpan(captureStart, captureEnd - captureStart);

                    int tokenIndex = Math.Max(tokens.Count - (insertBefore ? 1 : 0), 0);
                    insertBefore = false;

                    if (span.IsEmpty || span.IsWhiteSpace())
                    {
                        captureStart = i;
                        captured = false;
                        continue;
                    }

                    if (captureComment)
                    {
                        tokens.Insert(tokenIndex, new(TokenType.Comment, captureStart, span.Length, input, new(filename, captureStart, line, column)));
                        captureComment = false;
                    }
                    else if (captureString)
                    {
                        tokens.Insert(tokenIndex, new(captureStart, span.Length, input, new(filename, captureStart, line, column), LiteralType.String));
                        captureString = false;
                    }
                    else if (IsKeyword(span, out var keywordType))
                    {
                        tokens.Insert(tokenIndex, new(captureStart, span.Length, input, new(filename, captureStart, line, column), keywordType));
                    }
                    else if (IsNumber(span, out var numberType))
                    {
                        tokens.Insert(tokenIndex, new(captureStart, span.Length, input, new(filename, captureStart, line, column), numberType));
                    }
                    else
                    {
                        tokens.Insert(tokenIndex, new(TokenType.Identifier, captureStart, span.Length, input, new(filename, captureStart, line, column)));
                    }

                    captureStart = i;
                    captured = false;
                    continue;
                }

                i++;
            }

            if (captureComment)
            {
                diagnostics.Error("Unexpected end of file.", new(filename, captureStart, line, column));
                return new(diagnostics, null);
            }

            if (captureString)
            {
                diagnostics.Error("Unexpected end of file.", new(filename, captureStart, line, column));
                return new(diagnostics, null);
            }

            return new(diagnostics, tokens);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Trim(ReadOnlySpan<char> input, ref int start, ref int length)
        {
            bool wasChar = false;
            for (int i = start; i < length; i++)
            {
                var c = input[i];
                if (c == ' ' && !wasChar)
                {
                    start++;
                }
                else
                {
                    wasChar = true;
                }
            }
            wasChar = false;
            for (int i = length - 1; i >= start; i--)
            {
                var c = input[i];
                if (c == ' ' && !wasChar)
                {
                    length--;
                }
                else
                {
                    wasChar = true;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsWhiteSpaceOrNewLine(ReadOnlySpan<char> input, int index, out int length, out bool isNewLine)
        {
            length = 0;

            if (input[index] == ' ')
            {
                length = 1;
                isNewLine = false;
                return true;
            }

            if (input[index] == '\n')
            {
                length = 1;
                isNewLine = true;
                return true;
            }

            if (input[index] == '\t')
            {
                length = 1;
                isNewLine = false;
                return true;
            }

            if (StartsWith(input[index..], "\r\n"))
            {
                length = 2;
                isNewLine = true;
                return true;
            }

            isNewLine = false;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNewLine(ReadOnlySpan<char> input, int index)
        {
            if (input[index] == '\n')
            {
                return true;
            }

            if (StartsWith(input[index..], "\r\n"))
            {
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNewLineOrEof(ReadOnlySpan<char> input, int index, out int length)
        {
            length = 0;

            if (input[index] == '\n')
            {
                length = 1;
                return true;
            }

            if (StartsWith(input[index..], "\r\n"))
            {
                length = 2;
                return true;
            }

            if (index == input.Length - 1)
            {
                length = 0;
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPunctuation(ReadOnlySpan<char> input, int index, out int length)
        {
            length = 0;

            for (int i = 0; i < punctuations.Count; i++)
            {
                var separator = punctuations[i];
                if (input[index] == separator)
                {
                    length = 1;
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsKeyword(ReadOnlySpan<char> input, int index, out int length)
        {
            length = 0;
            var span = input[index..];

            for (int i = 0; i < keywords.Count; i++)
            {
                var keyword = keywords[i];
                if (StartsWith(span, keyword))
                {
                    length = keyword.Length;
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsKeyword(ReadOnlySpan<char> span, out KeywordType keywordType)
        {
            keywordType = KeywordType.Unknown;
            for (int i = 0; i < keywords.Count; i++)
            {
                var keyword = keywords[i];

                if (span.SequenceEqual(keyword))
                {
                    keywordType = keywordMap[keyword];
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsOperator(ReadOnlySpan<char> input, int index, out int length)
        {
            length = 0;
            var span = input[index..];

            for (int i = 0; i < numberNotations.Count; i++)
            {
                var op = numberNotations[i];
                if (StartsWithIgnoreCase(span, op))
                {
                    return false;
                }
                if (index > 0)
                {
                    if (StartsWith(input[(index - 1)..], op))
                    {
                        return false;
                    }
                }
            }

            for (int i = 0; i < operators.Count; i++)
            {
                var op = operators[i];
                if (StartsWith(span, op))
                {
                    length = op.Length;
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNumber(ReadOnlySpan<char> input, out NumberType type, NumberParseOptions options = NumberParseOptions.AllowAll)
        {
            type = NumberType.None;
            if (input.Length == 0)
                return false;

            type = NumberType.Int;

            bool isHex = false;

            if ((options & NumberParseOptions.AllowPositive) != 0 && input.StartsWith("+", StringComparison.InvariantCultureIgnoreCase))
            {
                input = input[1..];
            }
            else if (StartsWith(input, "+"))
            {
                return false;
            }

            if ((options & NumberParseOptions.AllowNegative) != 0 && input.StartsWith("-", StringComparison.InvariantCultureIgnoreCase))
            {
                input = input[1..];
            }
            else if (StartsWith(input, "-"))
            {
                return false;
            }

            if ((options & NumberParseOptions.AllowHex) != 0 && input.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                input = input[2..];
                isHex = true;
            }
            else if (StartsWith(input, "0x"))
            {
                return false;
            }

            if ((options & NumberParseOptions.AllowBinary) != 0 && input.StartsWith("0b", StringComparison.InvariantCultureIgnoreCase))
            {
                input = input[2..];
            }
            else if (StartsWith(input, "0b"))
            {
                return false;
            }

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (!char.IsNumber(c))
                {
                    if (c == '.')
                    {
                        type = NumberType.Double;
                    }
                    else if (!isHex || !char.IsAsciiHexDigit(c))
                    {
                        input = input[i..];
                        break;
                    }
                }
                if (i == input.Length - 1)
                {
                    return true;
                }
            }

            if ((options & NumberParseOptions.AllowExponent) != 0 && input.StartsWith("e+") || input.StartsWith("e-"))
            {
                input = input[2..];
                for (int i = 0; i < input.Length; i++)
                {
                    if (!char.IsDigit(input[i]))
                    {
                        input = input[i..];
                        break;
                    }
                    if (i == input.Length - 1)
                    {
                        return true;
                    }
                }
            }

            if ((options & NumberParseOptions.AllowSuffix) != 0 && input.EndsWith("L", StringComparison.InvariantCultureIgnoreCase))
            {
                type = NumberType.Long;
                input = input[1..];
            }

            if ((options & NumberParseOptions.AllowSuffix) != 0 && input.EndsWith("U", StringComparison.InvariantCultureIgnoreCase))
            {
                type = NumberType.UInt;
                input = input[1..];
            }

            if ((options & NumberParseOptions.AllowSuffix) != 0 && input.EndsWith("UL", StringComparison.InvariantCultureIgnoreCase))
            {
                type = NumberType.ULong;
                input = input[2..];
            }

            if ((options & NumberParseOptions.AllowSuffix) != 0 && input.EndsWith("F", StringComparison.InvariantCultureIgnoreCase))
            {
                type = NumberType.Float;
                input = input[1..];
            }

            if ((options & NumberParseOptions.AllowSuffix) != 0 && input.EndsWith("D", StringComparison.InvariantCultureIgnoreCase))
            {
                type = NumberType.Double;
                input = input[1..];
            }

            if ((options & NumberParseOptions.AllowSuffix) != 0 && input.EndsWith("M", StringComparison.InvariantCultureIgnoreCase))
            {
                type = NumberType.Decimal;
                input = input[1..];
            }

            return input.Length == 0;
        }
    }
}