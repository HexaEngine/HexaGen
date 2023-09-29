namespace HexaGen.Language.CSharp.Nodes
{
    using System.Collections.Generic;

    public class NamespaceNode : SyntaxNode
    {
        public NamespaceNode(string @namespace)
        {
            Namespace = @namespace;
        }

        public NamespaceNode(string @namespace, List<SyntaxNode> children) : base(children)
        {
            Namespace = @namespace;
        }

        public string Namespace { get; }

        public override string ToString()
        {
            return $"namespace: {Namespace}";
        }
    }
}