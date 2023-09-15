namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.Logging;
    using HexaGen.Core.Mapping;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public partial class CsCodeGeneratorSettings : IGeneratorSettings
    {
        public static CsCodeGeneratorSettings Load(string file)
        {
            CsCodeGeneratorSettings result;
            if (File.Exists(file))
            {
                result = JsonSerializer.Deserialize<CsCodeGeneratorSettings>(File.ReadAllText(file)) ?? new();
            }
            else
            {
                result = new();
            }
            result.Save(file);
            return result;
        }

        public string Namespace { get; set; } = string.Empty;

        public string ApiName { get; set; } = string.Empty;

        public string LibName { get; set; } = string.Empty;

        public LogSevertiy LogLevel { get; set; } = LogSevertiy.Warning;

        public LogSevertiy CppLogLevel { get; set; } = LogSevertiy.Error;

        public bool GenerateSizeOfStructs { get; set; } = false;

        public bool DelegatesAsVoidPointer { get; set; } = true;

        public bool GenerateConstants { get; set; } = true;

        public bool GenerateEnums { get; set; } = true;

        public bool GenerateExtensions { get; set; } = true;

        public bool GenerateFunctions { get; set; } = true;

        public bool GenerateHandles { get; set; } = true;

        public bool GenerateTypes { get; set; } = true;

        public bool GenerateDelegates { get; set; } = true;

        public BoolType BoolType { get; set; } = BoolType.Bool8;

        public Dictionary<string, string> KnownConstantNames { get; set; } = new();

        public Dictionary<string, string> KnownEnumValueNames { get; set; } = new();

        public Dictionary<string, string> KnownEnumPrefixes { get; set; } = new();

        public Dictionary<string, string> KnownExtensionPrefixes { get; set; } = new();

        public Dictionary<string, string> KnownExtensionNames { get; set; } = new();

        public Dictionary<string, string> KnownDefaultValueNames { get; set; } = new();

        public Dictionary<string, List<string>> KnownConstructors { get; set; } = new();

        public Dictionary<string, List<string>> KnownMemberFunctions { get; set; } = new();

        public HashSet<string> IgnoredParts { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public HashSet<string> PreserveCaps { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public HashSet<string> Keywords { get; set; } = new();

        public HashSet<string> IgnoredFunctions { get; set; } = new();

        public HashSet<string> IgnoredTypes { get; set; } = new();

        public HashSet<string> IgnoredEnums { get; set; } = new();

        public HashSet<string> IgnoredTypedefs { get; set; } = new();

        public HashSet<string> IgnoredDelegates { get; set; } = new();

        public HashSet<string> IgnoredConstants { get; set; } = new();

        public Dictionary<string, string> IIDMappings { get; set; } = new();

        public List<ConstantMapping> ConstantMappings { get; set; } = new();

        public List<EnumMapping> EnumMappings { get; set; } = new();

        public List<FunctionMapping> FunctionMappings { get; set; } = new();

        public List<HandleMapping> HandleMappings { get; set; } = new();

        public List<TypeMapping> ClassMappings { get; set; } = new();

        public List<DelegateMapping> DelegateMappings { get; set; } = new();

        public List<ArrayMapping> ArrayMappings { get; set; } = new();

        public Dictionary<string, string> NameMappings { get; set; } = new()
        {
        };

        public Dictionary<string, string> TypeMappings { get; set; } = new()
        {
            { "uint8_t", "byte" },
            { "uint16_t", "ushort" },
            { "uint32_t", "uint" },
            { "uint64_t", "ulong" },
            { "int8_t", "sbyte" },
            { "int32_t", "int" },
            { "int16_t", "short" },
            { "int64_t", "long" },
            { "int64_t*", "long*" },
            { "unsigned char", "byte" },
            { "signed char", "sbyte" },
            { "char", "byte" },
            { "size_t", "nuint" }
        };

        public HashSet<string> AllowedFunctions { get; set; } = new();

        public HashSet<string> AllowedTypes { get; set; } = new();

        public HashSet<string> AllowedEnums { get; set; } = new();

        public HashSet<string> AllowedTypedefs { get; set; } = new();

        public HashSet<string> AllowedDelegates { get; set; } = new();

        public HashSet<string> AllowedConstants { get; set; } = new();

        public List<string> Usings { get; set; } = new();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention ConstantNamingConvention { get; set; } = NamingConvention.Unknown;

        public void Save(string path)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                WriteIndented = true,
            }));
        }

        #region Mapping Helpers

        public bool TryGetEnumMapping(string enumName, [NotNullWhen(true)] out EnumMapping? mapping)
        {
            for (int i = 0; i < EnumMappings.Count; i++)
            {
                var enumMapping = EnumMappings[i];
                if (enumMapping.ExportedName == enumName)
                {
                    mapping = enumMapping;
                    return true;
                }
            }

            mapping = null;
            return false;
        }

        public EnumMapping? GetEnumMapping(string enumName)
        {
            for (int i = 0; i < EnumMappings.Count; i++)
            {
                var enumMapping = EnumMappings[i];
                if (enumMapping.ExportedName == enumName)
                {
                    return enumMapping;
                }
            }

            return null;
        }

        public bool TryGetFunctionMapping(string functionName, [NotNullWhen(true)] out FunctionMapping? mapping)
        {
            for (int i = 0; i < FunctionMappings.Count; i++)
            {
                var functionMapping = FunctionMappings[i];
                if (functionMapping.ExportedName == functionName)
                {
                    mapping = functionMapping;
                    return true;
                }
            }

            mapping = null;
            return false;
        }

        public FunctionMapping? GetFunctionMapping(string functionName)
        {
            for (int i = 0; i < FunctionMappings.Count; i++)
            {
                var functionMapping = FunctionMappings[i];
                if (functionMapping.ExportedName == functionName)
                {
                    return functionMapping;
                }
            }

            return null;
        }

        public bool TryGetTypeMapping(string typeName, [NotNullWhen(true)] out TypeMapping? mapping)
        {
            for (int i = 0; i < ClassMappings.Count; i++)
            {
                var structMapping = ClassMappings[i];
                if (structMapping.ExportedName == typeName)
                {
                    mapping = structMapping;
                    return true;
                }
            }

            mapping = null;
            return false;
        }

        public TypeMapping? GetTypeMapping(string typeName)
        {
            for (int i = 0; i < ClassMappings.Count; i++)
            {
                var structMapping = ClassMappings[i];
                if (structMapping.ExportedName == typeName)
                {
                    return structMapping;
                }
            }

            return null;
        }

        public bool TryGetDelegateMapping(string delegateName, [NotNullWhen(true)] out DelegateMapping? mapping)
        {
            for (int i = 0; i < DelegateMappings.Count; i++)
            {
                var delegateMapping = DelegateMappings[i];
                if (delegateMapping.Name == delegateName)
                {
                    mapping = delegateMapping;
                    return true;
                }
            }

            mapping = null;
            return false;
        }

        public DelegateMapping? GetDelegateMapping(string delegateName)
        {
            for (int i = 0; i < DelegateMappings.Count; i++)
            {
                var delegateMapping = DelegateMappings[i];
                if (delegateMapping.Name == delegateName)
                {
                    return delegateMapping;
                }
            }

            return null;
        }

        public bool TryGetArrayMapping(CppArrayType arrayType, [NotNullWhen(true)] out string? mapping)
        {
            for (int i = 0; i < ArrayMappings.Count; i++)
            {
                var map = ArrayMappings[i];
                if (map.Primitive == arrayType.GetPrimitiveKind(false) && map.Size == arrayType.Size)
                {
                    mapping = map.Name;
                    return true;
                }
            }
            mapping = null;
            return false;
        }

        #endregion Mapping Helpers

        public string GetCsTypeName(CppType? type, bool isPointer = false)
        {
            if (type is CppPrimitiveType primitiveType)
            {
                return GetCsTypeName(primitiveType, isPointer);
            }

            if (type is CppQualifiedType qualifiedType)
            {
                return GetCsTypeName(qualifiedType.ElementType, isPointer);
            }

            if (type is CppReferenceType referenceType)
            {
                return GetCsTypeName(referenceType.ElementType, true);
            }

            if (type is CppEnum enumType)
            {
                var enumCsName = GetCsCleanName(enumType.Name);
                if (isPointer)
                    return enumCsName + "*";

                return enumCsName;
            }

            if (type is CppTypedef typedef)
            {
                var typeDefCsName = GetCsCleanName(typedef.Name);
                if (typedef.IsDelegate(out var _))
                {
                    return typeDefCsName;
                }

                if (isPointer)
                    return typeDefCsName + "*";

                return typeDefCsName;
            }

            if (type is CppClass @class)
            {
                var className = GetCsCleanName(@class.Name);
                if (isPointer)
                    return className + "*";

                return className;
            }

            if (type is CppPointerType pointerType)
            {
                var pointerName = GetCsTypeName(pointerType);
                if (isPointer)
                    return pointerName + "*";

                return pointerName;
            }

            if (type is CppArrayType arrayType)
            {
                var arrayName = GetCsTypeName(arrayType.ElementType, false);
                return arrayName + "*";
            }

            if (type is CppUnexposedType unexposedType)
            {
                throw new();
            }

            return string.Empty;
        }

        public string GetCsTypeName(CppPointerType pointerType)
        {
            if (pointerType.ElementType is CppQualifiedType qualifiedType)
            {
                if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
                {
                    return GetCsTypeName(primitiveType, true);
                }
                else if (qualifiedType.ElementType is CppClass @classType)
                {
                    return GetCsTypeName(@classType, true);
                }
                else if (qualifiedType.ElementType is CppPointerType subPointerType)
                {
                    return GetCsTypeName(subPointerType.ElementType, true) + "*";
                }
                else if (qualifiedType.ElementType is CppTypedef typedef)
                {
                    return GetCsTypeName(typedef, true);
                }
                else if (qualifiedType.ElementType is CppEnum @enum)
                {
                    return GetCsTypeName(@enum, true);
                }

                return GetCsTypeName(qualifiedType.ElementType, true);
            }

            if (pointerType.ElementType is CppFunctionType functionType)
            {
                return $"delegate*<{GetNamelessParameterSignature(functionType.Parameters, false)}>";
            }

            if (pointerType.ElementType is CppPointerType subPointer)
            {
                return GetCsTypeName(subPointer) + "*";
            }

            return GetCsTypeName(pointerType.ElementType, true);
        }

        public string GetCsTypeName(CppPrimitiveType primitiveType, bool isPointer)
        {
            switch (primitiveType.Kind)
            {
                case CppPrimitiveKind.Void:
                    return isPointer ? "void*" : "void";

                case CppPrimitiveKind.Char:
                    return isPointer ? "byte*" : "byte";

                case CppPrimitiveKind.Bool:
                    return isPointer ? $"{GetBoolType()}*" : "bool";

                case CppPrimitiveKind.WChar:
                    return isPointer ? "char*" : "char";

                case CppPrimitiveKind.Short:
                    return isPointer ? "short*" : "short";

                case CppPrimitiveKind.Int:
                    return isPointer ? "int*" : "int";

                case CppPrimitiveKind.LongLong:
                    return isPointer ? "long*" : "long";

                case CppPrimitiveKind.UnsignedChar:
                    return isPointer ? "byte*" : "byte";

                case CppPrimitiveKind.UnsignedShort:
                    return isPointer ? "ushort*" : "ushort";

                case CppPrimitiveKind.UnsignedInt:
                    return isPointer ? "uint*" : "uint";

                case CppPrimitiveKind.UnsignedLongLong:
                    return isPointer ? "ulong*" : "ulong";

                case CppPrimitiveKind.Float:
                    return isPointer ? "float*" : "float";

                case CppPrimitiveKind.Double:
                    return isPointer ? "double*" : "double";

                case CppPrimitiveKind.LongDouble:
                    break;

                default:
                    return string.Empty;
            }

            return string.Empty;
        }

        public string GetCsWrapperTypeName(CppType? type, bool isPointer = false)
        {
            if (type is CppPrimitiveType primitiveType)
            {
                return GetCsWrapperTypeName(primitiveType, isPointer);
            }

            if (type is CppQualifiedType qualifiedType)
            {
                return GetCsWrapperTypeName(qualifiedType.ElementType, isPointer);
            }

            if (type is CppReferenceType referenceType)
            {
                return GetCsWrapperTypeName(referenceType.ElementType, true);
            }

            if (type is CppEnum enumType)
            {
                var enumCsName = GetCsCleanName(enumType.Name);
                if (isPointer)
                    return "ref " + enumCsName;

                return enumCsName;
            }

            if (type is CppTypedef typedef)
            {
                var typeDefCsName = GetCsCleanName(typedef.Name);
                if (typedef.IsDelegate())
                {
                    return typeDefCsName;
                }
                if (isPointer && typeDefCsName == "void")
                    return "void*";
                if (isPointer)
                    return "ref " + typeDefCsName;

                return typeDefCsName;
            }

            if (type is CppClass @class)
            {
                var className = GetCsCleanName(@class.Name);
                if (isPointer && className == "void")
                    return "void*";
                if (isPointer)
                    return "ref " + className;

                return className;
            }

            if (type is CppPointerType pointerType)
            {
                return GetCsWrapperTypeName(pointerType);
            }

            if (type is CppArrayType arrayType && arrayType.Size > 0)
            {
                if (TryGetArrayMapping(arrayType, out string? mapping))
                {
                    return mapping;
                }

                return GetCsWrapperTypeName(arrayType.ElementType, true);
            }
            else if (type is CppArrayType arrayType1 && arrayType1.Size < 0)
            {
                var arrayName = GetCsTypeName(arrayType1.ElementType, false);
                return arrayName + "*";
            }

            return string.Empty;
        }

        public string GetCsWrapperTypeName(CppPrimitiveType primitiveType, bool isPointer)
        {
            switch (primitiveType.Kind)
            {
                case CppPrimitiveKind.Void:
                    return isPointer ? "void*" : "void";

                case CppPrimitiveKind.Char:
                    return isPointer ? "ref byte" : "byte";

                case CppPrimitiveKind.Bool:
                    return isPointer ? $"ref {GetBoolType()}" : "bool";

                case CppPrimitiveKind.WChar:
                    return isPointer ? "ref char" : "char";

                case CppPrimitiveKind.Short:
                    return isPointer ? "ref short" : "short";

                case CppPrimitiveKind.Int:
                    return isPointer ? "ref int" : "int";

                case CppPrimitiveKind.LongLong:
                    break;

                case CppPrimitiveKind.UnsignedChar:
                    return isPointer ? "ref byte" : "byte";

                case CppPrimitiveKind.UnsignedShort:
                    return isPointer ? "ref ushort" : "ushort";

                case CppPrimitiveKind.UnsignedInt:
                    return isPointer ? "ref uint" : "uint";

                case CppPrimitiveKind.UnsignedLongLong:
                    break;

                case CppPrimitiveKind.Float:
                    return isPointer ? "ref float" : "float";

                case CppPrimitiveKind.Double:
                    return isPointer ? "ref double" : "double";

                case CppPrimitiveKind.LongDouble:
                    break;

                default:
                    return string.Empty;
            }

            return string.Empty;
        }

        public string GetCsWrapperTypeName(CppPointerType pointerType)
        {
            if (pointerType.ElementType is CppQualifiedType qualifiedType)
            {
                if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
                {
                    return GetCsWrapperTypeName(primitiveType, true);
                }
                else if (qualifiedType.ElementType is CppClass @classType)
                {
                    return GetCsWrapperTypeName(@classType, true);
                }
                else if (qualifiedType.ElementType is CppPointerType subPointerType)
                {
                    return GetCsWrapperTypeName(subPointerType, true) + "*";
                }
                else if (qualifiedType.ElementType is CppTypedef typedef)
                {
                    return GetCsWrapperTypeName(typedef, true);
                }
                else if (qualifiedType.ElementType is CppEnum @enum)
                {
                    return GetCsWrapperTypeName(@enum, true);
                }

                return GetCsWrapperTypeName(qualifiedType.ElementType, true);
            }

            if (pointerType.ElementType is CppFunctionType functionType)
            {
                return $"delegate*<{GetNamelessParameterSignature(functionType.Parameters, false)}>";
            }

            if (pointerType.ElementType is CppPointerType subPointer)
            {
                return GetCsWrapperTypeName(subPointer) + "*";
            }

            return GetCsWrapperTypeName(pointerType.ElementType, true);
        }

        public string GetParameterSignature(IList<CppParameter> parameters, bool canUseOut, bool attributes = true)
        {
            StringBuilder argumentBuilder = new();
            int index = 0;

            for (int i = 0; i < parameters.Count; i++)
            {
                CppParameter cppParameter = parameters[i];
                var paramCsTypeName = GetCsTypeName(cppParameter.Type, false);
                var paramCsName = GetParameterName(i, cppParameter.Name);

                if (attributes)
                {
                    argumentBuilder.Append($"[NativeName(NativeNameType.Param, \"{cppParameter.Name}\")] ");
                    argumentBuilder.Append($"[NativeName(NativeNameType.Type, \"{cppParameter.Type.GetDisplayName()}\")] ");
                }

                if (paramCsTypeName == "bool")
                {
                    paramCsTypeName = GetBoolType();
                }

                if (canUseOut && cppParameter.Type.CanBeUsedAsOutput(out CppTypeDeclaration? cppTypeDeclaration))
                {
                    argumentBuilder.Append("out ");
                    paramCsTypeName = GetCsTypeName(cppTypeDeclaration, false);
                }

                argumentBuilder.Append(paramCsTypeName).Append(' ').Append(paramCsName);

                if (index < parameters.Count - 1)
                {
                    argumentBuilder.Append(", ");
                }

                index++;
            }

            return argumentBuilder.ToString();
        }

        public string GetNamelessParameterSignature(IList<CppParameter> parameters, bool canUseOut)
        {
            var argumentBuilder = new StringBuilder();
            int index = 0;

            foreach (CppParameter cppParameter in parameters)
            {
                string direction = string.Empty;
                var paramCsTypeName = GetCsTypeName(cppParameter.Type, false);

                if (canUseOut && cppParameter.Type.CanBeUsedAsOutput(out CppTypeDeclaration? cppTypeDeclaration))
                {
                    argumentBuilder.Append("out ");
                    paramCsTypeName = GetCsTypeName(cppTypeDeclaration, false);
                }

                argumentBuilder.Append(paramCsTypeName);
                if (index < parameters.Count - 1)
                {
                    argumentBuilder.Append(", ");
                }

                index++;
            }

            return argumentBuilder.ToString();
        }

        public string GetParameterName(int paramIdx, string name)
        {
            if (name == "out")
            {
                return "output";
            }
            if (name == "ref")
            {
                return "reference";
            }
            if (name == "in")
            {
                return "input";
            }
            if (name == "base")
            {
                return "baseValue";
            }
            if (name == "void")
            {
                return "voidValue";
            }
            if (name == "int")
            {
                return "intValue";
            }
            if (name == "lock")
            {
                return "lock0";
            }
            if (name == "event")
            {
                return "evnt";
            }
            if (name == "string")
            {
                return "str";
            }
            if (Keywords.Contains(name))
            {
                return "@" + name;
            }

            if (name.StartsWith('p') && name.Length > 1 && char.IsUpper(name[1]))
            {
            }

            if (name == string.Empty)
            {
                return $"unknown{paramIdx}";
            }

            return NormalizeParameterName(name);
        }

        public string NormalizeParameterName(string name)
        {
            var parts = name.Split('_', StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new();
            for (int i = 0; i < parts.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append(char.ToLower(parts[i][0]));
                    sb.Append(parts[i][1..]);
                }
                else
                {
                    sb.Append(char.ToUpper(parts[i][0]));
                    sb.Append(parts[i][1..]);
                }
            }
            name = sb.ToString();
            if (Keywords.Contains(name))
            {
                return "@" + name;
            }

            return name;
        }

        public string? NormalizeValue(string value, bool sanitize)
        {
            if (KnownDefaultValueNames.TryGetValue(value, out var names))
            {
                return names;
            }

            if (value == "NULL")
                return "default";
            if (value == "FLT_MAX")
                return "float.MaxValue";
            if (value == "-FLT_MAX")
                return "-float.MaxValue";
            if (value == "FLT_MIN")
                return "float.MinValue";
            if (value == "-FLT_MIN")
                return "-float.MinValue";
            if (value == "nullptr")
                return "default";
            if (value == "false")
                return "0";
            if (value == "true")
                return "1";

            // TODO: needs refactoring. remove the ImVec!
            if (value.StartsWith("ImVec") && sanitize)
                return null;
            if (value.StartsWith("ImVec2"))
            {
                value = value[7..][..(value.Length - 8)];
                var parts = value.Split(',');
                return $"new Vector2({NormalizeValue(parts[0], sanitize)},{NormalizeValue(parts[1], sanitize)})";
            }
            if (value.StartsWith("ImVec4"))
            {
                value = value[7..][..(value.Length - 8)];
                var parts = value.Split(',');
                return $"new Vector4({NormalizeValue(parts[0], sanitize)},{NormalizeValue(parts[1], sanitize)},{NormalizeValue(parts[2], sanitize)},{NormalizeValue(parts[3], sanitize)})";
            }
            return value;
        }

        public string GetPrettyFunctionName(string function)
        {
            if (TryGetFunctionMapping(function, out var mapping))
            {
                if (mapping.FriendlyName != null)
                    return mapping.FriendlyName;
            }

            string[] parts = GetCsCleanName(function).SplitByCase();

            StringBuilder sb = new();
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];

                if (IgnoredParts.Contains(part))
                {
                    continue;
                }

                sb.Append(part);
            }

            return sb.ToString();
        }

        public bool TryGetDefaultValue(string functionName, CppParameter parameter, bool sanitize, out string? defaultValue)
        {
            if (TryGetFunctionMapping(functionName, out var mapping))
            {
                if (mapping.Defaults.TryGetValue(parameter.Name, out var value))
                {
                    if (parameter.Type is CppTypedef typedef && typedef.ElementType is CppPrimitiveType type)
                    {
                        if (value.IsNumeric())
                        {
                            defaultValue = value;
                            return true;
                        }

                        string csName = GetCsCleanName(typedef.Name);
                        EnumPrefix enumNamePrefix = GetEnumNamePrefix(typedef.Name);
                        if (csName.EndsWith("_"))
                        {
                            csName = csName.Remove(csName.Length - 1);
                        }
                        var enumItemName = GetPrettyEnumName(value, enumNamePrefix);

                        defaultValue = $"{csName}.{enumItemName}";
                        return true;
                    }
                    defaultValue = NormalizeValue(value, sanitize);
                    return true;
                }
            }
            defaultValue = null;
            return false;
        }

        public string GetPrettyConstantName(string value)
        {
            if (KnownConstantNames.TryGetValue(value, out string? knownName))
            {
                return knownName;
            }

            return GetCsCleanNameWithConvention(value, ConstantNamingConvention, false);
        }

        public EnumPrefix GetEnumNamePrefix(string typeName)
        {
            if (KnownEnumPrefixes.TryGetValue(typeName, out string? knownValue))
            {
                return new(knownValue.Split('_'));
            }

            string[] parts = typeName.Split('_', StringSplitOptions.RemoveEmptyEntries).SelectMany(x => x.SplitByCase()).ToArray();
            List<string> partList = new();
            bool mergeWithLast = false;
            int last = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if ((part.Length == 1 || part.IsNumeric()) && !mergeWithLast)
                {
                    if (i == 0 && parts.Length > 1)
                    {
                        mergeWithLast = true;
                    }
                    else if (i > 0)
                    {
                        partList[last] += part;
                    }
                    else
                    {
                        last = partList.Count;
                        partList.Add(part);
                    }
                }
                else if (mergeWithLast)
                {
                    last = partList.Count;
                    partList.Add(parts[last] + part);
                    mergeWithLast = false;
                }
                else
                {
                    last = partList.Count;
                    partList.Add(part);
                }
            }

            return new(partList.ToArray());
        }

        public string GetPrettyEnumName(string value, EnumPrefix enumPrefix)
        {
            if (KnownEnumValueNames.TryGetValue(value, out string? knownName))
            {
                return knownName;
            }

            if (value.StartsWith("0x"))
                return value;

            string[] parts = GetEnumNamePrefix(value).Parts;
            string[] prefixParts = enumPrefix.Parts;

            bool capture = false;
            var sb = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (IgnoredParts.Contains(part, StringComparer.InvariantCultureIgnoreCase) || prefixParts.Contains(part, StringComparer.InvariantCultureIgnoreCase) && !capture)
                {
                    continue;
                }

                part = part.ToLower();

                bool wasNum = false;
                for (int j = 0; j < part.Length; j++)
                {
                    var c = part[j];
                    if (j == 0 || wasNum)
                    {
                        sb.Append(char.ToUpper(c));
                        wasNum = false;
                    }
                    else if (char.IsDigit(c))
                    {
                        sb.Append(c);
                        wasNum = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }

                capture = true;
            }

            if (sb.Length == 0)
            {
                sb.Append(prefixParts[^1].ToCamelCase());
            }

            string prettyName = sb.ToString();

            return char.IsNumber(prettyName[0]) ? prefixParts[^1].ToCamelCase() + prettyName : prettyName;
        }

        public string GetExtensionNamePrefix(string typeName)
        {
            if (KnownExtensionPrefixes.TryGetValue(typeName, out string? knownValue))
            {
                return knownValue;
            }

            string[] parts = typeName.Split('_', StringSplitOptions.RemoveEmptyEntries).SelectMany(x => x.SplitByCase()).ToArray();

            return string.Join("_", parts.Select(s => s.ToUpper()));
        }

        public string GetPrettyExtensionName(string value, string extensionPrefix)
        {
            if (KnownExtensionNames.TryGetValue(value, out string? knownName))
            {
                return knownName;
            }

            string[] parts = value.Split('_', StringSplitOptions.RemoveEmptyEntries).SelectMany(x => x.SplitByCase()).ToArray();
            string[] prefixParts = extensionPrefix.Split('_', StringSplitOptions.RemoveEmptyEntries);

            bool capture = false;
            var sb = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (prefixParts.Contains(part, StringComparer.InvariantCultureIgnoreCase) && !capture)
                {
                    continue;
                }

                part = part.ToLower();

                sb.Append(char.ToUpper(part[0]));
                sb.Append(part[1..]);
                capture = true;
            }

            if (sb.Length == 0)
                sb.Append(value);

            string prettyName = sb.ToString();
            return (char.IsNumber(prettyName[0])) ? prefixParts[^1].ToCamelCase() + prettyName : prettyName;
        }

        public string NormalizeFieldName(string name)
        {
            var parts = name.Split('_', StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new();
            for (int i = 0; i < parts.Length; i++)
            {
                sb.Append(char.ToUpper(parts[i][0]));
                sb.Append(parts[i][1..]);
            }
            name = sb.ToString();
            if (Keywords.Contains(name))
            {
                return "@" + name;
            }

            if (name.Length == 0)
                return name;

            return char.IsDigit(name[0]) ? '_' + name : name;
        }

        public string GetBoolType()
        {
            return BoolType switch
            {
                BoolType.Bool8 => "byte",
                BoolType.Bool32 => "int",
                _ => throw new NotSupportedException(),
            };
        }
    }
}