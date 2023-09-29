namespace HexaGen.Language
{
    public class ParserBase
    {
        protected readonly List<ISyntaxAnalyzer> analyzers = new();
        protected readonly Lexer lexer = new();
        protected readonly ParserOptions options;

        public ParserBase(ParserOptions options)
        {
            this.options = options;
        }

        public virtual void AddAnalyser(ISyntaxAnalyzer analyzer)
        {
            analyzers.Add(analyzer);
        }

        public virtual ParserResult Parse(string input, string filename)
        {
            var lexerResult = lexer.Tokenize(input, filename);

            var diagnostics = lexerResult.Diagnostics;

            if (diagnostics.HasErrors)
                return new ParserResult(null, diagnostics);

            RootNode root = new();
            ParserContext context = new(root, options, analyzers, lexerResult.Tokens, diagnostics);

            while (!context.IsEnd)
            {
                if (context.AnalyzeCurrent() != AnalyserResult.Success)
                {
                    return new ParserResult(null, diagnostics);
                }
            }

            if (context.ScopeStack.Count != 0)
            {
                for (int i = 0; i < context.ScopeStack.Count; i++)
                    diagnostics.Error("Syntax Error: } expected");
                return new ParserResult(null, diagnostics);
            }

            return new ParserResult(new SyntaxTree(root), diagnostics);
        }
    }
}