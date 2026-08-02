namespace HexaGen.Cpp2C
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public struct IncludeBuilder
    {
        private StringBuilder sb = new();

        public IncludeBuilder()
        {
        }

        public static IncludeBuilder Create()
        {
            return new IncludeBuilder();
        }

        [UnscopedRef]
        public ref IncludeBuilder AddSystemInclude(string include)
        {
            sb.AppendLine($"#include <{include}>");
            return ref this;
        }

        [UnscopedRef]
        public ref IncludeBuilder AddInclude(string include)
        {
            sb.AppendLine($"#include \"{include}\"");
            return ref this;
        }

        [UnscopedRef]
        public ref IncludeBuilder AddIncludes(IEnumerable<string> includes)
        {
            foreach (string s in includes)
            {
                AddInclude(s);
            }
            return ref this;
        }

        public readonly string Build()
        {
            sb.AppendLine();
            var result = sb.ToString();
            sb.Clear();
            return result;
        }
    }
}