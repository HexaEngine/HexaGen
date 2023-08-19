namespace HexaGen.Language
{
    using System.Text;

    public abstract class SyntaxNode
    {
        protected List<SyntaxNode> children = new();

        public SyntaxNode()
        {
        }

        public SyntaxNode(List<SyntaxNode> children)
        {
            this.children = children;
        }

        public IReadOnlyList<SyntaxNode> Children => children;

        public void AddChild(SyntaxNode node)
        {
            children.Add(node);
        }

        public SyntaxNode GetChild(int index)
        {
            return children[index];
        }

        public void RemoveChild(SyntaxNode node)
        {
            children.Remove(node);
        }

        public void RemoveChildAt(int index)
        {
            children.RemoveAt(index);
        }

        public void Contains(SyntaxNode node)
        {
            children.Contains(node);
        }

        private const string intendString = "\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t";

        public void BuildDebugTree(StringBuilder sb, ref int level)
        {
            sb.Append(intendString.AsSpan(0, level));
            sb.AppendLine(ToString());
            level++;
            for (int i = 0; i < children.Count; i++)
            {
                children[i].BuildDebugTree(sb, ref level);
            }
            level--;
        }
    }
}