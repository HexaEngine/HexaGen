namespace HexaGen.Language
{
    using System.Collections.Generic;

    public class RootNode : SyntaxNode
    {
        public RootNode()
        {
        }

        public RootNode(List<SyntaxNode> children) : base(children)
        {
        }

        public override string ToString()
        {
            return $"root";
        }
    }
}