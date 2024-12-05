namespace HexaGen.GenerationSteps
{
    using CppAst;
    using HexaGen.Core;
    using HexaGen.Core.CSharp;
    using HexaGen.Metadata;
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using System.Text;

    public class ComExtensionGenerationStep : ExtensionGenerationStep
    {
        private readonly CsComCodeGenerator comGenerator;

        public readonly HashSet<string> DefinedCOMExtensionTypes = new();
        protected readonly HashSet<string> LibDefinedCOMExtensionTypes = new();
        public readonly Dictionary<string, HashSet<CsFunctionVariation>> DefindedCOMExtensions = new();

        public ComExtensionGenerationStep(CsComCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
            comGenerator = generator;
        }

        public override void CopyToMetadata(CsCodeGeneratorMetadata metadata)
        {
            base.CopyToMetadata(metadata);
            metadata.DefinedCOMExtensions.AddRange(DefindedCOMExtensions);
            metadata.DefinedCOMExtensionTypes.AddRange(DefinedCOMExtensionTypes);
        }

        public override void CopyFromMetadata(CsCodeGeneratorMetadata metadata)
        {
            base.CopyFromMetadata(metadata);
            LibDefinedCOMExtensionTypes.AddRange(metadata.DefinedCOMExtensionTypes);
        }

        public override void Reset()
        {
            base.Reset();
            LibDefinedCOMExtensionTypes.Clear();
            DefinedCOMExtensionTypes.Clear();
            DefindedCOMExtensions.Clear();
        }

        protected override List<string> SetupExtensionUsings()
        {
            var usings = base.SetupExtensionUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }

        protected virtual bool FilterCOMClassType(GenContext context, CppClass cppClass)
        {
            if (config.AllowedTypes.Count != 0 && !config.AllowedTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (config.IgnoredTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (LibDefinedCOMExtensionTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (DefinedCOMExtensionTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (!comGenerator.HasGUID(cppClass.Name) && (cppClass.Fields.Count != 0 || cppClass.Functions.Count == 0 || !cppClass.IsAbstract))
            {
                return true;
            }

            DefinedCOMExtensionTypes.Add(cppClass.Name);

            return false;
        }

        protected virtual bool FilterCOMBaseClassType(GenContext context, CppClass cppClass)
        {
            if (config.AllowedTypes.Count != 0 && !config.AllowedTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (config.IgnoredTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (!comGenerator.HasGUID(cppClass.Name) && (cppClass.Fields.Count != 0 || cppClass.Functions.Count == 0 || !cppClass.IsAbstract))
            {
                return true;
            }

            return false;
        }

        protected virtual bool FilterCOMExtensionFunction(GenContext context, CppFunction cppFunction)
        {
            if (config.AllowedFunctions.Count != 0 && !config.AllowedFunctions.Contains(cppFunction.Name))
            {
                return true;
            }

            if (config.IgnoredFunctions.Contains(cppFunction.Name))
            {
                return true;
            }

            return false;
        }

        public override void Generate(FileSet files, CppCompilation compilation, string outputPath, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata)
        {
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
                    if (!files.Contains(compilation.Typedefs[i].SourceFile))
                        continue;
                    WriteExtensionsForHandle(context, compilation.Typedefs[i]);
                }

                for (int i = 0; i < compilation.Classes.Count; i++)
                {
                    if (!files.Contains(compilation.Classes[i].SourceFile))
                        continue;
                    WriteExtensionsForCOMObject(context, compilation.Classes[i]);
                }
            }
        }

        protected virtual void WriteExtensionsForCOMObject(GenContext context, CppClass cppClass)
        {
            if (FilterCOMClassType(context, cppClass))
                return;

            string csName = config.GetCsCleanName(cppClass.Name);
            var mapping = config.GetTypeMapping(cppClass.Name);
            csName = mapping?.FriendlyName ?? csName;

            int vTableIndex = 0;
            WriteExtensionsForCOMObject(context, cppClass, cppClass, csName, ref vTableIndex);
        }

        protected virtual void WriteExtensionsForCOMObject(GenContext context, CppClass targetClass, CppClass cppClass, string className, ref int vTableIndex)
        {
            for (int i = 0; i < cppClass.BaseTypes.Count; i++)
            {
                var baseType = cppClass.BaseTypes[i];
                if (baseType.Type is CppClass baseClass)
                {
                    if (FilterCOMBaseClassType(context, baseClass))
                    {
                        continue;
                    }

                    WriteExtensionsForCOMObject(context, targetClass, baseClass, className, ref vTableIndex);
                }
            }

            List<CsFunction> commands = new();
            for (int i = 0; i < cppClass.Functions.Count; i++, vTableIndex++)
            {
                CppFunction cppFunction = cppClass.Functions[i];

#if CPPAST_15_OR_GREATER
                if (cppFunction.IsFunctionTemplate)
                {
                    vTableIndex--;
                    continue;
                }
#else
                if (!cppFunction.IsVirtual)
                {
                    vTableIndex--;
                    continue;
                }
#endif

                if (FilterCOMExtensionFunction(context, cppFunction))
                {
                    continue;
                }

                var extensionPrefix = config.GetExtensionNamePrefix(cppClass.Name);

                var csFunctionName = config.GetCsFunctionName(cppFunction.Name);
                var csName = config.GetExtensionName(csFunctionName, extensionPrefix);

                generator.CreateCsFunction(cppFunction, CsFunctionKind.Extension, csName, commands, out var overload);
                funcGen.GenerateVariations(cppFunction.Parameters, overload);

                if (!DefindedCOMExtensions.TryGetValue(targetClass.Name, out var definedExtensions))
                {
                    definedExtensions = new(IdentifierComparer<CsFunctionVariation>.Default);
                    DefindedCOMExtensions.Add(targetClass.Name, definedExtensions);
                }

                if (!WriteCOMExtensions(context, definedExtensions, cppFunction, overload, className, vTableIndex, "public static"))
                {
                    vTableIndex--;
                }
            }
        }

        protected virtual bool WriteCOMExtensions(GenContext context, HashSet<CsFunctionVariation> definedExtensions, CppFunction cppfunction, CsFunctionOverload overload, string className, int index, params string[] modifiers)
        {
            bool hasWritten = false;
            for (int i = 0; i < overload.Variations.Count; i++)
            {
                hasWritten |= WriteCOMExtension(context, definedExtensions, cppfunction, overload, overload.Variations[i], className, index, modifiers);
                if (i == 0 && !hasWritten)
                {
                    break;
                }
            }
            return hasWritten;
        }

        protected virtual bool WriteCOMExtension(GenContext context, HashSet<CsFunctionVariation> definedExtensions, CppFunction cppfunction, CsFunctionOverload overload, CsFunctionVariation variation, string className, int index, string[] modifiers)
        {
            var writer = context.Writer;
            CsType csReturnType = variation.ReturnType;

            generator.PrepareArgs(variation, csReturnType);

            variation.BuildExtensionSignatureIdentifierForCOM(className);

            if (FilterExtension(context, definedExtensions, variation))
            {
                return false;
            }

            string modifierString = string.Join(" ", modifiers);
            string header = variation.BuildFullExtensionSignatureForCOM(className, config.GenerateMetadata);
            string signatureNameless = config.GetNamelessParameterSignatureForCOM(className, cppfunction.Parameters, false);
            string signatureNamelessCompat = config.GetNamelessParameterSignatureForCOM(className, cppfunction.Parameters, false, compatibility: true);

            LogInfo("defined extension " + header);

            writer.WriteLines(overload.Comment);

            if (config.GenerateMetadata)
            {
                writer.WriteLines(overload.Attributes);
            }

            using (writer.PushBlock($"{modifierString} {header}"))
            {
                writer.WriteLine($"{className}* handle = comObj.Handle;");
                StringBuilder sb = new();
                StringBuilder builder = new();

                bool hasManaged = false;
                for (int j = 0; j < overload.Parameters.Count - 0; j++)
                {
                    var cppParameter = overload.Parameters[j + 0];
                    if (variation.HasParameter(cppParameter))
                    {
                        continue;
                    }

                    if (!overload.DefaultValues.TryGetValue(cppParameter.Name, out string? paramCsDefault))
                    {
                        continue;
                    }

                    if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
                    {
                        hasManaged = true;
                    }
                }

                if (!csReturnType.IsVoid || csReturnType.IsVoid && csReturnType.IsPointer)
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

                var ptr = index == 0 ? "*handle->LpVtbl" : $"handle->LpVtbl[{index}]";
                var tail = variation.Parameters.Count > 0 ? ", " : string.Empty;

                builder.Append(sb);
                builder.Append($"((delegate* unmanaged[Stdcall]<{signatureNamelessCompat}, {overload.ReturnType.Name}>)({ptr}))((nint)handle{tail}");
                sb.Append($"((delegate* unmanaged[Stdcall]<{signatureNameless}, {overload.ReturnType.Name}>)({ptr}))(handle{tail}");

                Stack<(string, CsParameterInfo, string)> stack = new();
                int strings = 0;
                Stack<string> arrays = new();
                int stacks = 0;

                void Append(string str)
                {
                    sb.Append(str);
                    builder.Append(str);
                }

                void AppendD(string str, string compat)
                {
                    sb.Append(str);
                    builder.Append(compat);
                }

                for (int i = 0; i < overload.Parameters.Count - 0; i++)
                {
                    var cppParameter = overload.Parameters[i + 0];
                    var paramFlags = ParameterFlags.None;

                    if (variation.TryGetParameter(cppParameter.Name, out var param))
                    {
                        paramFlags = param.Flags;
                        cppParameter = param;
                    }

                    if (paramFlags.HasFlag(ParameterFlags.Default))
                    {
                        var rootParam = overload.Parameters[i + 0];
                        var paramCsDefault = cppParameter.DefaultValue!;
                        if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
                        {
                            Append($"(string){paramCsDefault}");
                        }
                        else if (cppParameter.Type.IsBool && !cppParameter.Type.IsPointer && !cppParameter.Type.IsArray)
                        {
                            Append($"({config.GetBoolType()})({paramCsDefault})");
                        }
                        else if (rootParam.Type.IsEnum)
                        {
                            Append($"({rootParam.Type.Name})({paramCsDefault})");
                        }
                        else if (cppParameter.Type.IsPrimitive || cppParameter.Type.IsPointer || cppParameter.Type.IsArray)
                        {
                            if (cppParameter.Type.IsPointer || cppParameter.Type.IsArray)
                            {
                                AppendD($"({rootParam.Type.Name})({paramCsDefault})", $"(nint)({paramCsDefault})");
                            }
                            else
                            {
                                Append($"({rootParam.Type.Name})({paramCsDefault})");
                            }
                        }
                        else
                        {
                            Append($"{paramCsDefault}");
                        }
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.String))
                    {
                        if (paramFlags.HasFlag(ParameterFlags.Array))
                        {
                            MarshalHelper.WriteStringArrayConvertToUnmanaged(writer, cppParameter.Type, cppParameter.Name, arrays.Count);
                            AppendD($"pStrArray{arrays.Count}", $"(nint)pStrArray{arrays.Count}");
                            arrays.Push(cppParameter.Name);
                        }
                        else
                        {
                            if (paramFlags.HasFlag(ParameterFlags.Ref))
                            {
                                stack.Push((cppParameter.Name, cppParameter, $"pStr{strings}"));
                            }

                            MarshalHelper.WriteStringConvertToUnmanaged(writer, cppParameter.Type, cppParameter.Name, strings);
                            AppendD($"pStr{strings}", $"(nint)pStr{strings}");
                            strings++;
                        }
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Ref))
                    {
                        writer.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.Name.Replace("@", string.Empty)} = &{cppParameter.Name})");
                        AppendD($"({overload.Parameters[i + 0].Type.Name})p{cppParameter.Name}", $"(nint)p{cppParameter.Name}");
                        stacks++;
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Span))
                    {
                        writer.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.Name.Replace("@", string.Empty)} = {cppParameter.Name})");
                        AppendD($"({overload.Parameters[i + 0].Type.Name})p{cppParameter.Name}", $"(nint)p{cppParameter.Name}");
                        stacks++;
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Out))
                    {
                        writer.WriteLine($"{cppParameter.Name} = default;");
                        if (paramFlags.HasFlag(ParameterFlags.COMPtr))
                        {
                            AppendD($"({overload.Parameters[i + 0].Type.Name}){cppParameter.Name}.GetAddressOf()", $"(nint){cppParameter.Name}.GetAddressOf()");
                        }
                        else
                        {
                            Append($"out {cppParameter.Name}");
                        }
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Array))
                    {
                        writer.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.Name} = {cppParameter.Name})");
                        AppendD($"({overload.Parameters[i + 0].Type.Name})p{cppParameter.Name}", $"(nint)p{cppParameter.Name}");
                        stacks++;
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Bool) && !paramFlags.HasFlag(ParameterFlags.Ref) && !paramFlags.HasFlag(ParameterFlags.Pointer))
                    {
                        Append($"{cppParameter.Name} ? ({config.GetBoolType()})1 : ({config.GetBoolType()})0");
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.COMPtr))
                    {
                        if (paramFlags.HasFlag(ParameterFlags.Ref))
                        {
                            AppendD($"({overload.Parameters[i + 0].Type.Name}){cppParameter.Name}.GetAddressOf()", $"(nint){cppParameter.Name}.GetAddressOf()");
                        }
                        else
                        {
                            AppendD($"({overload.Parameters[i + 0].Type.Name}){cppParameter.Name}.Handle", $"(nint){cppParameter.Name}.Handle");
                        }
                    }
                    else
                    {
                        if (cppParameter.Type.IsPointer)
                        {
                            AppendD(cppParameter.Name, $"(nint){cppParameter.Name}");
                        }
                        else
                        {
                            Append(cppParameter.Name);
                        }
                    }

                    if (i != overload.Parameters.Count - 1 - 0)
                    {
                        Append(", ");
                    }
                }

                if (csReturnType.IsString)
                {
                    Append("));");
                }
                else
                {
                    Append(");");
                }

                writer.WriteLine("#if NET5_0_OR_GREATER");
                writer.WriteLine(sb.ToString());
                writer.WriteLine("#else");
                writer.WriteLine(builder.ToString());
                writer.WriteLine("#endif");

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

                if (!csReturnType.IsVoid || csReturnType.IsVoid && csReturnType.IsPointer)
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
            return true;
        }
    }
}