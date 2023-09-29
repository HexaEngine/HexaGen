namespace HexaGen.Language
{
    public class Preprocessor
    {
        protected readonly List<string> operators = new();
        protected readonly List<char> punctuations = new();
        protected readonly List<string> keywords;
        protected readonly Dictionary<string, KeywordType> keywordMap = new();
        protected readonly List<string> numberNotations = new();

        public Preprocessor()
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
        }

        public string Process(string text, string filename)
        {
            List<Token> tokens = new();
            int line = 0;
            int col = 0;
            int captureStart = 0;
            int captureEnd = 0;
            for (int i = 0; i < text.Length;)
            {
                if (IsNewLineOrEof(text, i, out int len))
                {
                    i += len;
                    col = 0;
                    line++;
                    continue;
                }

                if (text[i] != '#')
                {
                    while (!IsNewLineOrEof(text, i, out _))
                    {
                        i++;
                    }
                    continue;
                }

                if (text[i] == '#')
                {
                    captureStart = i;
                    while (!IsNewLineOrEof(text, i, out _))
                    {
                        i++;
                    }
                    captureEnd = i;
                    TokenizeRun(text, captureStart, captureEnd, tokens, line, col);
                }
            }
            return text;
        }

        private void TokenizeRun(string input, int start, int end, List<Token> tokens, int line, int col)
        {
            var span = input.AsSpan(start, end - start);

            int directiveEnd = end;
            for (int i = 0; i < span.Length; i++)
            {
                char c = span[i];
                if (char.IsWhiteSpace(c))
                {
                    directiveEnd = i;
                }
            }
        }

        public static bool IsNewLineOrEof(ReadOnlySpan<char> input, int index, out int length)
        {
            length = 0;

            if (input[index] == '\n')
            {
                length = 1;
                return true;
            }

            if (input[index..].StartsWith("\r\n"))
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
    }
}