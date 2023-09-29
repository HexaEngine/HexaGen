namespace HexaGen.Language.CSharp.Nodes
{
    public class ClassNode : SyntaxNode
    {
        public ClassNode(string name, KeywordType[] modifiers)
        {
            Name = name;
            Modifiers = modifiers;
        }

        public ClassNode(string name, KeywordType[] modifiers, List<SyntaxNode> children) : base(children)
        {
            Name = name;
            Modifiers = modifiers;
        }

        public string Name { get; }

        public KeywordType[] Modifiers { get; }

        public override string ToString()
        {
            return $"class: {string.Join(" ", Modifiers)} {Name}";
        }
    }
}