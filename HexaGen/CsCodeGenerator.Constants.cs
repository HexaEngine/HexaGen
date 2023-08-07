namespace HexaGen
{
    using CppAst;
    using System.Collections.Generic;

    public partial class CsCodeGenerator
    {
        private readonly HashSet<string> LibDefinedConstants = new();

        public readonly HashSet<string> DefinedConstants = new();
        private readonly Dictionary<string, CppMacro> DefinedCppConstants = new();

        private void GenerateConstants(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Constants.cs");
            CodeWriter writer = new(filePath, "System");
            for (int i = 0; i < compilation.Macros.Count; i++)
            {
                var macro = compilation.Macros[i];

                if (settings.AllowedConstants.Count != 0 && !settings.AllowedConstants.Contains(macro.Name))
                    continue;

                if (settings.IgnoredConstants.Contains(macro.Name))
                    continue;

                if (LibDefinedConstants.Contains(macro.Name))
                    continue;

                if (DefinedConstants.Contains(macro.Name))
                {
                    var o = DefinedCppConstants[macro.Name];
                    if (o.Name == macro.Name)
                    {
                        if (o.Value == macro.Value)
                        {
                            continue;
                        }
                    }
                    LogWarn($"{filePath}: constant {macro} is already defined!");
                    continue;
                }

                DefinedCppConstants.Add(macro.Name, macro);
                DefinedConstants.Add(macro.Name);

                var name = settings.GetPrettyConstantName(macro.Name);
                var value = macro.Value.NormalizeConstantValue();

                if (value == string.Empty)
                    continue;
                if (value.IsNumeric(true))
                {
                    writer.WriteLine($"[NativeName(\"{macro.Name}\")]");
                    writer.WriteLine($"public const uint {name} = {value};");
                    writer.WriteLine();
                }
                else if (value.IsString())
                {
                    writer.WriteLine($"[NativeName(\"{macro.Name}\")]");
                    writer.WriteLine($"public const string {name} = {value};");
                    writer.WriteLine();
                }
            }
        }
    }
}