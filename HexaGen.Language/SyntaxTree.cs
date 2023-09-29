namespace HexaGen.Language
{
    using System.Text;

    public class SyntaxTree
    {
        private readonly RootNode root;

        public SyntaxTree(RootNode root)
        {
            this.root = root;
        }

        public IReadOnlyList<SyntaxNode> Nodes => root.Children;

        public string BuildDebugTree()
        {
            StringBuilder sb = new();
            sb.AppendLine(root.ToString());
            var level = 1;

            for (int i = 0; i < root.Children.Count; i++)
            {
                root.Children[i].BuildDebugTree(sb, ref level);
            }

            return sb.ToString();
        }
    }
}