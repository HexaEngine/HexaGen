namespace HexaGen.Language.CSharp.Nodes
{
    public class MethodNode : SyntaxNode
    {
        public MethodNode(string name, KeywordType[] modifiers, string[] parameters, string returnType)
        {
            Name = name;
            Modifiers = modifiers;
            Parameters = parameters;
            ReturnType = returnType;
        }

        public string Name { get; }

        public KeywordType[] Modifiers { get; }

        public string[] Parameters { get; }

        public string ReturnType { get; }

        public override string ToString()
        {
            return $"method: {string.Join(" ", Modifiers)} {ReturnType} {Name} ({string.Join(" ", Parameters)})";
        }
    }
}