namespace HexaGen.GenerationSteps
{
    using CppAst;
    using HexaGen.Core;
    using HexaGen.Metadata;
    using System;
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using System.Reflection.Metadata;

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

        public override void Generate(FileSet files, ParseResult result, string outputPath, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata)
        {
            var compilation = result.Compilation;
            string folder = Path.Combine(outputPath, "Constants");
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, "Constants.cs");

            using CsSplitCodeWriter writer = new(filePath, config.Namespace, SetupConstantUsings(), config.HeaderInjector);
            GenContext context = new(result, filePath, writer);
            List<CsConstantMetadata> constants = [];

            for (int i = 0; i < compilation.Macros.Count; i++)
            {
                if (!files.Contains(compilation.Macros[i].SourceFile))
                    continue;

                var constant = ParseConstant(compilation.Macros[i]);

                constants.Add(constant);
            }

            Dictionary<string, CsConstantMetadata> constantsLookupTable = [];
            foreach (var constant in constants)
            {
                constantsLookupTable.TryAdd(constant.CppName, constant);
            }

            var frozenTable = constantsLookupTable.ToFrozenDictionary();

            foreach (var constant in constants)
            {
                if (constant.Type == CsConstantType.Unknown)
                {
                    if (constant.Value != null && frozenTable.ContainsKey(constant.Value))
                    {
                        constant.Type = CsConstantType.Reference;
                    }
                }
            }

            using (writer.PushBlock($"public unsafe partial class {config.ApiName}"))
            {
                foreach (var constant in constants)
                {
                    WriteConstant(context, constant, frozenTable);
                }
            }
        }

        protected virtual CsConstantMetadata ParseConstant(CppMacro macro)
        {
            macro.UpdateValueFromTokens();
            var name = config.GetConstantName(macro.Name);
            var value = macro.Value.NormalizeConstantValue();

            CsConstantType constantType = CsConstantType.Unknown;
            if (value.IsNumeric(out NumberType type))
            {
                constantType = type.GetConstantType();
            }
            else if (value.IsString())
            {
                constantType = CsConstantType.String;
            }

            return new(macro.Name, macro.Value, name, value, constantType, null);
        }

        protected virtual void WriteConstant(GenContext context, CsConstantMetadata csConstant, FrozenDictionary<string, CsConstantMetadata> constantsLookupTable)
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

            if (csConstant.Type == CsConstantType.Unknown)
            {
                return;
            }

            switch (csConstant.Type)
            {
                case CsConstantType.Reference:
                    string? resolveBaseType = ResolveBaseType(value, constantsLookupTable);
                    if (resolveBaseType == null) return;
                    WriteMetadata(csConstant, writer);
                    writer.WriteLine($"public const {resolveBaseType} {name} = {value};");
                    break;

                case CsConstantType.Custom:
                    WriteMetadata(csConstant, writer);
                    writer.WriteLine($"public const {csConstant.CustomType} {name} = {value};");
                    break;

                default:
                    WriteMetadata(csConstant, writer);
                    writer.WriteLine($"public const {csConstant.Type.GetCSharpType()} {name} = {value};");
                    break;
            }

            writer.WriteLine();

            void WriteMetadata(CsConstantMetadata csConstant, ICodeWriter writer)
            {
                if (config.GenerateMetadata)
                {
                    writer.WriteLine($"[NativeName(NativeNameType.Const, \"{csConstant.CppName}\")]");
                    writer.WriteLine($"[NativeName(NativeNameType.Value, \"{csConstant.EscapedCppValue}\")]");
                }
            }
        }

        private static string? ResolveBaseType(string value, FrozenDictionary<string, CsConstantMetadata> constantsLookupTable)
        {
            var metadata = constantsLookupTable[value];
            if (metadata.Type == CsConstantType.Unknown) return null;
            if (metadata.Type == CsConstantType.Reference) return metadata.Value == null ? null : ResolveBaseType(metadata.Value, constantsLookupTable);
            if (metadata.Type == CsConstantType.Custom) return metadata.CustomType;
            return metadata.Type.GetCSharpType();
        }
    }
}