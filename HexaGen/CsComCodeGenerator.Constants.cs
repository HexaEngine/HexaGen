namespace HexaGen
{
    using CppAst;
    using System.Collections.Generic;
    using System.Linq;

    public partial class CsComCodeGenerator
    {
        private readonly HashSet<string> LibDefinedConstants = new();

        public readonly HashSet<string> DefinedConstants = new();
        private readonly Dictionary<string, CppMacro> DefinedCppConstants = new();

        private void GenerateConstants(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Constants.cs");
            string[] usings = { "System", "HexaGen.Runtime", "HexaGen.Runtime.COM" };

            using CodeWriter writer = new(filePath, settings.Namespace, usings.Concat(settings.Usings).ToArray());

            using (writer.PushBlock($"public unsafe partial class {settings.ApiName}"))
            {
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
                        writer.WriteLine($"[NativeName(NativeNameType.Const, \"{macro.Name}\")]");
                        writer.WriteLine($"public const uint {name} = {value};");
                        writer.WriteLine();
                    }
                    else if (value.IsString())
                    {
                        writer.WriteLine($"[NativeName(NativeNameType.Const, \"{macro.Name}\")]");
                        writer.WriteLine($"public const string {name} = {value};");
                        writer.WriteLine();
                    }
                }
            }
        }
    }
}