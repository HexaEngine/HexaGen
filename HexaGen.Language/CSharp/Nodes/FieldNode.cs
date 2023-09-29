namespace HexaGen.Language.CSharp.Nodes
{
    using HexaGen.Language;

    public class FieldNode : SyntaxNode
    {
        public FieldNode(string type, string name, KeywordType[] modifiers, string? expression)
        {
            Type = type;
            Name = name;
            Modifiers = modifiers;
            Expression = expression;
        }

        public string Type { get; set; }

        public string Name { get; set; }

        public KeywordType[] Modifiers { get; }

        public string? Expression { get; set; }

        public override string ToString()
        {
            return $"field: {string.Join(" ", Modifiers)} {Type} {Name} = {Expression}";
        }
    }
}