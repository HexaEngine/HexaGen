namespace HexaGen.Batteries.Legacy.Steps
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using HexaGen.FunctionGeneration;
    using HexaGen.Metadata;
    using System.Collections.Generic;
    using System.Text;

    public class ExtensionGenerationStep : GenerationStep
    {
        protected readonly HashSet<string> LibDefinedExtensions = new();
        public readonly HashSet<string> DefinedTypeExtensions = new();
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
            metadata.DefinedTypeExtensions.AddRange(DefinedTypeExtensions);
            metadata.DefinedExtensions.AddRange(DefinedExtensions);
        }

        public override void CopyFromMetadata(CsCodeGeneratorMetadata metadata)
        {
            LibDefinedExtensions.AddRange(metadata.DefinedTypeExtensions);
        }

        public override void Reset()
        {
            LibDefinedExtensions.Clear();
            DefinedTypeExtensions.Clear();
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
                return true;

            if (LibDefinedExtensions.Contains(typedef.Name))
                return true;

            if (typedef.ElementType is not CppPointerType)
            {
                return true;
            }

            if (typedef.IsDelegate())
            {
                return true;
            }

            return false;
        }

        protected virtual bool FilterExtensionFunction(GenContext context, CppFunction cppFunction, CppTypedef typedef, bool isCustomHandle)
        {
#if CPPAST_15_OR_GREATER
            if (cppFunction.IsFunctionTemplate)
                return true;
#endif
            if (config.AllowedFunctions.Count != 0 && !config.AllowedFunctions.Contains(cppFunction.Name))
                return true;
            if (config.IgnoredFunctions.Contains(cppFunction.Name))
                return true;
            if (cppFunction.Parameters.Count == 0 || cppFunction.Parameters[0].Type.TypeKind == CppTypeKind.Pointer && !isCustomHandle)
                return true;

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

        protected virtual bool FilterExtension(GenContext context, HashSet<string> definedExtensions, string header)
        {
            lock (definedExtensions)
            {
                if (definedExtensions.Contains(header))
                {
                    LogWarn($"{context.FilePath}: {header} extension is already defined!");
                    return true;
                }

                definedExtensions.Add(header);
            }
            return false;
        }

        protected virtual bool FilterExtension(GenContext context, HashSet<CsFunctionVariation> definedExtensions, CsFunctionVariation variation)
        {
            lock (definedExtensions)
            {
                if (definedExtensions.Contains(variation))
                {
                    LogWarn($"{context.FilePath}: {variation} extension is already defined!");
                    return true;
                }

                definedExtensions.Add(variation);
            }
            return false;
        }

        public override void Generate(FileSet files, CppCompilation compilation, string outputPath, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata)
        {
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
            GenContext context = new(compilation, filePath, writer);

            using (writer.PushBlock($"public static unsafe partial class Extensions"))
            {
                for (int i = 0; i < compilation.Typedefs.Count; i++)
                {
                    var typedef = compilation.Typedefs[i];
                    if (!files.Contains(typedef.SourceFile))
                        continue;
                    WriteExtensionsForHandle(context, typedef);
                }
            }
        }

        protected virtual void WriteExtensionsForHandle(GenContext context, CppTypedef typedef, bool isCustomHandle = false)
        {
            if (FilterExtensionType(context, typedef))
                return;

            string handleName = typedef.Name;
            var compilation = context.Compilation;

            for (int i = 0; i < compilation.Functions.Count; i++)
            {
                var cppFunction = compilation.Functions[i];

                if (FilterExtensionFunction(context, cppFunction, typedef, isCustomHandle))
                    continue;

                var extensionPrefix = config.GetExtensionNamePrefix(handleName);

                var csFunctionName = config.GetCsFunctionName(cppFunction.Name);
                var csName = config.GetExtensionName(csFunctionName, extensionPrefix);

                generator.CreateCsFunction(cppFunction, CsFunctionKind.Extension, csName, DefinedExtensions, out var overload);
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
                    sb.Append($"{originalFunction}(");
                else if (firstParamReturn)
                    sb.Append($"{originalFunction}Native(&ret" + (overload.Parameters.Count > 1 ? ", " : ""));
                else
                    sb.Append($"{originalFunction}Native(");
                Stack<(string, CsParameterInfo, string)> stack = new();
                int strings = 0;
                Stack<string> arrays = new();
                int stacks = 0;

                for (int i = 0; i < overload.Parameters.Count - offset; i++)
                {
                    var cppParameter = overload.Parameters[i + offset];
                    var paramFlags = ParameterFlags.None;

                    if (variation.TryGetParameter(cppParameter.Name, out var param))
                    {
                        paramFlags = param.Flags;
                        cppParameter = param;
                    }

                    if (paramFlags.HasFlag(ParameterFlags.Default))
                    {
                        var rootParam = overload.Parameters[i + offset];
                        var paramCsDefault = cppParameter.DefaultValue!;
                        if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
                            sb.Append($"(string){paramCsDefault}");
                        else if (cppParameter.Type.IsBool && !cppParameter.Type.IsPointer && !cppParameter.Type.IsArray)
                            sb.Append($"({config.GetBoolType()})({paramCsDefault})");
                        else if (rootParam.Type.IsEnum)
                            sb.Append($"({rootParam.Type.Name})({paramCsDefault})");
                        else if (cppParameter.Type.IsPrimitive || cppParameter.Type.IsPointer || cppParameter.Type.IsArray)
                            sb.Append($"({rootParam.Type.Name})({paramCsDefault})");
                        else
                            sb.Append($"{paramCsDefault}");
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.String))
                    {
                        if (paramFlags.HasFlag(ParameterFlags.Array))
                        {
                            MarshalHelper.WriteStringArrayConvertToUnmanaged(writer, cppParameter.Type, cppParameter.Name, arrays.Count);
                            sb.Append($"pStrArray{arrays.Count}");
                            arrays.Push(cppParameter.Name);
                        }
                        else
                        {
                            if (paramFlags.HasFlag(ParameterFlags.Ref))
                            {
                                stack.Push((cppParameter.Name, cppParameter, $"pStr{strings}"));
                            }

                            MarshalHelper.WriteStringConvertToUnmanaged(writer, cppParameter.Type, cppParameter.Name, strings);
                            sb.Append($"pStr{strings}");
                            strings++;
                        }
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Ref))
                    {
                        writer.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.Name} = &{cppParameter.Name})");
                        sb.Append($"({overload.Parameters[i + offset].Type.Name})p{cppParameter.Name}");
                        stacks++;
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Array))
                    {
                        writer.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.Name} = {cppParameter.Name})");
                        sb.Append($"({overload.Parameters[i + offset].Type.Name})p{cppParameter.Name}");
                        stacks++;
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Bool) && !paramFlags.HasFlag(ParameterFlags.Ref) && !paramFlags.HasFlag(ParameterFlags.Pointer))
                    {
                        sb.Append($"{cppParameter.Name} ? ({config.GetBoolType()})1 : ({config.GetBoolType()})0");
                    }
                    else
                    {
                        sb.Append(cppParameter.Name);
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

                while (stack.TryPop(out var stackItem))
                {
                    MarshalHelper.WriteStringConvertToManaged(writer, stackItem.Item2.Type, stackItem.Item1, stackItem.Item3);
                }

                while (arrays.TryPop(out var arrayName))
                {
                    MarshalHelper.WriteFreeUnmanagedStringArray(writer, arrayName, arrays.Count);
                }

                while (strings > 0)
                {
                    strings--;
                    MarshalHelper.WriteFreeString(writer, strings);
                }

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

                while (stacks > 0)
                {
                    stacks--;
                    writer.EndBlock();
                }
            }

            writer.WriteLine();
        }
    }
}