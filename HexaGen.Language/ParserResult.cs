namespace HexaGen.Language
{
    public class ParserResult
    {
        private readonly SyntaxTree? tree;
        private readonly DiagnosticBag diagnostics;

        public ParserResult(SyntaxTree? tree, DiagnosticBag diagnostics)
        {
            this.tree = tree;
            this.diagnostics = diagnostics;
        }

        public SyntaxTree? SyntaxTree => tree;

        public DiagnosticBag Diagnostics => diagnostics;
    }
}