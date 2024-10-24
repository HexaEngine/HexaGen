namespace HexaGen.GenerationSteps
{
    using CppAst;
    using HexaGen.Core;
    using HexaGen.Metadata;
    using System.Collections.Generic;

    public class ConstantGenerationStep : GenerationStep
    {
        protected readonly HashSet<CsConstantMetadata> LibDefinedConstants = new(IdentifierComparer<CsConstantMetadata>.Default);
        public readonly HashSet<CsConstantMetadata> DefinedConstants = new(IdentifierComparer<CsConstantMetadata>.Default);
        protected readonly Dictionary<string, CsConstantMetadata> DefinedCppConstants = [];

        public ConstantGenerationStep(CsCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
        }

        public override string Name { get; } = "Constants";

        public override void Configure(CsCodeGeneratorConfig config)
        {
            Enabled = config.GenerateConstants;
        }

        public override void CopyToMetadata(CsCodeGeneratorMetadata metadata)
        {
            metadata.DefinedConstants.AddRange(DefinedConstants);
        }

        public override void CopyFromMetadata(CsCodeGeneratorMetadata metadata)
        {
            LibDefinedConstants.AddRange(metadata.DefinedConstants);
        }

        public override void Reset()
        {
            LibDefinedConstants.Clear();
            DefinedConstants.Clear();
            DefinedCppConstants.Clear();
        }

        protected virtual List<string> SetupConstantUsings()
        {
            List<string> usings = ["System", "HexaGen.Runtime", .. config.Usings];
            return usings;
        }

        protected virtual bool FilterConstant(GenContext context, CsConstantMetadata metadata)
        {
            if (config.AllowedConstants.Count != 0 && !config.AllowedConstants.Contains(metadata.Identifier))
                return true;

            if (config.IgnoredConstants.Contains(metadata.Identifier))
                return true;

            if (LibDefinedConstants.Contains(metadata))
                return true;

            if (DefinedConstants.Contains(metadata))
            {
                var o = DefinedCppConstants[metadata.Identifier];
                if (o.CppName == metadata.CppName)
                {
                    if (o.CppValue == metadata.CppValue)
                    {
                        return true;
                    }
                }
                LogWarn($"{context.FilePath}: constant {metadata} is already defined!");
                return true;
            }

            DefinedCppConstants.Add(metadata.Identifier, metadata);
            DefinedConstants.Add(metadata);
            return false;
        }

        public override void Generate(CppCompilation compilation, string outputPath, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata)
        {
            string folder = Path.Combine(outputPath, "Constants");
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, "Constants.cs");

            using CsSplitCodeWriter writer = new(filePath, config.Namespace, SetupConstantUsings(), config.HeaderInjector);
            GenContext context = new(compilation, filePath, writer);
            using (writer.PushBlock($"public unsafe partial class {config.ApiName}"))
            {
                for (int i = 0; i < compilation.Macros.Count; i++)
                {
                    WriteConstant(context, ParseConstant(compilation.Macros[i]));
                }
            }
        }

        protected virtual CsConstantMetadata ParseConstant(CppMacro macro)
        {
            macro.UpdateValueFromTokens();
            var name = config.GetConstantName(macro.Name);
            var value = macro.Value.NormalizeConstantValue();

            return new(macro.Name, macro.Value, name, value, null);
        }

        protected virtual void WriteConstant(GenContext context, CsConstantMetadata csConstant)
        {
            if (FilterConstant(context, csConstant))
                return;

            var writer = context.Writer;
            var name = csConstant.Name;
            var value = csConstant.Value;

            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            if (value.IsNumeric(out var type))
            {
                if (config.GenerateMetadata)
                {
                    writer.WriteLine($"[NativeName(NativeNameType.Const, \"{csConstant.CppName}\")]");
                    writer.WriteLine($"[NativeName(NativeNameType.Value, \"{csConstant.EscapedCppValue}\")]");
                }

                writer.WriteLine($"public const {type.GetNumberType()} {name} = {value};");
                writer.WriteLine();
            }
            else if (value.IsString())
            {
                if (config.GenerateMetadata)
                {
                    writer.WriteLine($"[NativeName(NativeNameType.Const, \"{csConstant.CppName}\")]");
                    writer.WriteLine($"[NativeName(NativeNameType.Value, \"{csConstant.EscapedCppValue}\")]");
                }

                writer.WriteLine($"public const string {name} = {value};");
                writer.WriteLine();
            }
            else if (!string.IsNullOrWhiteSpace(value))
            {
                //int start = 0;
                bool capture = false;
                for (int i = 0; i < value.Length; i++)
                {
                    var c = value[i];
                    if (c == '(')
                    {
                        if (capture) // not supported early exit.
                        {
                            return;
                        }
                        capture = true;
                    }
                }
                //var result = CppMacroParser.Default.Parse(value, "");
            }
        }
    }
}