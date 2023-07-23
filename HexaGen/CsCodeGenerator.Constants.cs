namespace HexaGen
{
    using CppAst;
    using System.Collections.Generic;

    public partial class CsCodeGenerator
    {
        private readonly HashSet<string> LibDefinedConstants = new();

        public readonly HashSet<string> DefinedConstants = new();

        private void GenerateConstants(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Constants.cs");
            CodeWriter writer = new(filePath, "System");
            for (int i = 0; i < compilation.Macros.Count; i++)
            {
                var macro = compilation.Macros[i];

                if (LibDefinedConstants.Contains(macro.Name))
                    continue;

                if (DefinedConstants.Contains(macro.Name))
                {
                    LogWarn($"{filePath}: constant {macro} is already defined!");
                    continue;
                }

                var name = settings.GetPrettyConstantName(macro.Name);
                var value = macro.Value.NormalizeConstantValue();

                if (value == string.Empty)
                    continue;
                writer.WriteLine();
            }
        }
    }
}