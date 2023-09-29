namespace HexaGen
{
    using CppAst;
    using HexaGen.Language.Cpp;
    using System.Collections.Generic;

    public unsafe partial class CsCodeGenerator
    {
        private CppMacroParser parser = new();

        protected readonly HashSet<string> LibDefinedConstants = new();

        public readonly HashSet<string> DefinedConstants = new();
        protected readonly Dictionary<string, CppMacro> DefinedCppConstants = new();

        protected virtual List<string> SetupConstantUsings()
        {
            List<string> usings = new() { "System", "HexaGen.Runtime" };
            usings.AddRange(settings.Usings);
            return usings;
        }

        protected virtual bool FilterConstant(GenContext context, CppMacro macro)
        {
            if (settings.AllowedConstants.Count != 0 && !settings.AllowedConstants.Contains(macro.Name))
                return true;

            if (settings.IgnoredConstants.Contains(macro.Name))
                return true;

            if (LibDefinedConstants.Contains(macro.Name))
                return true;

            if (DefinedConstants.Contains(macro.Name))
            {
                var o = DefinedCppConstants[macro.Name];
                if (o.Name == macro.Name)
                {
                    if (o.Value == macro.Value)
                    {
                        return true;
                    }
                }
                LogWarn($"{context.FilePath}: constant {macro} is already defined!");
                return true;
            }

            DefinedCppConstants.Add(macro.Name, macro);
            DefinedConstants.Add(macro.Name);
            return false;
        }

        protected virtual void GenerateConstants(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Constants.cs");

            using CodeWriter writer = new(filePath, settings.Namespace, SetupConstantUsings());
            GenContext context = new(compilation, filePath, writer);
            using (writer.PushBlock($"public unsafe partial class {settings.ApiName}"))
            {
                for (int i = 0; i < compilation.Macros.Count; i++)
                {
                    WriteConstant(context, compilation.Macros[i]);
                }
            }
        }

        protected virtual void WriteConstant(GenContext context, CppMacro macro)
        {
            if (FilterConstant(context, macro))
                return;

            var writer = context.Writer;
            var name = settings.GetConstantName(macro.Name);
            var value = macro.Value.NormalizeConstantValue();

            if (value == string.Empty)
                return;

            if (value.IsNumeric(out var type))
            {
                writer.WriteLine($"[NativeName(NativeNameType.Const, \"{macro.Name}\")]");
                writer.WriteLine($"public const {type.GetNumberType()} {name} = {value};");
                writer.WriteLine();
            }
            else if (value.IsString())
            {
                writer.WriteLine($"[NativeName(NativeNameType.Const, \"{macro.Name}\")]");
                writer.WriteLine($"public const string {name} = {value};");
                writer.WriteLine();
            }
            else if (macro.Parameters == null)
            {
                //var result = parser.Parse(value, "");
                //writer.WriteLine($"[NativeName(NativeNameType.NoneOrConst, \"{macro.Name}\")]");
                //writer.WriteLine($"public const string {name} = {value};");
                //writer.WriteLine();
            }
        }
    }
}