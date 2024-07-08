namespace HexaGen
{
    using CppAst;
    using HexaGen.Core;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Text;

    public partial class CsCodeGenerator
    {
        protected readonly HashSet<string> LibDefinedTypes = new();
        public readonly HashSet<string> DefinedTypes = new();
        protected readonly Dictionary<string, string> WrappedPointers = new();
        protected readonly Dictionary<string, HashSet<string>> MemberFunctions = new();

        protected virtual List<string> SetupTypeUsings()
        {
            List<string> usings = new() { "System", "System.Diagnostics", "System.Runtime.CompilerServices", "System.Runtime.InteropServices", "HexaGen.Runtime" };
            usings.AddRange(settings.Usings);
            return usings;
        }

        protected virtual bool FilterType(GenContext context, CppClass cppClass, out TypeMapping? mapping, [NotNullWhen(false)] out string? csName, bool bypassDef = false)
        {
            csName = null;
            mapping = null;
            if (settings.AllowedTypes.Count != 0 && !settings.AllowedTypes.Contains(cppClass.Name))
                return true;
            if (settings.IgnoredTypes.Contains(cppClass.Name))
                return true;
            if (LibDefinedTypes.Contains(cppClass.Name))
                return true;

            if (DefinedTypes.Contains(cppClass.Name) && !bypassDef)
            {
                LogWarn($"{context.FilePath}: {cppClass} is already defined!");
                return true;
            }

            if (!bypassDef)
                DefinedTypes.Add(cppClass.Name);

            csName = settings.GetCsCleanName(cppClass.Name);
            mapping = settings.GetTypeMapping(cppClass.Name);
            csName = mapping?.FriendlyName ?? csName;

            if (cppClass.ClassKind == CppClassKind.Class || cppClass.Name.EndsWith("_T") || csName == "void")
            {
                return true;
            }

            return false;
        }

        protected virtual void GenerateTypes(CppCompilation compilation, string outputPath)
        {
            MemberFunctions.Clear();
            string filePath = Path.Combine(outputPath, "Structures.cs");

            // Generate Structures
            using var writer = new CsSplitCodeWriter(filePath, settings.Namespace, SetupTypeUsings(), 1);
            GenContext context = new(compilation, filePath, writer);

            // Print All classes, structs
            for (int i = 0; i < compilation.Classes.Count; i++)
            {
                WriteClass(context, compilation.Classes[i]);
                if (settings.WrapPointersAsHandle)
                {
                    WriteHandle(context, compilation.Classes[i]);
                }
            }
        }

        protected virtual void WriteClass(GenContext context, CppClass cppClass)
        {
            if (FilterType(context, cppClass, out var mapping, out var csNameDefault))
                return;

            WriteClass(context, cppClass, mapping, csNameDefault);
        }

        protected virtual void WriteClass(GenContext context, CppClass cppClass, TypeMapping? mapping, string csName)
        {
            var writer = context.Writer;

            bool isReadOnly = false;
            string modifier = "partial";

            LogInfo("defined struct " + csName);
            var commentWritten = settings.WriteCsSummary(cppClass.Comment, writer);
            if (!commentWritten)
            {
                commentWritten = settings.WriteCsSummary(mapping?.Comment, writer);
            }
            if (settings.GenerateMetadata)
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
                if (settings.GenerateSizeOfStructs && cppClass.SizeOf > 0)
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
                    var csSubName = settings.GetCsSubTypeName(cppClass, csName, subClass, j);
                    var subClassMapping = settings.GetTypeMapping(subClass.FullName);

                    csSubName = subClassMapping?.FriendlyName ?? csSubName;

                    WriteClass(context, subClass, subClassMapping, csSubName);
                    subClasses.Add(new(subClass, csSubName, cppClass.Name, $"Union{(cppClass.Classes.Count == 1 ? "" : j.ToString())}"));
                }

                for (int j = 0; j < cppClass.Fields.Count; j++)
                {
                    CppField cppField = cppClass.Fields[j];
                    var fieldMapping = mapping?.GetFieldMapping(cppField.Name);
                    if (cppField.Type is CppClass cppClass1 && cppClass1.ClassKind == CppClassKind.Union)
                    {
                        var fieldCommentWritten = settings.WriteCsSummary(cppField.Comment, writer);
                        if (!fieldCommentWritten)
                        {
                            fieldCommentWritten = settings.WriteCsSummary(fieldMapping?.Comment, writer);
                        }

                        if (settings.GenerateMetadata)
                        {
                            writer.WriteLine($"[NativeName(NativeNameType.Field, \"{cppField.Name}\")]");
                            writer.WriteLine($"[NativeName(NativeNameType.Type, \"{cppField.Type.GetDisplayName()}\")]");
                        }

                        var subClass = subClasses.FirstOrDefault(x => x.CppType == cppClass1);

                        string csFieldName = settings.GetFieldName(cppField.Name);
                        if (isUnion)
                        {
                            writer.WriteLine("[FieldOffset(0)]");
                        }
                        if (subClass == null)
                        {
                            string csFieldType = settings.GetCsCleanName(cppClass1.Name);
                            subClasses.Add(new(cppField.Type, csFieldType, cppField.Name, csFieldName));

                            writer.WriteLine($"public {csFieldType} {csFieldName};");
                            if (fieldCommentWritten)
                            {
                                writer.WriteLine();
                            }

                            continue;
                        }

                        writer.WriteLine($"public {subClass.Name} {csFieldName};");

                        if (fieldCommentWritten)
                            writer.WriteLine();
                    }
                    else if (cppField.Type is CppPointerType cppPointer && cppPointer.IsDelegate(out var cppFunctionType))
                    {
                        var fieldCommentWritten = settings.WriteCsSummary(cppField.Comment, writer);
                        if (!fieldCommentWritten)
                        {
                            fieldCommentWritten = settings.WriteCsSummary(fieldMapping?.Comment, writer);
                        }

                        if (settings.GenerateMetadata)
                        {
                            writer.WriteLine($"[NativeName(NativeNameType.Field, \"{cppField.Name}\")]");
                            writer.WriteLine($"[NativeName(NativeNameType.Type, \"{cppField.Type.GetDisplayName()}\")]");
                        }

                        string csFieldName = settings.GetFieldName(cppField.Name);
                        string returnCsName = settings.GetCsTypeName(cppFunctionType.ReturnType, false);
                        string signature = settings.GetNamelessParameterSignature(cppFunctionType.Parameters, false);
                        returnCsName = returnCsName.Replace("bool", settings.GetBoolType());
                        if (settings.DelegatesAsVoidPointer)
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
                if (settings.KnownConstructors.TryGetValue(cppClass.Name, out var constructors))
                {
                    writer.WriteLine();
                    List<CsFunction> commands = new();
                    for (int i = 0; i < constructors.Count; i++)
                    {
                        CppFunction cppFunction = FindFunction(context.Compilation, constructors[i]);
                        var csFunctionName = settings.GetPrettyFunctionName(cppFunction.Name);

                        CsFunction function = CreateCsFunction(cppFunction, csFunctionName, commands, out var overload);

                        for (int j = 0; j < overload.Parameters.Count; j++)
                        {
                            var param = overload.Parameters[j];
                            var subClass = subClasses.First(x => x.CppType == param.CppType);
                            param.Type = subClass.Type;

                            int depth = 0;
                            var subClass1 = subClasses.FirstOrDefault(x => x.CppType.IsPointerOf(param.CppType, ref depth));
                            if (subClass1 != null)
                            {
                                param.Type = new CsType(subClass1.Name + new string('*', depth), CppPrimitiveKind.Void);
                            }
                        }

                        overload.StructName = csName;
                        funcGen.GenerateVariations(cppFunction.Parameters, overload, true);

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
                            WriteConstructors(context, definedConstructors, function, overload, cppClass.Fields, subClasses, "public unsafe");
                        }
                    }
                }

                if (settings.GenerateConstructorsForStructs && cppClass.Fields.Count > 0)
                {
                    settings.WriteCsSummary((string?)null, out string? comment);
                    CsFunction function = new(csName, comment);
                    CsFunctionOverload overload = new(string.Empty, csName, comment, csName, true, true, false, new(string.Empty, CppPrimitiveKind.Void));
                    function.Overloads.Add(overload);
                    for (int j = 0; j < cppClass.Fields.Count; j++)
                    {
                        var cppField = cppClass.Fields[j];
                        var csFieldName = settings.GetFieldName(cppField.Name);
                        var paramCsTypeName = settings.GetCsTypeName(cppField.Type, false);
                        var paramCsName = settings.GetParameterName(j, cppField.Name);
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

                        if (cppField.Type is CppArrayType arrayType && arrayType.ElementType is CppPointerType pointerType && pointerType.ElementType is CppFunctionType && settings.DelegatesAsVoidPointer)
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
                    WriteConstructors(context, definedConstructors, function, overload, cppClass.Fields, subClasses, "public unsafe");
                }

                writer.WriteLine();

                for (int j = 0; j < cppClass.Fields.Count; j++)
                {
                    CppField cppField = cppClass.Fields[j];
                    if (cppField.Type.TypeKind == CppTypeKind.Array)
                    {
                        WriteProperty(writer, cppField);
                    }
                }

                if (settings.KnownMemberFunctions.TryGetValue(cppClass.Name, out var functions))
                {
                    WriteMemberFunctions(context, cppClass, functions, WriteFunctionFlags.UseThis);
                }
            }

            writer.WriteLine();
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

        private void WriteConstructors(GenContext context, HashSet<string> definedFunctions, CsFunction csFunction, CsFunctionOverload overload, IList<CppField> fields, List<CsSubClass> subClasses, params string[] modifiers)
        {
            for (int j = 0; j < overload.Variations.Count; j++)
            {
                WriteConstructor(context, definedFunctions, csFunction, overload, overload.Variations[j], fields, subClasses, modifiers);
            }
        }

        private void WriteConstructor(GenContext context, HashSet<string> definedFunctions, CsFunction function, CsFunctionOverload overload, CsFunctionVariation variation, IList<CppField> fields, List<CsSubClass> subClasses, params string[] modifiers)
        {
            var writer = context.Writer;
            CsType returnType = variation.ReturnType;
            PrepareArgs(variation, returnType);

            string header = variation.BuildFullConstructorSignature(settings.GenerateMetadata);
            string id = variation.BuildConstructorSignatureIdentifier();

            if (FilterConstructor(context, definedFunctions, id))
            {
                return;
            }

            ClassifyParameters(overload, variation, returnType, out _, out _, out _);

            LogInfo("defined constructor " + header);

            writer.WriteLines(overload.Comment);
            if (settings.GenerateMetadata)
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
                        fieldName = subClasses.First(x => x.CppType == cppField.Type).Name;
                    }

                    if (fieldName == cppParameter.Name)
                    {
                        fieldName = $"this.{fieldName}";
                    }

                    // skip array field types.
                    // TODO: Add support for array field types.
                    if (cppField.Type is CppArrayType arrayType)
                    {
                        using (writer.PushBlock($"if ({cppParameter.Name} != default)"))
                        {
                            for (int j = 0; j < arrayType.Size; j++)
                            {
                                writer.WriteLine($"{fieldName}_{j} = {cppParameter.Name}[{j}];");
                            }
                        }
                    }
                    else if (cppField.Type.IsDelegate())
                    {
                        if (cppParameter.Type.Name.StartsWith("delegate*<"))
                        {
                            writer.WriteLine($"{fieldName} = (void*){cppParameter.Name};");
                        }
                        else
                        {
                            writer.WriteLine($"{fieldName} = (void*)Marshal.GetFunctionPointerForDelegate({cppParameter.Name});");
                        }
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Bool))
                    {
                        writer.WriteLine($"{fieldName} = {cppParameter.Name} ? ({settings.GetBoolType()})1 : ({settings.GetBoolType()})0;");
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
            string csFieldName = settings.GetFieldName(field.Name);

            var fieldCommentWritten = settings.WriteCsSummary(field.Comment, writer);
            if (!fieldCommentWritten)
            {
                fieldCommentWritten = settings.WriteCsSummary(mapping?.Comment, writer);
            }

            if (settings.GenerateMetadata)
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
                string csFieldType = settings.GetCsTypeName(arrayType.ElementType, false);

                if (arrayType.ElementType is CppPointerType pointerType && pointerType.ElementType is CppFunctionType functionType && settings.DelegatesAsVoidPointer)
                {
                    csFieldType = "nint";
                }

                if (arrayType.ElementType is CppTypedef typedef)
                {
                    if (typedef.IsPrimitive(out var primitive))
                    {
                        csFieldType = settings.GetCsTypeName(primitive, false);
                    }
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
                string csFieldType = settings.GetCsTypeName(field.Type, false);

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
                    csFieldType = settings.GetBoolType();

                if (field.Type is CppTypedef typedef &&
                    typedef.ElementType is CppPointerType pointerType &&
                    pointerType.ElementType is CppFunctionType functionType)
                {
                    StringBuilder builder = new();
                    for (int i = 0; i < functionType.Parameters.Count; i++)
                    {
                        CppParameter parameter = functionType.Parameters[i];
                        string paramCsType = settings.GetCsTypeName(parameter.Type, false);
                        // Otherwise we get interop issues with non blittable types

                        builder.Append(paramCsType);

                        builder.Append(", ");
                    }

                    string returnCsName = settings.GetCsTypeName(functionType.ReturnType, false);
                    returnCsName = returnCsName.Replace("bool", settings.GetBoolType());
                    builder.Append(returnCsName);

                    if (settings.DelegatesAsVoidPointer)
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
            string csFieldName = settings.GetFieldName(field.Name);

            var fieldCommentWritten = settings.WriteCsSummary(field.Comment, writer);
            if (!fieldCommentWritten)
            {
                fieldCommentWritten = settings.WriteCsSummary(mapping?.Comment, writer);
            }

            if (field.Type is CppArrayType arrayType)
            {
                string csFieldType = settings.GetCsTypeName(arrayType.ElementType, false);

                if (arrayType.ElementType is CppTypedef typedef && typedef.IsPrimitive(out var primitive))
                {
                    csFieldType = settings.GetCsTypeName(primitive, false);
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
                        csFieldType = classCsName.Replace("*", "") + "." + settings.GetCsSubTypeName(cppClass, classCsName.Replace("*", ""), subClass, index);
                        csFieldName = $"Union{(cppClass.Classes.Count == 1 ? "" : index.ToString())}";
                    }
                    else
                    {
                        csFieldType = settings.GetCsTypeName(field.Type, false);
                    }
                }
                else
                {
                    csFieldType = settings.GetCsTypeName(field.Type, false);
                }

                if (field.Type.IsDelegate(out var functionType))
                {
                    StringBuilder builder = new();
                    for (int i = 0; i < functionType.Parameters.Count; i++)
                    {
                        CppParameter parameter = functionType.Parameters[i];
                        string paramCsType = settings.GetCsTypeName(parameter.Type, false);
                        // Otherwise we get interop issues with non blittable types

                        builder.Append(paramCsType);

                        builder.Append(", ");
                    }

                    string returnCsName = settings.GetCsTypeName(functionType.ReturnType, false);
                    returnCsName = returnCsName.Replace("bool", settings.GetBoolType());
                    builder.Append(returnCsName);

                    if (settings.DelegatesAsVoidPointer)
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

                if (csFieldType.EndsWith('*'))
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
            string csFieldName = settings.GetFieldName(field.Name);

            if (field.Type is CppArrayType arrayType)
            {
                string csFieldType = settings.GetCsTypeName(arrayType.ElementType, false);
                bool canUseFixed = false;
                if (arrayType.ElementType is CppPrimitiveType)
                {
                    canUseFixed = true;
                }
                else if (arrayType.ElementType is CppTypedef typedef && typedef.IsPrimitive(out var primitive))
                {
                    csFieldType = settings.GetCsTypeName(primitive, false);
                    canUseFixed = true;
                }

                if (canUseFixed)
                {
                }
                else
                {
                    if (csFieldType.Contains('*'))
                    {
                        csFieldType = "nint";
                    }

                    settings.WriteCsSummary(field.Comment, writer);
                    writer.WriteLine($"public unsafe Span<{csFieldType}> {csFieldName}");
                    using (writer.PushBlock(""))
                    {
                        using (writer.PushBlock("get"))
                        {
                            using (writer.PushBlock($"fixed ({csFieldType}* p = &this.{csFieldName}_0)"))
                            {
                                writer.WriteLine($"return new Span<{csFieldType}>(p, {arrayType.Size});");
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
            settings.WriteCsSummary(cppClass.Comment, writer);
            if (settings.GenerateMetadata)
            {
                writer.WriteLine($"[NativeName(NativeNameType.Typedef, \"{cppClass.Name}\")]");
            }
            writer.WriteLine($"[DebuggerDisplay(\"{{DebuggerDisplay,nq}}\")]");
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
                writer.WriteLine($"private string DebuggerDisplay => string.Format(\"{csName} [0x{{0}}]\", ((nuint)Handle).ToString(\"X\"));");
                var pCount = handleType.Count(x => x == '*');
                if (pCount == 1)
                {
                    for (int j = 0; j < cppClass.Fields.Count; j++)
                    {
                        CppField cppField = cppClass.Fields[j];
                        var fieldMapping = mapping?.GetFieldMapping(cppField.Name);
                        WriteProperty(writer, cppClass, handleType, cppField, fieldMapping, isUnion, false);
                    }

                    if (settings.KnownMemberFunctions.TryGetValue(cppClass.Name, out var functions))
                    {
                        WriteMemberFunctions(context, cppClass, functions, WriteFunctionFlags.UseHandle);
                    }
                }
                else
                {
                    using (writer.PushBlock($"public {csName[..^3]} this[int index]"))
                    {
                        writer.WriteLine($"get => Handle[index]; set => Handle[index] = value;");
                    }
                }
            }
            writer.WriteLine();
        }

        private void WriteMemberFunctions(GenContext context, CppClass cppClass, List<string> functions, WriteFunctionFlags flags)
        {
            HashSet<string> definedFunctions = new();
            List<CsFunction> commands = new();
            for (int i = 0; i < functions.Count; i++)
            {
                CppFunction cppFunction = FindFunction(context.Compilation, functions[i]);
                var csFunctionName = settings.GetPrettyFunctionName(cppFunction.Name);

                CsFunction function = CreateCsFunction(cppFunction, csFunctionName, commands, out var overload);
                funcGen.GenerateVariations(cppFunction.Parameters, overload, true);

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
                    WriteFunctions(context, definedFunctions, function, overload, flags, "public unsafe");
                }
            }

            if (flags == WriteFunctionFlags.UseThis)
            {
                if (MemberFunctions.TryGetValue(cppClass.Name, out var funcs))
                {
                    foreach (string f in definedFunctions)
                    {
                        if (funcs.Contains(f))
                            continue;
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