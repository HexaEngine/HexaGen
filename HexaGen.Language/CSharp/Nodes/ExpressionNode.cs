namespace HexaGen.Language.CSharp.Nodes
{
    public class ExpressionNode : SyntaxNode
    {
        public override string ToString()
        {
            if (Children.Count == 0) return "expression: <Empty>";
            return "";
        }
    }
}