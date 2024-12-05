namespace HexaGen.GenerationSteps
{
    using CppAst;
    using HexaGen.Core;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using HexaGen.Metadata;
    using System.Collections.Frozen;
    using System.Collections.Generic;
    using System.Text;

    public class ComTypeGenerationStep : TypeGenerationStep
    {
        private readonly CsComCodeGenerator comGenerator;

        public ComTypeGenerationStep(CsComCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
            comGenerator = generator;
        }

        protected override List<string> SetupTypeUsings()
        {
            var usings = base.SetupTypeUsings();
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

            if (LibDefinedTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (!comGenerator.HasGUID(cppClass.Name) && (cppClass.Fields.Count != 0 || cppClass.Functions.Count == 0 || !cppClass.IsAbstract))
            {
                return true;
            }

            return false;
        }

        protected virtual bool FilterCOMFunction(GenContext context, CppFunction cppFunction)
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

        protected virtual bool FilterCOMMemberFunction(GenContext context, HashSet<string> definedFunctions, string header)
        {
            if (definedFunctions.Contains(header))
            {
                LogWarn($"{context.FilePath}: {header} member function is already defined!");
                return true;
            }

            definedFunctions.Add(header);

            return false;
        }

        protected virtual bool FilterCOMMemberFunction(GenContext context, HashSet<CsFunctionVariation> definedFunctions, CsFunctionVariation variation)
        {
            if (definedFunctions.Contains(variation))
            {
                LogWarn($"{context.FilePath}: {variation} member function is already defined!");
                return true;
            }

            definedFunctions.Add(variation);

            return false;
        }

        public override void Generate(FileSet files, CppCompilation compilation, string outputPath, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata)
        {
            string folder = Path.Combine(outputPath, "Structs");
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            Directory.CreateDirectory(folder);

            for (int i = 0; i < compilation.Classes.Count; i++)
            {
                CppClass? cppClass = compilation.Classes[i];

                if (!files.Contains(cppClass.SourceFile))
                    continue;

                if (config.AllowedTypes.Count != 0 && !config.AllowedTypes.Contains(cppClass.Name))
                {
                    continue;
                }

                if (config.IgnoredTypes.Contains(cppClass.Name))
                {
                    continue;
                }

                if (LibDefinedTypes.Contains(cppClass.Name))
                {
                    continue;
                }

                if (DefinedTypes.Contains(cppClass.Name))
                {
                    LogWarn($"{cppClass} is already defined!");
                    continue;
                }

                DefinedTypes.Add(cppClass.Name);

                string csName = config.GetCsCleanName(cppClass.Name);

                var mapping = config.GetTypeMapping(cppClass.Name);

                csName = mapping?.FriendlyName ?? csName;

                string filePath = Path.Combine(folder, $"{csName}.cs");
                using var writer = new CsCodeWriter(filePath, config.Namespace, SetupTypeUsings(), config.HeaderInjector);
                GenContext context = new(compilation, filePath, writer);

                if (comGenerator.TryGetGUID(cppClass.Name, out var guid))
                {
                    WriteCOMObject(context, cppClass, mapping, csName, guid);
                }
                else
                {
                    if (cppClass.Fields.Count == 0 && cppClass.Functions.Count > 0 && cppClass.IsAbstract)
                    {
                        WriteCOMObject(context, cppClass, mapping, csName, null);
                        continue;
                    }

                    WriteClass(context, cppClass, mapping, csName);
                }
            }
        }

        private void WriteCOMObject(GenContext context, CppClass cppClass, TypeMapping? mapping, string csName, Guid? guid)
        {
            var writer = context.Writer;
            if (cppClass.ClassKind == CppClassKind.Class || cppClass.Name.EndsWith("_T") || csName == "void")
            {
                return;
            }

            int vTableIndex = 0;
            //bool isReadOnly = false;
            string modifier = "partial";

            LogInfo("defined struct " + csName);
            var commentWritten = config.WriteCsSummary(cppClass.Comment, writer);
            if (!commentWritten)
            {
                commentWritten = config.WriteCsSummary(mapping?.Comment, writer);
            }
            if (guid != null)
            {
                writer.WriteLine($"[Guid(\"{guid}\")]");
            }

            if (config.GenerateMetadata)
            {
                writer.WriteLine($"[NativeName(NativeNameType.StructOrClass, \"{cppClass.Name}\")]");
            }

            StringBuilder sb = new($"IComObject, IComObject<{csName}>");
            {
                Queue<CppBaseType> baseTypes = new();
                for (int i = 0; i < cppClass.BaseTypes.Count; i++)
                {
                    baseTypes.Enqueue(cppClass.BaseTypes[i]);
                }
                while (baseTypes.Count > 0)
                {
                    var current = baseTypes.Dequeue();
                    if (current.Type is CppClass baseClass)
                    {
                        string csNameBaseClass = config.GetCsCleanName(baseClass.Name);
                        sb.Append($", IComObject<{csNameBaseClass}>");

                        for (int i = 0; i < baseClass.BaseTypes.Count; i++)
                        {
                            baseTypes.Enqueue(baseClass.BaseTypes[i]);
                        }
                    }
                }
            }

            using (writer.PushBlock($"public {modifier} struct {csName} : {sb}"))
            {
                writer.WriteLine("public unsafe void** LpVtbl;");
                writer.WriteLine();

                if (guid != null)
                {
                    writer.WriteLine($"public static readonly Guid Guid = new(\"{guid}\");");
                    writer.WriteLine();
                }

                WriteCOMConstructor(writer, csName);

                WriteCOMObjectMemberFunctions(context, cppClass, cppClass, csName, ref vTableIndex, []);

                using (writer.PushBlock($"unsafe void*** IComObject.AsVtblPtr()"))
                {
                    writer.WriteLine($"return (void***)Unsafe.AsPointer(ref Unsafe.AsRef(in this));");
                }
                writer.WriteLine();

                for (int i = 0; i < cppClass.BaseTypes.Count; i++)
                {
                    var baseType = cppClass.BaseTypes[i];
                    if (baseType.Type is CppClass baseClass)
                    {
                        WriteCOMBaseTypeCast(writer, csName, baseClass);
                    }
                }
            }

            writer.WriteLine();
        }

        private void WriteCOMObjectMemberFunctions(GenContext context, CppClass targetClass, CppClass cppClass, string csName, ref int vTableIndex, HashSet<string> signatures)
        {
            for (int i = 0; i < cppClass.BaseTypes.Count; i++)
            {
                var baseType = cppClass.BaseTypes[i];
                if (baseType.Type is CppClass baseClass)
                {
                    if (FilterCOMClassType(context, baseClass))
                    {
                        continue;
                    }

                    WriteCOMObjectMemberFunctions(context, targetClass, baseClass, csName, ref vTableIndex, signatures);
                }
            }

            if (targetClass.Name == "ID3D11ShaderReflection")
            {
            }

            List<CsFunction> commands = new();
            for (int i = 0; i < cppClass.Functions.Count; i++, vTableIndex++)
            {
                var cppFunction = cppClass.Functions[i];

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

                if (FilterCOMFunction(context, cppFunction))
                {
                    continue;
                }

                string? csFunctionName = config.GetCsFunctionName(cppFunction.Name);

                CsFunction? function = generator.CreateCsFunction(cppFunction, CsFunctionKind.Member, csFunctionName, commands, out var overload);
                funcGen.GenerateVariations(cppFunction.Parameters, overload);

                if (!MemberFunctions.TryGetValue(targetClass.Name, out var definedFunctions))
                {
                    definedFunctions = new(IdentifierComparer<CsFunctionVariation>.Default);
                    MemberFunctions.Add(targetClass.Name, definedFunctions);
                }

                if (!WriteCOMFunctions(context, definedFunctions, cppFunction, overload, csName, vTableIndex, "public readonly unsafe"))
                {
                    vTableIndex--;
                }
            }
        }

        private void WriteCOMConstructor(ICodeWriter writer, string csName)
        {
            using (writer.PushBlock($"public unsafe {csName} (void** lpVtbl = null)"))
            {
                writer.WriteLine("LpVtbl = lpVtbl;");
            }
            writer.WriteLine();
        }

        private void WriteCOMBaseTypeCast(ICodeWriter writer, string csName, CppClass baseClass)
        {
            string csNameBaseClass = config.GetCsCleanName(baseClass.Name);
            using (writer.PushBlock($"public unsafe static implicit operator {csNameBaseClass} ({csName} value)"))
            {
                writer.WriteLine($"return Unsafe.As<{csName}, {csNameBaseClass}>(ref value);");
            }
            writer.WriteLine();

            for (int i = 0; i < baseClass.BaseTypes.Count; i++)
            {
                var baseType = baseClass.BaseTypes[i];
                if (baseType.Type is CppClass basebaseClass)
                {
                    WriteCOMBaseTypeCast(writer, csName, basebaseClass);
                }
            }
        }

        private bool WriteCOMFunctions(GenContext context, HashSet<CsFunctionVariation> definedFunctions, CppFunction cppFunction, CsFunctionOverload overload, string className, int index, params string[] modifiers)
        {
            bool hasWritten = false;
            for (int j = 0; j < overload.Variations.Count; j++)
            {
                hasWritten |= WriteCOMFunction(context, definedFunctions, cppFunction, overload, overload.Variations[j], className, index, modifiers);
                if (j == 0 && !hasWritten) // if the first function hasn't been written (the overload) then all variations are invalid.
                {
                    break;
                }
            }
            return hasWritten;
        }

        private bool WriteCOMFunction(GenContext context, HashSet<CsFunctionVariation> definedFunctions, CppFunction cppFunction, CsFunctionOverload overload, CsFunctionVariation variation, string className, int index, params string[] modifiers)
        {
            var writer = context.Writer;
            CsType csReturnType = variation.ReturnType;
            generator.PrepareArgs(variation, csReturnType);

            variation.BuildSignatureIdentifierForCOM();

            if (FilterCOMMemberFunction(context, definedFunctions, variation))
            {
                return false;
            }

            string header = variation.BuildFullSignatureForCOM(config.GenerateMetadata);
            string signatureNameless = config.GetNamelessParameterSignatureForCOM(className, cppFunction.Parameters, false);
            string signatureNamelessCompat = config.GetNamelessParameterSignatureForCOM(className, cppFunction.Parameters, false, compatibility: true);

            CsCodeGenerator.ClassifyParameters(overload, variation, csReturnType, out _, out int offset, out bool hasManaged);

            LogInfo("defined function " + header);

            writer.WriteLines(overload.Comment);
            if (config.GenerateMetadata)
            {
                writer.WriteLines(overload.Attributes);
            }

            using (writer.PushBlock($"{string.Join(" ", modifiers)} {header}"))
            {
                writer.WriteLine($"{className}* ptr = ({className}*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));");
                StringBuilder sb = new();
                StringBuilder builder = new();

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

                var ptr = index == 0 ? "*LpVtbl" : $"LpVtbl[{index}]";
                var tail = variation.Parameters.Count > 0 ? ", " : string.Empty;

                builder.Append(sb);
                builder.Append($"((delegate* unmanaged[Stdcall]<{signatureNamelessCompat}, {overload.ReturnType.Name}>)({ptr}))((nint)ptr{tail}");
                sb.Append($"((delegate* unmanaged[Stdcall]<{signatureNameless}, {overload.ReturnType.Name}>)({ptr}))(ptr{tail}");
                int start = sb.Length;

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
                        writer.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.Name} = &{cppParameter.Name})");
                        AppendD($"({overload.Parameters[i + offset].Type.Name})p{cppParameter.Name}", $"(nint)p{cppParameter.Name}");
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
                        AppendD($"({overload.Parameters[i + offset].Type.Name})p{cppParameter.Name}", $"(nint)p{cppParameter.Name}");
                        stacks++;
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Bool) && !paramFlags.HasFlag(ParameterFlags.Ref) && !paramFlags.HasFlag(ParameterFlags.Pointer))
                    {
                        Append($"{cppParameter.Name} ? ({config.GetBoolType()} )1 : ( {config.GetBoolType()})0");
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

                    if (i != overload.Parameters.Count - 1 - offset)
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