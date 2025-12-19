namespace HexaGen.GenerationSteps
{
    using HexaGen.Core;
    using HexaGen.Core.Collections;
    using HexaGen.Core.CSharp;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Types;
    using HexaGen.FunctionGeneration;
    using HexaGen.FunctionGeneration.ParameterWriters;
    using HexaGen.Metadata;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class FunctionGenerationStep : GenerationStep
    {
        protected readonly HashSet<string> LibDefinedFunctions = [];
        public readonly HashSet<string> CppDefinedFunctions = [];
        public readonly HashSet<string> DefinedNativeFunctions = [];
        public readonly List<CsFunction> DefinedFunctions = [];
        public readonly HashSet<CsFunctionVariation> DefinedVariationsFunctions = new(IdentifierComparer<CsFunctionVariation>.Default);
        protected readonly HashSet<string> OutReturnFunctions = [];

        private FunctionGenerator funcGen = null!;
        private FunctionTableBuilder FunctionTableBuilder = null!;

        public FunctionGenerationStep(CsCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
        }

        public override string Name { get; } = "Functions";

        public override void Configure(CsCodeGeneratorConfig config)
        {
            Enabled = config.GenerateFunctions;
            funcGen = generator.FunctionGenerator;
            FunctionTableBuilder = generator.FunctionTableBuilder;
        }

        public override void CopyToMetadata(CsCodeGeneratorMetadata metadata)
        {
            metadata.CppDefinedFunctions.AddRange(CppDefinedFunctions);
            metadata.DefinedFunctions.AddRange(DefinedFunctions);
            metadata.FunctionTable = new(FunctionTableBuilder.Entries);
        }

        public override void CopyFromMetadata(CsCodeGeneratorMetadata metadata)
        {
            LibDefinedFunctions.AddRange(metadata.CppDefinedFunctions);
        }

        public override void Reset()
        {
            LibDefinedFunctions.Clear();
            CppDefinedFunctions.Clear();
            DefinedNativeFunctions.Clear();
            DefinedVariationsFunctions.Clear();
            OutReturnFunctions.Clear();
            DefinedFunctions.Clear();
        }

        protected virtual List<string> SetupFunctionUsings()
        {
            List<string> usings = ["System", "System.Runtime.CompilerServices", "System.Runtime.InteropServices", "HexaGen.Runtime", .. config.Usings];
            return usings;
        }

        protected virtual bool FilterFunctionIgnored(GenContext context, CppFunction cppFunction)
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

            if (config.IgnoredFunctions.Contains(cppFunction.Name))
            {
                return true;
            }

            return false;
        }

        protected virtual bool FilterNativeFunction(GenContext context, CppFunction cppFunction, string header)
        {
            if (LibDefinedFunctions.Contains(cppFunction.Name))
            {
                return true;
            }

            CppDefinedFunctions.Add(cppFunction.Name);

            if (DefinedNativeFunctions.Contains(header))
            {
                LogWarn($"{context.FilePath}: function {cppFunction}, C#: {header} is already defined!");
                return true;
            }

            DefinedNativeFunctions.Add(header);

            return false;
        }

        protected virtual bool FilterFunction(GenContext context, HashSet<CsFunctionVariation> definedFunctions, CsFunctionVariation variation)
        {
            if (definedFunctions.Contains(variation))
            {
                LogInfo($"{context.FilePath}: {variation} function is already defined!");
                return true;
            }
            definedFunctions.Add(variation);
            return false;
        }

        public override void Generate(FileSet files, ParseResult result, string outputPath, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata)
        {
            var compilation = result.Compilation;
            string folder = Path.Combine(outputPath, "Functions");
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, "Functions.cs");

            DefinedVariationsFunctions.Clear();

            // Generate Functions
            using var writer = new CsSplitCodeWriter(filePath, config.Namespace, SetupFunctionUsings(), config.HeaderInjector);
            GenContext context = new(result, filePath, writer);

            FunctionTableBuilder.Append(config.FunctionTableEntries);
            using (writer.PushBlock($"public unsafe partial class {config.ApiName}"))
            {
                if (!config.UseFunctionTable)
                {
                    writer.WriteLine($"internal const string LibName = \"{config.LibName}\";\n");
                }

                for (int i = 0; i < compilation.Functions.Count; i++)
                {
                    CppFunction? cppFunction = compilation.Functions[i];
                    if (!files.Contains(cppFunction.SourceFile))
                        continue;

                    if (FilterFunctionIgnored(context, cppFunction))
                    {
                        continue;
                    }

                    string? csName = config.GetCsFunctionName(cppFunction.Name);
                    string returnCsName = config.GetCsReturnType(cppFunction.ReturnType);
                    CppPrimitiveKind returnKind = cppFunction.ReturnType.GetPrimitiveKind();

                    bool boolReturn = returnCsName == "bool";
                    var argumentsString = config.GetParameterSignature(cppFunction.Parameters, false, config.GenerateMetadata);
                    var headerId = $"{csName}({config.GetParameterSignature(cppFunction.Parameters, false, false, false)})";
                    var header = $"{returnCsName} {csName}Native({argumentsString})";

                    if (FilterNativeFunction(context, cppFunction, headerId))
                    {
                        continue;
                    }

                    var function = generator.CreateCsFunction(cppFunction, CsFunctionKind.Default, csName, DefinedFunctions, out var overload);

                    writer.WriteLines(function.Comment);
                    if (config.GenerateMetadata)
                    {
                        writer.WriteLine($"[NativeName(NativeNameType.Func, \"{cppFunction.Name}\")]");
                        writer.WriteLine($"[return: NativeName(NativeNameType.Type, \"{cppFunction.ReturnType.GetDisplayName()}\")]");
                    }

                    string? modifiers = null;

                    switch (config.ImportType)
                    {
                        case ImportType.DllImport:
                            writer.WriteLine($"[DllImport(LibName, CallingConvention = CallingConvention.{cppFunction.CallingConvention.GetCallingConvention()}, EntryPoint = \"{cppFunction.Name}\")]");
                            modifiers = "internal static extern";
                            break;

                        case ImportType.LibraryImport:
                            writer.WriteLine($"[LibraryImport(LibName, EntryPoint = \"{cppFunction.Name}\")]");
                            writer.WriteLine($"[UnmanagedCallConv(CallConvs = new Type[] {{typeof({cppFunction.CallingConvention.GetCallingConventionLibrary()})}})]");
                            modifiers = "internal static partial";
                            break;

                        case ImportType.FunctionTable:

                            writer.WriteLine($"[MethodImpl(MethodImplOptions.AggressiveInlining)]");
                            if (boolReturn)
                            {
                                writer.BeginBlock($"internal static {config.GetBoolType()} {csName}Native({argumentsString})");
                            }
                            else
                            {
                                writer.BeginBlock($"internal static {header}");
                            }

                            string returnType = config.GetCsReturnType(cppFunction.ReturnType);
                            if (cppFunction.ReturnType.IsDelegate(out var outDelegate))
                            {
                                returnType = config.MakeDelegatePointer(outDelegate);
                            }
                            if (returnType == "bool")
                            {
                                returnType = config.GetBoolType();
                            }

                            string attributes = $"{cppFunction.CallingConvention.GetCallingConventionDelegate()}";

                            string delegateType;
                            if (cppFunction.Parameters.Count == 0)
                            {
                                delegateType = $"delegate* unmanaged[{attributes}]<{returnType}>";
                            }
                            else
                            {
                                delegateType = $"delegate* unmanaged[{attributes}]<{config.GetNamelessParameterSignature(cppFunction.Parameters, false, true)}, {returnType}>";
                            }

                            writer.WriteLine("#if NET5_0_OR_GREATER");
                            // isolates the argument names
                            string argumentNames = config.WriteFunctionMarshalling(cppFunction.Parameters);

                            int funcTableableIndex = FunctionTableBuilder.Add(cppFunction.Name);

                            if (returnCsName == "void")
                            {
                                writer.WriteLine($"(({delegateType})funcTable[{funcTableableIndex}])({argumentNames});");
                            }
                            else
                            {
                                writer.WriteLine($"return (({delegateType})funcTable[{funcTableableIndex}])({argumentNames});");
                            }

                            writer.WriteLine("#else");

                            string returnTypeOld = config.GetCsReturnType(cppFunction.ReturnType);
                            if (returnTypeOld == "bool")
                            {
                                returnTypeOld = config.GetBoolType();
                            }
                            if (returnTypeOld.Contains('*'))
                            {
                                returnTypeOld = "nint";
                            }
                            string delegateTypeOld;
                            if (cppFunction.Parameters.Count == 0)
                            {
                                delegateTypeOld = $"delegate* unmanaged[{attributes}]<{returnTypeOld}>";
                            }
                            else
                            {
                                delegateTypeOld = $"delegate* unmanaged[{attributes}]<{config.GetNamelessParameterSignature(cppFunction.Parameters, false, true, compatibility: true)}, {returnTypeOld}>";
                            }

                            string argumentNamesOld = config.WriteFunctionMarshalling(cppFunction.Parameters, compatibility: true);

                            if (returnCsName == "void")
                            {
                                writer.WriteLine($"(({delegateTypeOld})funcTable[{funcTableableIndex}])({argumentNamesOld});");
                            }
                            else
                            {
                                writer.WriteLine($"return ({returnType})(({delegateTypeOld})funcTable[{funcTableableIndex}])({argumentNamesOld});");
                            }

                            writer.WriteLine("#endif");

                            writer.EndBlock();
                            break;

                        default:
                            throw new NotSupportedException();
                    }
                    if (modifiers is not null)
                    {
                        if (boolReturn)
                        {
                            writer.WriteLine($"{modifiers} {config.GetBoolType()} {csName}Native({argumentsString});");
                        }
                        else
                        {
                            writer.WriteLine($"{modifiers} {header};");
                        }
                    }
                    writer.WriteLine();

                    overload.Modifiers.Add("public");
                    overload.Modifiers.Add("static");
                    GenerateVariations(cppFunction, overload);
                    WriteFunctions(context, DefinedVariationsFunctions, function, overload, WriteFunctionFlags.None, "public static");
                }
            }

            if (config.UseFunctionTable)
            {
                var initString = FunctionTableBuilder.Finish(out var count);
                if (count == 0) return;
                string filePathfuncTable = Path.Combine(outputPath, "FunctionTable.cs");
                using var writerfuncTable = new CsCodeWriter(filePathfuncTable, config.Namespace, SetupFunctionUsings(), config.HeaderInjector);
                using (writerfuncTable.PushBlock($"public unsafe partial class {config.ApiName}"))
                {
                    writerfuncTable.WriteLine("internal static FunctionTable funcTable;");
                    writerfuncTable.WriteLine();
                    writerfuncTable.WriteLine("/// <summary>");
                    writerfuncTable.WriteLine("/// Initializes the function table, automatically called. Do not call manually, only after <see cref=\"FreeApi\"/>.");
                    writerfuncTable.WriteLine("/// </summary>");

                    if (config.UseCustomContext)
                    {
                        using (writerfuncTable.PushBlock("public static void InitApi(INativeContext context)"))
                        {
                            writerfuncTable.WriteLine($"funcTable = new FunctionTable(context, {count});");
                            writerfuncTable.WriteLines(initString);
                        }
                    }
                    else
                    {
                        using (writerfuncTable.PushBlock("public static void InitApi()"))
                        {
                            writerfuncTable.WriteLine($"funcTable = new FunctionTable(LibraryLoader.LoadLibrary({config.GetLibraryNameFunctionName}, {config.GetLibraryExtensionFunctionName ?? "null"}), {count});");
                            writerfuncTable.WriteLines(initString);
                        }
                    }
                    writerfuncTable.WriteLine();
                    using (writerfuncTable.PushBlock("public static void FreeApi()"))
                    {
                        writerfuncTable.WriteLine("funcTable.Free();");
                    }
                }
            }
        }

        protected virtual void GenerateVariations(CppFunction cppFunction, CsFunctionOverload overload)
        {
            funcGen.GenerateVariations(cppFunction.Parameters, overload);
        }

        public virtual void WriteFunctions(GenContext context, HashSet<CsFunctionVariation> definedFunctions, CsFunction csFunction, CsFunctionOverload overload, WriteFunctionFlags flags, params string[] modifiers)
        {
            foreach (CsFunctionVariation variation in overload.Variations)
            {
                WriteFunctionEx(context, definedFunctions, csFunction, overload, variation, flags, modifiers);

                foreach (var alias in context.ParseResult.EnumerateFunctionAliases(overload.ExportedName))
                {
                    var mapping = config.GetFunctionAliasMapping(alias.ExportedName, alias.ExportedAliasName);
                    if (mapping != null)
                    {
                        if (mapping.FriendlyName != null)
                        {
                            alias.FriendlyName = mapping.FriendlyName;
                        }

                        if (mapping.Comment != null)
                        {
                            alias.Comment = mapping.Comment;
                        }
                    }

                    WriteAlias(context, definedFunctions, csFunction, overload, variation, flags, alias, modifiers);
                }
            }
        }

        public virtual List<IParameterWriter> ParameterWriters { get; } =
        [
            new HandleParameterWriter(),
            new UseThisParameterWriter(),
            new DefaultValueParameterWriter(),
            new StringParameterWriter(),
            new DelegateParameterWriter(),
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

        protected virtual bool WriteFunctionEx(GenContext context, HashSet<CsFunctionVariation> definedFunctions, CsFunction function, CsFunctionOverload overload, CsFunctionVariation variation, WriteFunctionFlags flags, params string[] modifiers)
        {
            var writer = context.Writer;
            CsType csReturnType = variation.ReturnType;
            generator.PrepareArgs(variation, csReturnType);

            string header = variation.BuildFunctionHeader(csReturnType, flags, config.GenerateMetadata);
            variation.BuildFunctionHeaderId(flags);

            if (FilterFunction(context, definedFunctions, variation))
            {
                return false;
            }

            ClassifyParameters(overload, variation, csReturnType, out bool firstParamReturn, out int offset, out bool hasManaged);

            LogInfo("defined function " + header);

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
                    WriteStringConvertToManaged(sb, variation.ReturnType);
                }

                if (flags != WriteFunctionFlags.None)
                {
                    sb.Append($"{config.ApiName}.");
                }

                if (hasManaged)
                {
                    sb.Append($"{overload.Name}(");
                }
                else if (firstParamReturn)
                {
                    sb.Append($"{overload.Name}Native(&ret" + (overload.Parameters.Count > 1 ? ", " : ""));
                }
                else
                {
                    sb.Append($"{overload.Name}Native(");
                }

                FunctionWriterContext writerContext = new(context.Writer, config, sb, overload, variation, flags);

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

            return true;
        }

        protected virtual bool WriteAlias(GenContext context, HashSet<CsFunctionVariation> definedFunctions, CsFunction function, CsFunctionOverload overload, CsFunctionVariation variation, WriteFunctionFlags flags, FunctionAlias alias, params string[] modifiers)
        {
            var writer = context.Writer;
            CsType csReturnType = variation.ReturnType;
            generator.PrepareArgs(variation, csReturnType);

            string header = variation.BuildFunctionHeader(alias.FriendlyName, csReturnType, flags, config.GenerateMetadata);
            variation.BuildFunctionHeaderId(alias.FriendlyName, flags);

            if (FilterFunction(context, definedFunctions, variation))
            {
                return false;
            }

            ClassifyParameters(overload, variation, csReturnType, out bool firstParamReturn, out int offset, out bool hasManaged);

            LogInfo("defined alias function " + header);

            writer.WriteLines(alias.Comment);
            var overloadString = variation.BuildFunctionOverload(flags);

            writer.WriteLine($"{string.Join(" ", modifiers)} {header} => {overloadString};");
            writer.WriteLine();

            return true;
        }

        public static void ClassifyParameters(CsFunctionOverload overload, CsFunctionVariation variation, CsType csReturnType, out bool firstParamReturn, out int offset, out bool hasManaged)
        {
            firstParamReturn = false;
            if (!csReturnType.IsString && csReturnType.Name != overload.ReturnType.Name)
            {
                firstParamReturn = true;
            }

            offset = firstParamReturn ? 1 : 0;
            hasManaged = false;
            for (int j = 0; j < variation.Parameters.Count - offset; j++)
            {
                var cppParameter = variation.Parameters[j + offset];

                if (cppParameter.DefaultValue == null)
                {
                    continue;
                }

                var paramCsDefault = cppParameter.DefaultValue;
                if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
                {
                    hasManaged = true;
                }
            }
        }

        protected static void WriteStringConvertToManaged(StringBuilder sb, CppType type)
        {
            CppPrimitiveKind primitiveKind = type.GetPrimitiveKind();
            if (primitiveKind == CppPrimitiveKind.Char)
            {
                sb.Append("Utils.DecodeStringUTF8(");
            }
            else if (primitiveKind == CppPrimitiveKind.WChar)
            {
                sb.Append("Utils.DecodeStringUTF16(");
            }
            else
            {
                throw new NotSupportedException($"String type ({primitiveKind}) is not supported");
            }
        }

        protected static void WriteStringConvertToManaged(StringBuilder sb, CsType type)
        {
            if (type.StringType == CsStringType.StringUTF8)
            {
                sb.Append("Utils.DecodeStringUTF8(");
            }
            else if (type.StringType == CsStringType.StringUTF16)
            {
                sb.Append("Utils.DecodeStringUTF16(");
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        protected static void WriteStringConvertToManaged(ICodeWriter writer, CppType type, string variable, string pointer)
        {
            CppPrimitiveKind primitiveKind = type.GetPrimitiveKind();
            if (primitiveKind == CppPrimitiveKind.Char)
            {
                writer.WriteLine($"{variable} = Marshal.DecodeStringUTF8({pointer});");
            }
            else if (primitiveKind == CppPrimitiveKind.WChar)
            {
                writer.WriteLine($"{variable} = Marshal.DecodeStringUTF16({pointer});");
            }
            else
            {
                throw new NotSupportedException($"String type ({primitiveKind}) is not supported");
            }
        }

        protected static void WriteStringConvertToManaged(ICodeWriter writer, CsType type, string variable, string pointer)
        {
            if (type.StringType == CsStringType.StringUTF8)
            {
                writer.WriteLine($"{variable} = Utils.DecodeStringUTF8({pointer});");
            }
            else if (type.StringType == CsStringType.StringUTF16)
            {
                writer.WriteLine($"{variable} = Utils.DecodeStringUTF16({pointer});");
            }
            else
            {
                throw new NotSupportedException($"String type ({type.StringType}) is not supported");
            }
        }

        protected static void WriteStringConvertToUnmanaged(ICodeWriter writer, CppType type, string name, int i)
        {
            CppPrimitiveKind primitiveKind = type.GetPrimitiveKind();
            if (primitiveKind == CppPrimitiveKind.Char)
            {
                writer.WriteLine($"byte* pStr{i} = null;");
                writer.WriteLine($"int pStrSize{i} = 0;");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    writer.WriteLine($"pStrSize{i} = Utils.GetByteCountUTF8({name});");
                    using (writer.PushBlock($"if (pStrSize{i} >= Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStr{i} = Utils.Alloc<byte>(pStrSize{i} + 1);");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrStack{i} = stackalloc byte[pStrSize{i} + 1];");
                        writer.WriteLine($"pStr{i} = pStrStack{i};");
                    }
                    writer.WriteLine($"int pStrOffset{i} = Utils.EncodeStringUTF8({name}, pStr{i}, pStrSize{i});");
                    writer.WriteLine($"pStr{i}[pStrOffset{i}] = 0;");
                }
            }
            else if (primitiveKind == CppPrimitiveKind.WChar)
            {
                writer.WriteLine($"char* pStr{i} = null;");
                writer.WriteLine($"int pStrSize{i} = 0;");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    writer.WriteLine($"pStrSize{i} = Utils.GetByteCountUTF16({name});");
                    using (writer.PushBlock($"if (pStrSize{i} >= Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStr{i} = Utils.Alloc<char>(pStrSize{i} + 1);");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrStack{i} = stackalloc byte[pStrSize{i} + 1];");
                        writer.WriteLine($"pStr{i} = (char*)pStrStack{i};");
                    }
                    writer.WriteLine($"int pStrOffset{i} = Utils.EncodeStringUTF16({name}, pStr{i}, pStrSize{i});");
                    writer.WriteLine($"pStr{i}[pStrOffset{i}] = '\\0';");
                }
            }
            else
            {
                throw new NotSupportedException($"String type ({primitiveKind}) is not supported");
            }
        }

        protected static void WriteStringConvertToUnmanaged(ICodeWriter writer, CsType type, string name, int i)
        {
            if (type.StringType == CsStringType.StringUTF8)
            {
                writer.WriteLine($"byte* pStr{i} = null;");
                writer.WriteLine($"int pStrSize{i} = 0;");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    writer.WriteLine($"pStrSize{i} = Utils.GetByteCountUTF8({name});");
                    using (writer.PushBlock($"if (pStrSize{i} >= Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStr{i} = Utils.Alloc<byte>(pStrSize{i} + 1);");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrStack{i} = stackalloc byte[pStrSize{i} + 1];");
                        writer.WriteLine($"pStr{i} = pStrStack{i};");
                    }
                    writer.WriteLine($"int pStrOffset{i} = Utils.EncodeStringUTF8({name}, pStr{i}, pStrSize{i});");
                    writer.WriteLine($"pStr{i}[pStrOffset{i}] = 0;");
                }
            }
            else if (type.StringType == CsStringType.StringUTF16)
            {
                writer.WriteLine($"char* pStr{i} = null;");
                writer.WriteLine($"int pStrSize{i} = 0;");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    writer.WriteLine($"pStrSize{i} = Utils.GetByteCountUTF16({name});");
                    using (writer.PushBlock($"if (pStrSize{i} >= Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStr{i} = Utils.Alloc<char>(pStrSize{i} + 1);");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrStack{i} = stackalloc byte[pStrSize{i} + 1];");
                        writer.WriteLine($"pStr{i} = (char*)pStrStack{i};");
                    }
                    writer.WriteLine($"int pStrOffset{i} = Utils.EncodeStringUTF16({name}, pStr{i}, pStrSize{i});");
                    writer.WriteLine($"pStr{i}[pStrOffset{i}] = '\\0';");
                }
            }
            else
            {
                throw new NotSupportedException($"String type ({type.StringType}) is not supported");
            }
        }

        protected static void WriteFreeString(ICodeWriter writer, int i)
        {
            using (writer.PushBlock($"if (pStrSize{i} >= Utils.MaxStackallocSize)"))
            {
                writer.WriteLine($"Utils.Free(pStr{i});");
            }
        }

        protected static void WriteStringArrayConvertToUnmanaged(ICodeWriter writer, CppType type, string name, int i)
        {
            CppPrimitiveKind primitiveKind = type.GetPrimitiveKind();
            if (primitiveKind == CppPrimitiveKind.Char)
            {
                writer.WriteLine($"byte** pStrArray{i} = null;");
                writer.WriteLine($"int pStrArraySize{i} = Utils.GetByteCountArray({name});");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    using (writer.PushBlock($"if (pStrArraySize{i} > Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStrArray{i} = (byte**)Utils.Alloc<byte>(pStrArraySize{i});");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrArrayStack{i} = stackalloc byte[pStrArraySize{i}];");
                        writer.WriteLine($"pStrArray{i} = (byte**)pStrArrayStack{i};");
                    }
                }
                using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
                {
                    writer.WriteLine($"pStrArray{i}[i] = (byte*)Utils.StringToUTF8Ptr({name}[i]);");
                }
            }
            else if (primitiveKind == CppPrimitiveKind.WChar)
            {
                writer.WriteLine($"char** pStrArray{i} = null;");
                writer.WriteLine($"int pStrArraySize{i} = Utils.GetByteCountArray({name});");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    using (writer.PushBlock($"if (pStrArraySize{i} > Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStrArray{i} = (char**)Utils.Alloc<byte>(pStrArraySize{i});");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrArrayStack{i} = stackalloc byte[pStrArraySize{i}];");
                        writer.WriteLine($"pStrArray{i} = (char**)pStrArrayStack{i};");
                    }
                }
                using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
                {
                    writer.WriteLine($"pStrArray{i}[i] = (char*)Utils.StringToUTF16Ptr({name}[i]);");
                }
            }
            else
            {
                throw new NotSupportedException($"String type ({primitiveKind}) is not supported");
            }
        }

        protected static void WriteStringArrayConvertToUnmanaged(ICodeWriter writer, CsType type, string name, int i)
        {
            if (type.StringType == CsStringType.StringUTF8)
            {
                writer.WriteLine($"byte** pStrArray{i} = null;");
                writer.WriteLine($"int pStrArraySize{i} = Utils.GetByteCountArray({name});");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    using (writer.PushBlock($"if (pStrArraySize{i} > Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStrArray{i} = (byte**)Utils.Alloc<byte>(pStrArraySize{i});");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrArrayStack{i} = stackalloc byte[pStrArraySize{i}];");
                        writer.WriteLine($"pStrArray{i} = (byte**)pStrArrayStack{i};");
                    }
                }
                using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
                {
                    writer.WriteLine($"pStrArray{i}[i] = (byte*)Utils.StringToUTF8Ptr({name}[i]);");
                }
            }
            else if (type.StringType == CsStringType.StringUTF16)
            {
                writer.WriteLine($"char** pStrArray{i} = null;");
                writer.WriteLine($"int pStrArraySize{i} = Utils.GetByteCountArray({name});");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    using (writer.PushBlock($"if (pStrArraySize{i} > Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStrArray{i} = (char**)Utils.Alloc<byte>(pStrArraySize{i});");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrArrayStack{i} = stackalloc byte[pStrArraySize{i}];");
                        writer.WriteLine($"pStrArray{i} = (char**)pStrArrayStack{i};");
                    }
                }
                using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
                {
                    writer.WriteLine($"pStrArray{i}[i] = (char*)Utils.StringToUTF16Ptr({name}[i]);");
                }
            }
            else
            {
                throw new NotSupportedException($"String type ({type.StringType}) is not supported");
            }
        }

        protected static void WriteFreeUnmanagedStringArray(ICodeWriter writer, string name, int i)
        {
            using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
            {
                writer.WriteLine($"Utils.Free(pStrArray{i}[i]);");
            }
            using (writer.PushBlock($"if (pStrArraySize{i} >= Utils.MaxStackallocSize)"))
            {
                writer.WriteLine($"Utils.Free(pStrArray{i});");
            }
        }

        protected static void WriteFreeUnmanagedStringArray(ICodeWriter writer, string name, string varName)
        {
            using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
            {
                writer.WriteLine($"Utils.Free({varName}[i]);");
            }
            using (writer.PushBlock($"if ({varName}Size >= Utils.MaxStackallocSize)"))
            {
                writer.WriteLine($"Utils.Free({varName});");
            }
        }
    }
}