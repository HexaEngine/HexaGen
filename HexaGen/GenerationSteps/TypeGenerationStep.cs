﻿namespace HexaGen.GenerationSteps
{
    using HexaGen.Core;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Types;
    using HexaGen.FunctionGeneration;
    using HexaGen.Metadata;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class TypeGenerationStep : GenerationStep
    {
        protected readonly HashSet<string> LibDefinedTypes = new();
        public readonly HashSet<string> DefinedTypes = new();
        public readonly Dictionary<string, string> WrappedPointers = new();
        public readonly Dictionary<string, HashSet<CsFunctionVariation>> MemberFunctions = new();

        protected FunctionGenerator funcGen = null!;

        public TypeGenerationStep(CsCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
        }

        public override string Name { get; } = "Types";

        public override void Configure(CsCodeGeneratorConfig config)
        {
            Enabled = config.GenerateTypes;
            funcGen = generator.FunctionGenerator;
        }

        public override void CopyToMetadata(CsCodeGeneratorMetadata metadata)
        {
            metadata.DefinedTypes.AddRange(DefinedTypes);
            metadata.WrappedPointers.AddRange(WrappedPointers);
        }

        public override void CopyFromMetadata(CsCodeGeneratorMetadata metadata)
        {
            LibDefinedTypes.AddRange(metadata.DefinedTypes);
            WrappedPointers.AddRange(metadata.WrappedPointers);
        }

        public override void Reset()
        {
            LibDefinedTypes.Clear();
            DefinedTypes.Clear();
            WrappedPointers.Clear();
            MemberFunctions.Clear();
        }

        protected virtual List<string> SetupTypeUsings()
        {
            List<string> usings =
            [
                "System", "System.Diagnostics", "System.Runtime.CompilerServices", "System.Runtime.InteropServices", "HexaGen.Runtime",
                .. config.Usings,
            ];
            return usings;
        }

        protected virtual bool FilterType(GenContext? context, CppClass cppClass, out TypeMapping? mapping, [NotNullWhen(false)] out string? csName, bool bypassDef = false)
        {
            csName = null;
            mapping = null;
            if (config.AllowedTypes.Count != 0 && !config.AllowedTypes.Contains(cppClass.Name))
                return true;
            if (config.IgnoredTypes.Contains(cppClass.Name))
                return true;
            if (LibDefinedTypes.Contains(cppClass.Name))
                return true;

            if (DefinedTypes.Contains(cppClass.Name) && !bypassDef)
            {
                LogWarn($"{context?.FilePath}: {cppClass} is already defined!");
                return true;
            }

            if (!bypassDef)
                DefinedTypes.Add(cppClass.Name);

            csName = config.GetCsCleanName(cppClass.Name);
            mapping = config.GetTypeMapping(cppClass.Name);
            csName = mapping?.FriendlyName ?? csName;

            if (cppClass.ClassKind == CppClassKind.Class || cppClass.Name.EndsWith("_T") || csName == "void")
            {
                return true;
            }

            return false;
        }

        public override void Generate(FileSet files, ParseResult result, string outputPath, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata)
        {
            var compilation = result.Compilation;
            string folder = Path.Combine(outputPath, "Structs");
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            Directory.CreateDirectory(folder);

            MemberFunctions.Clear();

            if (config.OneFilePerType)
            {
                // Generate Structures

                for (int i = 0; i < compilation.Classes.Count; i++)
                {
                    CppClass cppClass = compilation.Classes[i];
                    if (!files.Contains(cppClass.SourceFile))
                        continue;

                    if (FilterType(null, cppClass, out var mapping, out var csNameDefault))
                        continue;
                    string filePath = Path.Combine(folder, $"{csNameDefault}.cs");
                    using var writer = new CsCodeWriter(filePath, config.Namespace, SetupTypeUsings(), config.HeaderInjector);
                    GenContext context = new(result, filePath, writer);

                    WriteClass(context, cppClass, mapping, csNameDefault);

                    if (config.WrapPointersAsHandle)
                    {
                        WriteHandle(context, cppClass);
                    }
                }
            }
            else
            {
                string filePath = Path.Combine(folder, "Structs.cs");

                // Generate Structures
                using var writer = new CsSplitCodeWriter(filePath, config.Namespace, SetupTypeUsings(), config.HeaderInjector, 1);
                GenContext context = new(result, filePath, writer);

                // Print All classes, structs
                for (int i = 0; i < compilation.Classes.Count; i++)
                {
                    CppClass cppClass = compilation.Classes[i];
                    if (!files.Contains(cppClass.SourceFile))
                        continue;

                    if (FilterType(context, cppClass, out var mapping, out var csNameDefault))
                        continue;

                    WriteClass(context, cppClass, mapping, csNameDefault);

                    if (config.WrapPointersAsHandle)
                    {
                        WriteHandle(context, cppClass);
                    }
                }
            }
        }

        protected virtual void WriteClass(GenContext context, CppClass cppClass, TypeMapping? mapping, string csName)
        {
            var writer = context.Writer;

            bool isReadOnly = false;
            string modifier = "partial";

            LogInfo("defined struct " + csName);

            bool commentWritten = false;
            if (mapping != null && mapping.Comment != null)
            {
                commentWritten = config.WriteCsSummary(mapping.Comment, writer);
            }
            else
            {
                commentWritten = config.WriteCsSummary(cppClass.Comment, writer);
            }

            if (config.GenerateMetadata)
            {
                writer.WriteLine($"[NativeName(NativeNameType.StructOrClass, \"{cppClass.FullName}\")]");
            }

            bool isUnion = cppClass.ClassKind == CppClassKind.Union;

            if (isUnion)
            {
                writer.WriteLine("[StructLayout(LayoutKind.Explicit)]");
            }
            else
            {
                writer.WriteLine("[StructLayout(LayoutKind.Sequential)]");
            }

            using (writer.PushBlock($"public {modifier} struct {csName}"))
            {
                if (config.GenerateSizeOfStructs && cppClass.SizeOf > 0)
                {
                    writer.WriteLine("/// <summary>");
                    writer.WriteLine($"/// The size of the <see cref=\"{csName}\"/> type, in bytes.");
                    writer.WriteLine("/// </summary>");
                    writer.WriteLine($"public static readonly int SizeInBytes = {cppClass.SizeOf};");
                    writer.WriteLine();
                }

                List<CsSubClass> subClasses = new();
                for (int j = 0; j < cppClass.Classes.Count; j++)
                {
                    var subClass = cppClass.Classes[j];
                    var csSubName = config.GetCsSubTypeName(cppClass, csName, subClass, j);
                    var subClassMapping = config.GetTypeMapping(subClass.FullName);

                    csSubName = subClassMapping?.FriendlyName ?? csSubName;

                    WriteClass(context, subClass, subClassMapping, csSubName);
                    subClasses.Add(new(subClass, csSubName, cppClass.Name, $"Union{(cppClass.Classes.Count == 1 ? "" : j.ToString())}"));
                }

                List<CsPropertyMetadata> properties = [];
                int bitfieldCount = 0;
                for (int j = 0; j < cppClass.Fields.Count; j++)
                {
                    CppField cppField = cppClass.Fields[j];

                    if (cppField.IsBitField)
                    {
                        GenerateBitfields(context, cppClass, csName, ref j, properties, ref bitfieldCount);
                        continue;
                    }

                    var fieldMapping = mapping?.GetFieldMapping(cppField.Name);
                    if (cppField.Type is CppClass cppClass1 && cppClass1.ClassKind == CppClassKind.Union && cppClass1.FullParentName == cppClass.FullName)
                    {
                        var fieldCommentWritten = config.WriteCsSummary(cppField.Comment, writer);
                        if (!fieldCommentWritten)
                        {
                            fieldCommentWritten = config.WriteCsSummary(fieldMapping?.Comment, writer);
                        }

                        if (config.GenerateMetadata)
                        {
                            writer.WriteLine($"[NativeName(NativeNameType.Field, \"{cppField.Name}\")]");
                            writer.WriteLine($"[NativeName(NativeNameType.Type, \"{cppField.Type.GetDisplayName()}\")]");
                        }

                        var subClass = subClasses.FirstOrDefault(x => x.CppType == cppClass1);

                        if (isUnion)
                        {
                            writer.WriteLine("[FieldOffset(0)]");
                        }
                        if (subClass == null)
                        {
                            string csFieldName = config.GetFieldName(cppField.Name);
                            string csFieldType = config.GetCsCleanName(cppClass1.Name);
                            subClasses.Add(new(cppField.Type, csFieldType, cppField.Name, csFieldName));

                            writer.WriteLine($"public {csFieldType} {csFieldName};");
                            if (fieldCommentWritten)
                            {
                                writer.WriteLine();
                            }

                            continue;
                        }

                        writer.WriteLine($"public {subClass.Name} {subClass.FieldName};");

                        if (fieldCommentWritten)
                            writer.WriteLine();
                    }
                    else if (cppField.Type is CppPointerType cppPointer && cppPointer.IsDelegate(out var cppFunctionType))
                    {
                        var fieldCommentWritten = config.WriteCsSummary(cppField.Comment, writer);
                        if (!fieldCommentWritten)
                        {
                            fieldCommentWritten = config.WriteCsSummary(fieldMapping?.Comment, writer);
                        }

                        if (config.GenerateMetadata)
                        {
                            writer.WriteLine($"[NativeName(NativeNameType.Field, \"{cppField.Name}\")]");
                            writer.WriteLine($"[NativeName(NativeNameType.Type, \"{cppField.Type.GetDisplayName()}\")]");
                        }

                        string csFieldName = config.GetFieldName(cppField.Name);
                        string returnCsName = config.GetCsTypeName(cppFunctionType.ReturnType, false);
                        string signature = config.GetNamelessParameterSignature(cppFunctionType.Parameters, false);
                        returnCsName = returnCsName.Replace("bool", config.GetBoolType());
                        if (config.DelegatesAsVoidPointer)
                        {
                            writer.WriteLine($"public unsafe void* {csFieldName};");
                        }
                        else
                        {
                            writer.WriteLine($"public unsafe delegate* unmanaged[{cppFunctionType.CallingConvention.GetCallingConventionDelegate()}]<{signature}, {returnCsName}> {csFieldName};");
                        }

                        if (fieldCommentWritten)
                            writer.WriteLine();
                    }
                    else
                    {
                        WriteField(writer, cppField, fieldMapping, subClasses, isUnion, isReadOnly);
                    }
                }

                writer.WriteLine();

                HashSet<string> definedConstructors = new();

                if (config.GenerateConstructorsForStructs && cppClass.Fields.Count > 0)
                {
                    config.WriteCsSummary((string?)null, out string? comment);
                    CsFunction function = new(csName, comment);
                    CsFunctionOverload overload = new(string.Empty, csName, comment, csName, CsFunctionKind.Constructor, new(string.Empty, CppPrimitiveKind.Void));
                    function.Overloads.Add(overload);
                    for (int j = 0; j < cppClass.Fields.Count; j++)
                    {
                        var cppField = cppClass.Fields[j];
                        var csFieldName = config.GetFieldName(cppField.Name);
                        var paramCsTypeName = config.GetCsTypeName(cppField.Type, false);
                        var paramCsName = config.GetParameterName(j, cppField.Name);
                        var direction = cppField.Type.GetDirection();
                        var kind = cppField.Type.GetPrimitiveKind();

                        var subClass = subClasses.FirstOrDefault(x => x.CppType == cppField.Type);

                        if (subClass != null && cppField.Type is CppClass cppClass1 && cppClass1.ClassKind == CppClassKind.Union)
                        {
                            subClass = subClasses.First(x => x.CppType == cppClass1);
                            paramCsTypeName = subClass.Name;
                            paramCsName = subClass.FieldName.ToLower();
                            csFieldName = subClass.FieldName;
                        }

                        if (cppField.Type is CppArrayType arrayType && arrayType.ElementType is CppPointerType pointerType && pointerType.ElementType is CppFunctionType && config.DelegatesAsVoidPointer)
                        {
                            paramCsTypeName = "nint*";
                        }

                        if (subClass != null)
                        {
                            paramCsTypeName = subClass.Name;
                        }

                        int depth = 0;
                        var subClass1 = subClasses.FirstOrDefault(x => x.CppType.IsPointerOf(cppField.Type, ref depth));
                        if (subClass1 != null)
                        {
                            paramCsTypeName = subClass1.Name + new string('*', depth);
                        }

                        CsType csType = new(paramCsTypeName, kind);

                        CsParameterInfo csParameter = new(paramCsName, cppField.Type, csType, direction, "default", csFieldName);
                        overload.DefaultValues.TryAdd(paramCsName, "default");
                        overload.Parameters.Add(csParameter);
                    }

                    funcGen.GenerateConstructorVariations(cppClass, subClasses, csName, overload);
                    WriteConstructors(context, definedConstructors, function, overload, cppClass.Fields, properties, subClasses, "public unsafe");
                }

                writer.WriteLine();

                foreach (var property in properties)
                {
                    writer.WriteLine($"public {property.Type.Name} {property.Name} {{ {property.Getter} {property.Setter} }}");
                    writer.WriteLine();
                }

                for (int j = 0; j < cppClass.Fields.Count; j++)
                {
                    CppField cppField = cppClass.Fields[j];
                    if (cppField.Type.TypeKind == CppTypeKind.Array)
                    {
                        WriteProperty(writer, cppField);
                    }
                }

                if (config.KnownMemberFunctions.TryGetValue(cppClass.Name, out var functions))
                {
                    WriteMemberFunctions(context, cppClass, functions, WriteFunctionFlags.UseThis);
                }
            }

            writer.WriteLine();
        }

        private void GenerateBitfields(GenContext context, CppClass cppClass, string csClassName, ref int fieldIndex, List<CsPropertyMetadata> properties, ref int bitfieldCount)
        {
            var writer = context.Writer;
            var fields = cppClass.Fields;
            var baseOffset = fields[fieldIndex].BitOffset;
            var baseType = fields[fieldIndex].Type;
            int storageBitSize = baseType.SizeOf * 8;
            var csBaseType = new CsType(config.GetCsTypeName(baseType), baseType.IsEnum(), baseType.GetPrimitiveKind());

            var bitfieldId = bitfieldCount++;
            var bitfieldCsName = $"RawBits{bitfieldId}";
            writer.WriteLine($"public {csBaseType.Name} {bitfieldCsName};");

            int bitfieldOffset = 0;
            int i = fieldIndex;
            for (; i < fields.Count; ++i)
            {
                var field = fields[i];
                if (!field.IsBitField)
                {
                    break;
                }

                if (!field.Type.IsType(baseType))
                {
                    break;
                }

                var bitfieldOffsetNext = bitfieldOffset + field.BitFieldWidth;
                if (bitfieldOffsetNext > storageBitSize)
                {
                    break;
                }
                var csFieldName = config.GetFieldName(field.Name);
                CsPropertyMetadata property = new(baseType, csBaseType, csFieldName, $"get => Bitfield.Get({bitfieldCsName}, {field.BitOffset - baseOffset}, {field.BitFieldWidth});", $"set => Bitfield.Set(ref {bitfieldCsName}, value, {field.BitOffset - baseOffset}, {field.BitFieldWidth});");
                properties.Add(property);
                bitfieldOffset = bitfieldOffsetNext;
            }

            fieldIndex = i - 1;
        }

        protected virtual void WriteHandle(GenContext context, CppClass cppClass)
        {
            if (FilterType(context, cppClass, out var mapping, out var csName, true))
                return;

            if (cppClass.IsUsedAsPointer(context.Compilation, out var depths))
            {
                for (int j = 0; j < depths.Count; j++)
                {
                    int depth = depths[j];
                    LogDebug("used as pointer: " + cppClass.Name + ", depth: " + depth);
                    StringBuilder sb1 = new();
                    StringBuilder sb2 = new();
                    sb1.Append(csName);
                    sb2.Append(csName);
                    for (int jj = 0; jj < depth; jj++)
                    {
                        sb1.Append("Ptr");
                        sb2.Append('*');
                    }

                    WriteStructHandle(context, cppClass, mapping, sb1.ToString(), sb2.ToString());
                    WrappedPointers.Add(sb2.ToString(), sb1.ToString());
                }
            }
        }

        protected virtual bool FilterConstructor(GenContext context, HashSet<string> definedFunctions, string header)
        {
            if (definedFunctions.Contains(header))
            {
                LogWarn($"{context.FilePath}: {header} constructor is already defined!");
                return true;
            }
            definedFunctions.Add(header);
            return false;
        }

        protected virtual void WriteConstructors(GenContext context, HashSet<string> definedFunctions, CsFunction csFunction, CsFunctionOverload overload, IList<CppField> fields, List<CsPropertyMetadata> properties, List<CsSubClass> subClasses, params string[] modifiers)
        {
            for (int j = 0; j < overload.Variations.Count; j++)
            {
                WriteConstructor(context, definedFunctions, csFunction, overload, overload.Variations[j], fields, subClasses, properties, modifiers);
            }
        }

        protected virtual void WriteConstructor(GenContext context, HashSet<string> definedFunctions, CsFunction function, CsFunctionOverload overload, CsFunctionVariation variation, IList<CppField> fields, List<CsSubClass> subClasses, List<CsPropertyMetadata> properties, params string[] modifiers)
        {
            var writer = context.Writer;
            CsType returnType = variation.ReturnType;
            generator.PrepareArgs(variation, returnType);

            string header = variation.BuildFullConstructorSignature(config.GenerateMetadata);
            string id = variation.BuildConstructorSignatureIdentifier();

            if (FilterConstructor(context, definedFunctions, id))
            {
                return;
            }

            CsCodeGenerator.ClassifyParameters(overload, variation, returnType, out _, out _, out _);

            LogInfo("defined constructor " + header);

            writer.WriteLines(overload.Comment);
            if (config.GenerateMetadata)
            {
                writer.WriteLines(overload.Attributes);
            }

            using (writer.PushBlock($"{string.Join(" ", modifiers)} {header}"))
            {
                for (int i = 0; i < overload.Parameters.Count; i++)
                {
                    var cppParameter = overload.Parameters[i];
                    var cppField = fields[i];

                    ParameterFlags paramFlags = ParameterFlags.None;

                    if (variation.TryGetParameter(cppParameter.Name, out var param))
                    {
                        cppParameter = param;
                        paramFlags = param.Flags;
                    }

                    var fieldName = cppParameter.FieldName;

                    if (string.IsNullOrEmpty(fieldName))
                    {
                        var subClass = subClasses.First(x => x.CppType == cppField.Type);
                        fieldName = subClass.Name;
                    }

                    if (fieldName == cppParameter.Name)
                    {
                        fieldName = $"this.{fieldName}";
                    }

                    bool targetsProperty = false;
                    if (properties.Any(x => x.Name == cppParameter.FieldName))
                    {
                        targetsProperty = true;
                    }

                    // skip array field types.
                    // TODO: Add support for array field types.
                    if (cppField.Type is CppArrayType arrayType)
                    {
                        using (writer.PushBlock($"if ({cppParameter.Name} != default({cppParameter.Type.Name}))"))
                        {
                            for (int j = 0; j < arrayType.Size; j++)
                            {
                                writer.WriteLine($"{fieldName}_{j} = {cppParameter.Name}[{j}];");
                            }
                        }
                    }
                    else if (cppField.Type.IsDelegate(out var cppFunction))
                    {
                        int depth = 0;
                        cppField.Type.IsPointer(ref depth);
                        string delegateType = $"({config.MakeDelegatePointer(cppFunction, false)}{new string('*', depth)})";
                        if (cppParameter.Type.Name.StartsWith("delegate*<"))
                        {
                            writer.WriteLine($"{fieldName} = {delegateType}{cppParameter.Name};");
                        }
                        else
                        {
                            writer.WriteLine($"{fieldName} = {delegateType}Marshal.GetFunctionPointerForDelegate({cppParameter.Name});");
                        }
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Bool) && !targetsProperty && !paramFlags.HasFlag(ParameterFlags.Ref) && !paramFlags.HasFlag(ParameterFlags.Pointer))
                    {
                        writer.WriteLine($"{fieldName} = {cppParameter.Name} ? ({config.GetBoolType()})1 : ({config.GetBoolType()})0;");
                    }
                    else
                    {
                        writer.WriteLine($"{fieldName} = {cppParameter.Name};");
                    }
                }
            }

            writer.WriteLine();
        }

        private void WriteField(ICodeWriter writer, CppField field, TypeFieldMapping? mapping, List<CsSubClass> subClasses, bool isUnion = false, bool isReadOnly = false)
        {
            string csFieldName = config.GetFieldName(field.Name);

            var fieldCommentWritten = config.WriteCsSummary(field.Comment, writer);
            if (!fieldCommentWritten)
            {
                fieldCommentWritten = config.WriteCsSummary(mapping?.Comment, writer);
            }

            if (config.GenerateMetadata)
            {
                writer.WriteLine($"[NativeName(NativeNameType.Field, \"{field.Name}\")]");
                writer.WriteLine($"[NativeName(NativeNameType.Type, \"{field.Type.GetDisplayName()}\")]");
            }

            if (isUnion)
            {
                writer.WriteLine("[FieldOffset(0)]");
            }

            if (field.Type is CppArrayType arrayType)
            {
                string csFieldType = config.GetCsTypeName(arrayType.ElementType);

                if (arrayType.ElementType is CppPointerType pointerType && pointerType.ElementType is CppFunctionType functionType && config.DelegatesAsVoidPointer)
                {
                    csFieldType = "nint";
                }

                string unsafePrefix = string.Empty;

                if (csFieldType.Contains('*'))
                {
                    unsafePrefix = "unsafe ";
                }

                for (int i = 0; i < arrayType.Size; i++)
                {
                    if (isUnion && i != 0)
                    {
                        writer.WriteLine($"[FieldOffset({arrayType.SizeOf * i})]");
                    }
                    writer.WriteLine($"public {unsafePrefix}{csFieldType} {csFieldName}_{i};");
                }
            }
            else
            {
                string csFieldType = config.GetCsTypeName(field.Type, false);

                var subClass = subClasses.Find(x => x.CppType == field.Type);
                if (subClass != null)
                {
                    csFieldType = subClass.Name;
                }

                int depth = 0;
                var subClass1 = subClasses.FirstOrDefault(x => x.CppType.IsPointerOf(field.Type, ref depth));
                if (subClass1 != null)
                {
                    csFieldType = subClass1.Name + new string('*', depth);
                }

                string fieldPrefix = isReadOnly ? "readonly " : string.Empty;

                if (csFieldType == "bool")
                    csFieldType = config.GetBoolType();

                if (field.Type is CppTypedef typedef &&
                    typedef.ElementType is CppPointerType pointerType &&
                    pointerType.ElementType is CppFunctionType functionType)
                {
                    StringBuilder builder = new();
                    for (int i = 0; i < functionType.Parameters.Count; i++)
                    {
                        CppParameter parameter = functionType.Parameters[i];
                        string paramCsType = config.GetCsTypeName(parameter.Type, false);
                        // Otherwise we get interop issues with non blittable types

                        builder.Append(paramCsType);

                        builder.Append(", ");
                    }

                    string returnCsName = config.GetCsTypeName(functionType.ReturnType, false);
                    returnCsName = returnCsName.Replace("bool", config.GetBoolType());
                    builder.Append(returnCsName);

                    if (config.DelegatesAsVoidPointer)
                    {
                        writer.WriteLine($"public {fieldPrefix}unsafe void* {csFieldName};");
                    }
                    else
                    {
                        writer.WriteLine($"public {fieldPrefix}unsafe delegate* unmanaged[{functionType.CallingConvention.GetCallingConventionDelegate()}]<{builder}> {csFieldName};");
                    }

                    return;
                }

                if (csFieldType.EndsWith('*'))
                {
                    fieldPrefix += "unsafe ";
                }

                writer.WriteLine($"public {fieldPrefix}{csFieldType} {csFieldName};");
            }

            if (fieldCommentWritten)
                writer.WriteLine();
        }

        private void WriteProperty(ICodeWriter writer, CppClass cppClass, string classCsName, CppField field, TypeFieldMapping? mapping, bool isUnion = false, bool isReadOnly = false)
        {
            string csFieldName = config.GetFieldName(field.Name);

            var fieldCommentWritten = config.WriteCsSummary(field.Comment, writer);
            if (!fieldCommentWritten)
            {
                fieldCommentWritten = config.WriteCsSummary(mapping?.Comment, writer);
            }

            if (field.Type is CppArrayType arrayType)
            {
                string csFieldType = config.GetCsTypeName(arrayType.ElementType, false);

                if (arrayType.ElementType is CppTypedef typedef && typedef.IsPrimitive(out var primitive))
                {
                    csFieldType = config.GetCsTypeName(primitive, false);
                }

                if (csFieldType.EndsWith('*'))
                {
                    return;
                }

                writer.WriteLine($"public unsafe Span<{csFieldType}> {csFieldName}");
                using (writer.PushBlock(""))
                {
                    using (writer.PushBlock("get"))
                    {
                        writer.WriteLine($"return new Span<{csFieldType}>(&Handle->{csFieldName}_0, {arrayType.Size});");
                    }
                }
            }
            else
            {
                string csFieldType;

                if (field.Type is CppClass subClass)
                {
                    int index = cppClass.Classes.IndexOf(subClass);
                    if (index != -1)
                    {
                        csFieldType = classCsName.Replace("*", "") + "." + config.GetCsSubTypeName(cppClass, classCsName.Replace("*", ""), subClass, index);
                        csFieldName = $"Union{(cppClass.Classes.Count == 1 ? "" : index.ToString())}";
                    }
                    else
                    {
                        csFieldType = config.GetCsTypeName(field.Type, false);
                    }
                }
                else
                {
                    csFieldType = config.GetCsTypeName(field.Type, false);
                }

                if (field.Type.IsDelegate(out var functionType))
                {
                    StringBuilder builder = new();
                    for (int i = 0; i < functionType.Parameters.Count; i++)
                    {
                        CppParameter parameter = functionType.Parameters[i];
                        string paramCsType = config.GetCsTypeName(parameter.Type, false);
                        // Otherwise we get interop issues with non blittable types

                        builder.Append(paramCsType);

                        builder.Append(", ");
                    }

                    string returnCsName = config.GetCsTypeName(functionType.ReturnType, false);
                    returnCsName = returnCsName.Replace("bool", config.GetBoolType());
                    builder.Append(returnCsName);

                    if (config.DelegatesAsVoidPointer)
                    {
                        if (isReadOnly)
                        {
                            writer.WriteLine($"public void* {csFieldName} {{ get => Handle->{csFieldName}; }}");
                        }
                        else
                        {
                            writer.WriteLine($"public void* {csFieldName} {{ get => Handle->{csFieldName}; set => Handle->{csFieldName} = value; }}");
                        }
                    }
                    else
                    {
                        if (isReadOnly)
                        {
                            writer.WriteLine($"public delegate* unmanaged[{functionType.CallingConvention.GetCallingConventionDelegate()}]<{builder}> {csFieldName} {{ get => Handle->{csFieldName}; }}");
                        }
                        else
                        {
                            writer.WriteLine($"public delegate* unmanaged[{functionType.CallingConvention.GetCallingConventionDelegate()}]<{builder}> {csFieldName} {{ get => Handle->{csFieldName}; set => Handle->{csFieldName} = value; }}");
                        }
                    }

                    return;
                }

                if (csFieldType.EndsWith('*') && !CsType.IsKnownPrimitive(csFieldType))
                {
                    StringBuilder sb = new();
                    int x = 0;
                    while (csFieldType.EndsWith('*'))
                    {
                        csFieldType = csFieldType[..^1];
                        x++;
                    }
                    sb.Append(csFieldType);
                    for (int j = 0; j < x; j++)
                    {
                        sb.Append("Ptr");
                    }
                    csFieldType = sb.ToString();
                }

                if (csFieldType.EndsWith('*') || field.IsBitField)
                {
                    if (isReadOnly)
                    {
                        writer.WriteLine($"public {csFieldType} {csFieldName} => Handle->{csFieldName};");
                    }
                    else
                    {
                        writer.WriteLine($"public {csFieldType} {csFieldName} {{ get => Handle->{csFieldName}; set => Handle->{csFieldName} = value; }}");
                    }
                }
                else
                {
                    if (isReadOnly)
                    {
                        writer.WriteLine($"public {csFieldType} {csFieldName} => Handle->{csFieldName};");
                    }
                    else
                    {
                        writer.WriteLine($"public ref {csFieldType} {csFieldName} => ref Unsafe.AsRef<{csFieldType}>(&Handle->{csFieldName});");
                    }
                }
            }
        }

        private void WriteProperty(ICodeWriter writer, CppField field)
        {
            string csFieldName = config.GetFieldName(field.Name);

            if (field.Type is CppArrayType arrayType)
            {
                string csFieldType = config.GetCsTypeName(arrayType.ElementType, false);
                string spanType = csFieldType;
                bool canUseFixed = false;
                if (arrayType.ElementType is CppPrimitiveType)
                {
                    canUseFixed = true;
                }
                else if (arrayType.ElementType is CppTypedef typedef && typedef.IsPrimitive(out var primitive))
                {
                    csFieldType = config.GetCsTypeName(primitive, false);
                    canUseFixed = true;
                }

                if (canUseFixed)
                {
                }
                else
                {
                    if (csFieldType.Contains("delegate*"))
                    {
                        if (config.DelegatesAsVoidPointer)
                        {
                            csFieldType = "nint";
                        }
                        spanType = "nint";
                    }
                    else if (csFieldType.Contains('*'))
                    {
                        spanType = $"Pointer<{csFieldType.Replace("*", "")}>";
                    }

                    config.WriteCsSummary(field.Comment, writer);
                    writer.WriteLine($"public unsafe Span<{spanType}> {csFieldName}");
                    using (writer.PushBlock(""))
                    {
                        using (writer.PushBlock("get"))
                        {
                            using (writer.PushBlock($"fixed ({csFieldType}* p = &this.{csFieldName}_0)"))
                            {
                                writer.WriteLine($"return new Span<{spanType}>(p, {arrayType.Size});");
                            }
                        }
                    }
                }
            }
        }

        private void WriteStructHandle(GenContext context, CppClass cppClass, TypeMapping? mapping, string csName, string handleType)
        {
            var writer = context.Writer;
            bool isUnion = cppClass.ClassKind == CppClassKind.Union;
            LogInfo("defined handle " + csName);
            config.WriteCsSummary(cppClass.Comment, writer);
            if (config.GenerateMetadata)
            {
                writer.WriteLine($"[NativeName(NativeNameType.Typedef, \"{cppClass.Name}\")]");
            }
            writer.WriteLine("#if NET5_0_OR_GREATER");
            writer.WriteLine($"[DebuggerDisplay(\"{{DebuggerDisplay,nq}}\")]");
            writer.WriteLine("#endif");
            using (writer.PushBlock($"public unsafe struct {csName} : IEquatable<{csName}>"))
            {
                string nullValue = "null";

                writer.WriteLine($"public {csName}({handleType} handle) {{ Handle = handle; }}");
                writer.WriteLine();
                writer.WriteLine($"public {handleType} Handle;");
                writer.WriteLine();
                writer.WriteLine($"public bool IsNull => Handle == null;");
                writer.WriteLine();
                writer.WriteLine($"public static {csName} Null => new {csName}({nullValue});");
                writer.WriteLine();
                writer.WriteLine($"public {handleType.AsSpan().TrimEndFirstOccurrence('*')} this[int index] {{ get => Handle[index]; set => Handle[index] = value; }}");
                writer.WriteLine();
                writer.WriteLine($"public static implicit operator {csName}({handleType} handle) => new {csName}(handle);");
                writer.WriteLine();
                writer.WriteLine($"public static implicit operator {handleType}({csName} handle) => handle.Handle;");
                writer.WriteLine();
                writer.WriteLine($"public static bool operator ==({csName} left, {csName} right) => left.Handle == right.Handle;");
                writer.WriteLine();
                writer.WriteLine($"public static bool operator !=({csName} left, {csName} right) => left.Handle != right.Handle;");
                writer.WriteLine();
                writer.WriteLine($"public static bool operator ==({csName} left, {handleType} right) => left.Handle == right;");
                writer.WriteLine();
                writer.WriteLine($"public static bool operator !=({csName} left, {handleType} right) => left.Handle != right;");
                writer.WriteLine();
                writer.WriteLine($"public bool Equals({csName} other) => Handle == other.Handle;");
                writer.WriteLine();
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override bool Equals(object obj) => obj is {csName} handle && Equals(handle);");
                writer.WriteLine();
                writer.WriteLine("/// <inheritdoc/>");
                writer.WriteLine($"public override int GetHashCode() => ((nuint)Handle).GetHashCode();");
                writer.WriteLine();
                writer.WriteLine("#if NET5_0_OR_GREATER");
                writer.WriteLine($"private string DebuggerDisplay => string.Format(\"{csName} [0x{{0}}]\", ((nuint)Handle).ToString(\"X\"));");
                writer.WriteLine("#endif");
                var pCount = handleType.Count(x => x == '*');
                if (pCount == 1)
                {
                    for (int j = 0; j < cppClass.Fields.Count; j++)
                    {
                        CppField cppField = cppClass.Fields[j];
                        var fieldMapping = mapping?.GetFieldMapping(cppField.Name);
                        WriteProperty(writer, cppClass, handleType, cppField, fieldMapping, isUnion, false);
                    }

                    if (config.KnownMemberFunctions.TryGetValue(cppClass.Name, out var functions))
                    {
                        WriteMemberFunctions(context, cppClass, functions, WriteFunctionFlags.UseHandle);
                    }
                }
            }
            writer.WriteLine();
        }

        private void WriteMemberFunctions(GenContext context, CppClass cppClass, List<string> functions, WriteFunctionFlags flags)
        {
            HashSet<CsFunctionVariation> definedFunctions = new(IdentifierComparer<CsFunctionVariation>.Default);
            List<CsFunction> commands = new();
            for (int i = 0; i < functions.Count; i++)
            {
                CppFunction cppFunction = CsCodeGenerator.FindFunction(context.Compilation, functions[i]);
                var csFunctionName = config.GetCsFunctionName(cppFunction.Name);

                CsFunction function = generator.CreateCsFunction(cppFunction, CsFunctionKind.Member, csFunctionName, commands, out var overload);
                funcGen.GenerateVariations(cppFunction.Parameters, overload);

                bool useThisRef = false;
                if (cppFunction.Parameters.Count > 0 && cppClass.IsPointerOf(cppFunction.Parameters[0].Type))
                {
                    useThisRef = true;
                }

                bool useThis = false;
                if (cppFunction.Parameters.Count > 0 && cppClass.IsType(cppFunction.Parameters[0].Type))
                {
                    useThis = true;
                }

                if (useThis || useThisRef)
                {
                    GetGenerationStep<FunctionGenerationStep>().WriteFunctions(context, definedFunctions, function, overload, flags, "public unsafe");
                }
            }

            if (flags == WriteFunctionFlags.UseThis)
            {
                if (MemberFunctions.TryGetValue(cppClass.Name, out var funcs))
                {
                    foreach (var f in definedFunctions)
                    {
                        funcs.Add(f);
                    }
                }
                else
                {
                    MemberFunctions.Add(cppClass.Name, definedFunctions);
                }
            }
        }
    }
}