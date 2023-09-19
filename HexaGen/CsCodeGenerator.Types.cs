namespace HexaGen
{
    using CppAst;
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
            using var writer = new CodeWriter(filePath, settings.Namespace, SetupTypeUsings());
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
            var commentWritten = cppClass.Comment.WriteCsSummary(writer);
            if (!commentWritten)
            {
                commentWritten = mapping?.Comment.WriteCsSummary(writer) ?? false;
            }

            writer.WriteLine($"[NativeName(NativeNameType.StructOrClass, \"{cppClass.Name}\")]");

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

                List<(CppType, string, string)> subClasses = new();
                for (int j = 0; j < cppClass.Classes.Count; j++)
                {
                    var subClass = cppClass.Classes[j];
                    var csSubName = settings.GetCsSubTypeName(cppClass, csName, subClass, j);
                    var subClassMapping = settings.GetTypeMapping(subClass.Name);

                    csSubName = subClassMapping?.FriendlyName ?? csSubName;

                    WriteClass(context, subClass, subClassMapping, csSubName);
                    subClasses.Add((subClass, csSubName, $"Union{(cppClass.Classes.Count == 1 ? "" : j.ToString())}"));
                }

                for (int j = 0; j < cppClass.Fields.Count; j++)
                {
                    CppField cppField = cppClass.Fields[j];
                    var fieldMapping = mapping?.GetFieldMapping(cppField.Name);
                    if (cppField.Type is CppClass cppClass1 && cppClass1.ClassKind == CppClassKind.Union)
                    {
                        var fieldCommentWritten = cppField.Comment.WriteCsSummary(writer);
                        if (!fieldCommentWritten)
                        {
                            fieldCommentWritten = fieldMapping?.Comment.WriteCsSummary(writer) ?? false;
                        }
                        writer.WriteLine($"[NativeName(NativeNameType.Field, \"{cppField.Name}\")]");
                        writer.WriteLine($"[NativeName(NativeNameType.Type, \"{cppField.Type.GetDisplayName()}\")]");

                        var subClass = subClasses.FirstOrDefault(x => ReferenceEquals(x.Item1, cppClass1));
                        if (isUnion)
                        {
                            writer.WriteLine("[FieldOffset(0)]");
                        }
                        if (subClass == default)
                        {
                            string csFieldName = settings.GetFieldName(cppField.Name);
                            string csFieldType = settings.GetCsCleanName(cppClass1.Name);

                            writer.WriteLine($"public {csFieldType} {csFieldName};");
                            if (fieldCommentWritten)
                            {
                                writer.WriteLine();
                            }

                            continue;
                        }

                        writer.WriteLine($"public {subClass.Item2} {subClass.Item3};");

                        if (fieldCommentWritten)
                            writer.WriteLine();
                    }
                    else if (cppField.Type is CppPointerType cppPointer && cppPointer.IsDelegate(out var cppFunctionType))
                    {
                        var fieldCommentWritten = cppField.Comment.WriteCsSummary(writer);
                        if (!fieldCommentWritten)
                        {
                            fieldCommentWritten = fieldMapping?.Comment.WriteCsSummary(writer) ?? false;
                        }
                        writer.WriteLine($"[NativeName(NativeNameType.Field, \"{cppField.Name}\")]");
                        writer.WriteLine($"[NativeName(NativeNameType.Type, \"{cppField.Type.GetDisplayName()}\")]");

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

                if (settings.KnownConstructors.TryGetValue(cppClass.Name, out var constructors))
                {
                    HashSet<string> definedFunctions = new();
                    writer.WriteLine();
                    List<CsFunction> commands = new();
                    for (int i = 0; i < constructors.Count; i++)
                    {
                        CppFunction cppFunction = FindFunction(context.Compilation, constructors[i]);
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
                            WriteConstructors(writer, definedFunctions, function, overload, csName, "public unsafe");
                        }
                    }
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

        private static void WriteConstructors(CodeWriter writer, HashSet<string> definedFunctions, CsFunction csFunction, CsFunctionOverload overload, string typeName, params string[] modifiers)
        {
            for (int j = 0; j < overload.Variations.Count; j++)
            {
                WriteConstructor(writer, definedFunctions, csFunction, overload, overload.Variations[j], typeName, modifiers);
            }
        }

        private static void WriteConstructor(CodeWriter writer, HashSet<string> definedFunctions, CsFunction function, CsFunctionOverload overload, CsFunctionVariation variation, string typeName, params string[] modifiers)
        {
        }

        private void WriteField(CodeWriter writer, CppField field, TypeFieldMapping? mapping, List<(CppType, string, string)> subClasses, bool isUnion = false, bool isReadOnly = false)
        {
            string csFieldName = settings.GetFieldName(field.Name);

            var fieldCommentWritten = field.Comment.WriteCsSummary(writer);
            if (!fieldCommentWritten)
            {
                fieldCommentWritten = mapping?.Comment.WriteCsSummary(writer) ?? false;
            }

            writer.WriteLine($"[NativeName(NativeNameType.Field, \"{field.Name}\")]");
            writer.WriteLine($"[NativeName(NativeNameType.Type, \"{field.Type.GetDisplayName()}\")]");

            if (isUnion)
            {
                writer.WriteLine("[FieldOffset(0)]");
            }

            if (field.Type is CppArrayType arrayType)
            {
                string csFieldType = settings.GetCsTypeName(arrayType.ElementType, false);

                if (arrayType.ElementType is CppTypedef typedef && typedef.IsPrimitive(out var primitive))
                {
                    csFieldType = settings.GetCsTypeName(primitive, false);
                }

                string unsafePrefix = string.Empty;

                if (csFieldType.EndsWith('*'))
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
                if (string.IsNullOrEmpty(csFieldType))
                {
                    var subClass = subClasses.Find(x => x.Item1 == field.Type);
                    csFieldType = subClass.Item2;
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

        private void WriteProperty(CodeWriter writer, CppClass cppClass, string classCsName, CppField field, TypeFieldMapping? mapping, bool isUnion = false, bool isReadOnly = false)
        {
            string csFieldName = settings.GetFieldName(field.Name);

            var fieldCommentWritten = field.Comment.WriteCsSummary(writer);
            if (!fieldCommentWritten)
            {
                fieldCommentWritten = mapping?.Comment.WriteCsSummary(writer) ?? false;
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

        private void WriteProperty(CodeWriter writer, CppField field)
        {
            string csFieldName = settings.GetFieldName(field.Name);
            field.Comment.WriteCsSummary(writer);
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
                    if (csFieldType.EndsWith('*'))
                    {
                        return;
                    }

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
            cppClass.Comment.WriteCsSummary(writer);
            writer.WriteLine($"[NativeName(NativeNameType.Typedef, \"{cppClass.Name}\")]");
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