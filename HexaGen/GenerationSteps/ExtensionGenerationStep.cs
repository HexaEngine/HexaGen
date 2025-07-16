namespace HexaGen.GenerationSteps
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using HexaGen.FunctionGeneration;
    using HexaGen.FunctionGeneration.ParameterWriters;
    using HexaGen.Metadata;
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using System.Text;

    public class ExtensionGenerationStep : GenerationStep
    {
        protected readonly HashSet<string> LibDefinedExtensionTypes = new();
        protected readonly HashSet<string> DefinedExtensionTypes = new();
        protected HashSet<CsFunctionVariation> DefinedVariationsFunctions;

        public readonly List<CsFunction> DefinedExtensions = [];

        protected FunctionGenerator funcGen = null!;

        public ExtensionGenerationStep(CsCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
            DefinedVariationsFunctions = null!;
        }

        public override string Name { get; } = "Extensions";

        public override void Configure(CsCodeGeneratorConfig config)
        {
            Enabled = config.GenerateExtensions;
            funcGen = generator.FunctionGenerator;
        }

        public override void CopyToMetadata(CsCodeGeneratorMetadata metadata)
        {
            metadata.DefinedExtensions.AddRange(DefinedExtensions);
            metadata.DefinedExtensionTypes.AddRange(DefinedExtensionTypes);
        }

        public override void CopyFromMetadata(CsCodeGeneratorMetadata metadata)
        {
            LibDefinedExtensionTypes.AddRange(metadata.DefinedExtensionTypes);
        }

        public override void Reset()
        {
            LibDefinedExtensionTypes.Clear();
            DefinedExtensionTypes.Clear();
            DefinedExtensions.Clear();
        }

        protected virtual List<string> SetupExtensionUsings()
        {
            List<string> usings = ["System", "System.Runtime.CompilerServices", "System.Runtime.InteropServices", "HexaGen.Runtime", .. config.Usings];
            return usings;
        }

        protected virtual bool FilterExtensionType(GenContext context, CppTypedef typedef)
        {
            if (config.IgnoredTypedefs.Contains(typedef.Name))
            {
                return true;
            }

            if (LibDefinedExtensionTypes.Contains(typedef.Name))
            {
                return true;
            }

            if (DefinedExtensionTypes.Contains(typedef.Name))
            {
                return true;
            }

            if (typedef.ElementType is not CppPointerType)
            {
                return true;
            }

            if (typedef.IsDelegate())
            {
                return true;
            }

            DefinedExtensionTypes.Add(typedef.Name);

            return false;
        }

        protected virtual bool FilterExtensionFunction(GenContext context, CppFunction cppFunction, CppTypedef typedef, bool isCustomHandle)
        {
            if (!cppFunction.IsPublicExport())
            {
                return true;
            }

#if CPPAST_15_OR_GREATER
            if (cppFunction.IsFunctionTemplate)
            {
                return true;
            }
#endif

            if (cppFunction.Flags == CppFunctionFlags.Inline)
            {
                return true;
            }

            if (config.AllowedFunctions.Count != 0 && !config.AllowedFunctions.Contains(cppFunction.Name))
            {
                return true;
            }

            if (config.AllowedExtensions.Count != 0 && !config.AllowedExtensions.Contains(cppFunction.Name))
            {
                return true;
            }

            if (config.IgnoredFunctions.Contains(cppFunction.Name))
            {
                return true;
            }

            if (config.IgnoredExtensions.Contains(cppFunction.Name))
            {
                return true;
            }

            if (cppFunction.Parameters.Count == 0 || cppFunction.Parameters[0].Type.TypeKind == CppTypeKind.Pointer && !isCustomHandle)
            {
                return true;
            }

            if (cppFunction.Parameters[0].Type.GetDisplayName() == typedef.GetDisplayName())
            {
                return false;
            }

            if (cppFunction.Parameters[0].Type is CppQualifiedType qualifiedType && qualifiedType.ElementType.IsType(typedef))
            {
                return false;
            }

            return true;
        }

        protected virtual bool FilterExtension(GenContext context, HashSet<CsFunctionVariation> definedExtensions, CsFunctionVariation variation)
        {
            if (definedExtensions.Contains(variation))
            {
                LogWarn($"{context.FilePath}: {variation} extension is already defined!");
                return true;
            }

            definedExtensions.Add(variation);

            return false;
        }

        public override void Generate(FileSet files, ParseResult result, string outputPath, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata)
        {
            var compilation = result.Compilation;
            DefinedVariationsFunctions = GetGenerationStep<FunctionGenerationStep>().DefinedVariationsFunctions;
            string folder = Path.Combine(outputPath, "Extensions");
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, "Extensions.cs");

            // Generate Extensions
            using var writer = new CsSplitCodeWriter(filePath, config.Namespace, SetupExtensionUsings(), config.HeaderInjector);
            GenContext context = new(result, filePath, writer);

            using (writer.PushBlock($"public static unsafe partial class Extensions"))
            {
                for (int i = 0; i < compilation.Typedefs.Count; i++)
                {
                    if (!files.Contains(compilation.Typedefs[i].SourceFile))
                        continue;

                    WriteExtensionsForHandle(context, compilation.Typedefs[i]);
                }
            }
        }

        protected virtual void WriteExtensionsForHandle(GenContext context, CppTypedef typedef, bool isCustomHandle = false)
        {
            if (FilterExtensionType(context, typedef))
            {
                return;
            }

            string handleName = typedef.Name;
            var compilation = context.Compilation;

            for (int i = 0; i < compilation.Functions.Count; i++)
            {
                var cppFunction = compilation.Functions[i];

                if (FilterExtensionFunction(context, cppFunction, typedef, isCustomHandle))
                {
                    continue;
                }

                var extensionPrefix = config.GetExtensionNamePrefix(handleName);

                var csFunctionName = config.GetCsFunctionName(cppFunction.Name);
                var csName = config.GetExtensionName(csFunctionName, extensionPrefix);

                var csFunction = generator.CreateCsFunction(cppFunction, CsFunctionKind.Extension, csName, DefinedExtensions, out var overload);
                funcGen.GenerateVariations(cppFunction.Parameters, overload);
                WriteExtensions(context, DefinedVariationsFunctions, csFunctionName, overload, "public static");
            }
        }

        protected virtual void WriteExtensions(GenContext context, HashSet<CsFunctionVariation> definedExtensions, string originalFunction, CsFunctionOverload overload, params string[] modifiers)
        {
            for (int j = 0; j < overload.Variations.Count; j++)
            {
                WriteExtension(context, definedExtensions, originalFunction, overload, overload.Variations[j], modifiers);
            }
        }

        public virtual List<IParameterWriter> ParameterWriters { get; } =
        [
            new HandleParameterWriter(),
            new UseThisParameterWriter(),
            new DefaultValueParameterWriter(),
            new StringParameterWriter(),
            new RefParameterWriter(),
            new SpanParameterWriter(),
            new ArrayParameterWriter(),
            new BoolParameterWriter(),
            new FallthroughParameterWriter(),
        ];

        public void AddParamterWriter(IParameterWriter writer)
        {
            ParameterWriters.Add(writer);
            ParameterWriters.Sort(new ParameterPriorityComparer());
        }

        public void RemoveParamterWriter(IParameterWriter writer)
        {
            ParameterWriters.Remove(writer);
        }

        public void OverwriteParameterWriter<T>(IParameterWriter newWriter) where T : IParameterWriter
        {
            for (int i = 0; i < ParameterWriters.Count; i++)
            {
                var writer = ParameterWriters[i];
                if (writer is T)
                {
                    ParameterWriters[i] = newWriter;
                    break;
                }
            }
            ParameterWriters.Sort(new ParameterPriorityComparer());
        }

        protected virtual void WriteExtension(GenContext context, HashSet<CsFunctionVariation> definedExtensions, string originalFunction, CsFunctionOverload overload, CsFunctionVariation variation, string[] modifiers)
        {
            var writer = context.Writer;
            CsType csReturnType = variation.ReturnType;
            generator.PrepareArgs(variation, csReturnType);

            string header = generator.BuildFunctionHeader(variation, csReturnType, WriteFunctionFlags.Extension, config.GenerateMetadata);
            variation.BuildFunctionHeaderId(WriteFunctionFlags.Extension);

            if (FilterExtension(context, definedExtensions, variation))
            {
                return;
            }

            CsCodeGenerator.ClassifyParameters(overload, variation, csReturnType, out bool firstParamReturn, out int offset, out bool hasManaged);

            LogInfo("defined extension " + header);

            writer.WriteLines(overload.Comment);
            if (config.GenerateMetadata)
            {
                writer.WriteLines(overload.Attributes);
            }

            using (writer.PushBlock($"{string.Join(" ", modifiers)} {header}"))
            {
                StringBuilder sb = new();

                if (!firstParamReturn && (!csReturnType.IsVoid || csReturnType.IsVoid && csReturnType.IsPointer))
                {
                    if (csReturnType.IsBool && !csReturnType.IsPointer && !hasManaged)
                    {
                        sb.Append($"{config.GetBoolType()} ret = ");
                    }
                    else
                    {
                        sb.Append($"{csReturnType.Name} ret = ");
                    }
                }

                if (csReturnType.IsString)
                {
                    MarshalHelper.WriteStringConvertToManaged(sb, variation.ReturnType);
                }

                sb.Append($"{config.ApiName}.");

                if (hasManaged)
                {
                    sb.Append($"{originalFunction}(");
                }
                else if (firstParamReturn)
                {
                    sb.Append($"{originalFunction}Native(&ret" + (overload.Parameters.Count > 1 ? ", " : ""));
                }
                else
                {
                    sb.Append($"{originalFunction}Native(");
                }

                FunctionWriterContext writerContext = new(context.Writer, config, sb, overload, variation, WriteFunctionFlags.Extension);

                for (int i = 0; i < overload.Parameters.Count - offset; i++)
                {
                    var cppParameter = overload.Parameters[i + offset];
                    var paramFlags = ParameterFlags.None;

                    if (variation.TryGetParameter(cppParameter.Name, out var param))
                    {
                        paramFlags = param.Flags;
                        cppParameter = param;
                    }

                    foreach (var parameterWriter in ParameterWriters)
                    {
                        if (parameterWriter.CanWrite(writerContext, overload.Parameters[i + offset], cppParameter, paramFlags, i, offset))
                        {
                            parameterWriter.Write(writerContext, overload.Parameters[i + offset], cppParameter, paramFlags, i, offset);
                            break;
                        }
                    }

                    if (i != overload.Parameters.Count - 1 - offset)
                    {
                        sb.Append(", ");
                    }
                }

                if (csReturnType.IsString)
                {
                    sb.Append("));");
                }
                else
                {
                    sb.Append(");");
                }

                if (firstParamReturn)
                {
                    writer.WriteLine($"{csReturnType.Name} ret;");
                }

                writer.WriteLine(sb.ToString());

                writerContext.ConvertStrings();
                writerContext.FreeStringArrays();
                writerContext.FreeStrings();

                if (firstParamReturn || !csReturnType.IsVoid || csReturnType.IsVoid && csReturnType.IsPointer)
                {
                    if (csReturnType.IsBool && !csReturnType.IsPointer && !hasManaged)
                    {
                        writer.WriteLine("return ret != 0;");
                    }
                    else
                    {
                        writer.WriteLine("return ret;");
                    }
                }

                writerContext.EndBlocks();
            }

            writer.WriteLine();
        }
    }
}