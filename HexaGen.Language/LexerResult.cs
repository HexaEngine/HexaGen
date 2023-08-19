namespace HexaGen.Language
{
    public class LexerResult
    {
        private readonly DiagnosticBag diagnostics;
        private readonly List<Token> tokens;

        public LexerResult(DiagnosticBag diagnostics, List<Token> tokens)
        {
            this.diagnostics = diagnostics;
            this.tokens = tokens;
        }

        public List<Token> Tokens => tokens;

        public DiagnosticBag Diagnostics => diagnostics;
    }
}