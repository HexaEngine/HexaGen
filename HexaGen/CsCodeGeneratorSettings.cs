namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.Logging;
    using HexaGen.Core.Mapping;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Xml.Linq;

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

        /// <summary>
        /// This option generates the sizes of the structs.
        /// </summary>
        public bool GenerateSizeOfStructs { get; set; } = false;

        /// <summary>
        /// This option makes that the delegates are just void* and not delegate pointer (delegate*<void>)
        /// </summary>
        public bool DelegatesAsVoidPointer { get; set; } = true;

        /// <summary>
        /// This option makes the resulting wrapper more "safe" so you don't need unsafe blocks everywhere.
        /// </summary>
        public bool WrapPointersAsHandle { get; set; } = false;

        /// <summary>
        /// This causes the code generator to generate summary xml comments if it's missing with the text "To be documented."
        /// </summary>
        public bool GeneratePlaceholderComments { get; set; } = true;

        /// <summary>
        /// Enables generation for constants (CPP: Macros)
        /// </summary>
        public bool GenerateConstants { get; set; } = true;

        /// <summary>
        /// Enables generation for enums
        /// </summary>
        public bool GenerateEnums { get; set; } = true;

        /// <summary>
        /// Enables generation for extensions, this option is very useful if you have an handle type or COM objects.
        /// </summary>
        public bool GenerateExtensions { get; set; } = true;

        /// <summary>
        /// Enables generation for functions. This option generates the public API dllexport functions.
        /// </summary>
        public bool GenerateFunctions { get; set; } = true;

        /// <summary>
        /// Enables generation for handles. (CPP: Typedefs)
        /// </summary>
        public bool GenerateHandles { get; set; } = true;

        /// <summary>
        /// Enables generation for types. This includes COM objects and normal C-Structs.
        /// </summary>
        public bool GenerateTypes { get; set; } = true;

        /// <summary>
        /// Enables generation for delegates.
        /// </summary>
        public bool GenerateDelegates { get; set; } = true;

        /// <summary>
        /// This option controls the bool type eg. 8Bit Bool and 32Bit Bool
        /// </summary>
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

        /// <summary>
        /// All function names in this HashSet will be ignored in the generation process.
        /// </summary>
        public HashSet<string> IgnoredFunctions { get; set; } = new();

        /// <summary>
        /// All types names in this HashSet will be ignored in the generation process.
        /// </summary>
        public HashSet<string> IgnoredTypes { get; set; } = new();

        /// <summary>
        /// All enums names in this HashSet will be ignored in the generation process.
        /// </summary>
        public HashSet<string> IgnoredEnums { get; set; } = new();

        /// <summary>
        /// All typedefs names in this HashSet will be ignored in the generation process.
        /// </summary>
        public HashSet<string> IgnoredTypedefs { get; set; } = new();

        /// <summary>
        /// All delegates names in this HashSet will be ignored in the generation process.
        /// </summary>
        public HashSet<string> IgnoredDelegates { get; set; } = new();

        /// <summary>
        /// All constants names in this HashSet will be ignored in the generation process.
        /// </summary>
        public HashSet<string> IgnoredConstants { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on functions.
        /// </summary>
        public HashSet<string> AllowedFunctions { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on types.
        /// </summary>
        public HashSet<string> AllowedTypes { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on enums.
        /// </summary>
        public HashSet<string> AllowedEnums { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on typedefs.
        /// </summary>
        public HashSet<string> AllowedTypedefs { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on delegates.
        /// </summary>
        public HashSet<string> AllowedDelegates { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on constants.
        /// </summary>
        public HashSet<string> AllowedConstants { get; set; } = new();

        /// <summary>
        /// Allows to define or overwrite COM object Guids. where the Key is the com object name and the value the guid.
        /// </summary>
        public Dictionary<string, string> IIDMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify constants
        /// </summary>
        public List<ConstantMapping> ConstantMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify enums
        /// </summary>
        public List<EnumMapping> EnumMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify functions
        /// </summary>
        public List<FunctionMapping> FunctionMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify handles
        /// </summary>
        public List<HandleMapping> HandleMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify classes
        /// </summary>
        public List<TypeMapping> ClassMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify delegates
        /// </summary>
        public List<DelegateMapping> DelegateMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify arrays
        /// </summary>
        public List<ArrayMapping> ArrayMappings { get; set; } = new();

        /// <summary>
        /// Allows to modify names fully or partially. newName = newName.Replace(item.Key, item.Value, StringComparison.InvariantCultureIgnoreCase);
        /// </summary>
        public Dictionary<string, string> NameMappings { get; set; } = new()
        {
        };

        /// <summary>
        /// Maps type Key to type Value.
        /// </summary>
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

        public List<string> Usings { get; set; } = new();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention ConstantNamingConvention { get; set; } = NamingConvention.Unknown;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention EnumNamingConvention { get; set; } = NamingConvention.PascalCase;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention EnumItemNamingConvention { get; set; } = NamingConvention.PascalCase;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention ExtensionNamingConvention { get; set; } = NamingConvention.PascalCase;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention FunctionNamingConvention { get; set; } = NamingConvention.PascalCase;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention HandleNamingConvention { get; set; } = NamingConvention.PascalCase;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention TypeNamingConvention { get; set; } = NamingConvention.PascalCase;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention DelegateNamingConvention { get; set; } = NamingConvention.PascalCase;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention ParameterNamingConvention { get; set; } = NamingConvention.CamelCase;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention MemberNamingConvention { get; set; } = NamingConvention.PascalCase;

        public List<string> IncludeFolders { get; set; } = new();

        public List<string> SystemIncludeFolders { get; set; } = new();

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

        private readonly ConcurrentDictionary<CppType, string> typeNameCache = new();

        public string GetCsTypeName(CppType? type, bool isPointer = false)
        {
            if (type == null)
                return string.Empty;

            if (typeNameCache.TryGetValue(type, out var typeName))
                return typeName;

            var name = GetCsTypeNameInternal(type, isPointer);
            typeNameCache.TryAdd(type, name);
            return name;
        }

        public string GetCsTypeName(CppPointerType type)
        {
            if (type == null)
                return string.Empty;

            if (typeNameCache.TryGetValue(type, out var typeName))
                return typeName;

            var name = GetCsTypeNameInternal(type);
            typeNameCache.TryAdd(type, name);
            return name;
        }

        public string GetCsTypeName(CppPrimitiveType type, bool isPointer)
        {
            if (type == null)
                return string.Empty;

            if (typeNameCache.TryGetValue(type, out var typeName))
                return typeName;

            var name = GetCsTypeNameInternal(type, isPointer);
            typeNameCache.TryAdd(type, name);
            return name;
        }

        private string GetCsTypeNameInternal(CppType? type, bool isPointer = false)
        {
            if (type is CppPrimitiveType primitiveType)
            {
                return GetCsTypeNameInternal(primitiveType, isPointer);
            }

            if (type is CppQualifiedType qualifiedType)
            {
                return GetCsTypeNameInternal(qualifiedType.ElementType, isPointer);
            }

            if (type is CppReferenceType referenceType)
            {
                return GetCsTypeNameInternal(referenceType.ElementType, true);
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
                var pointerName = GetCsTypeNameInternal(pointerType);
                if (isPointer)
                    return pointerName + "*";

                return pointerName;
            }

            if (type is CppArrayType arrayType)
            {
                var arrayName = GetCsTypeNameInternal(arrayType.ElementType, false);
                return arrayName + "*";
            }

            if (type is CppUnexposedType unexposedType)
            {
                throw new();
            }

            return string.Empty;
        }

        private string GetCsTypeNameInternal(CppPointerType pointerType)
        {
            if (pointerType.ElementType is CppQualifiedType qualifiedType)
            {
                if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
                {
                    return GetCsTypeNameInternal(primitiveType, true);
                }
                else if (qualifiedType.ElementType is CppClass @classType)
                {
                    return GetCsTypeNameInternal(@classType, true);
                }
                else if (qualifiedType.ElementType is CppPointerType subPointerType)
                {
                    return GetCsTypeNameInternal(subPointerType.ElementType, true) + "*";
                }
                else if (qualifiedType.ElementType is CppTypedef typedef)
                {
                    return GetCsTypeNameInternal(typedef, true);
                }
                else if (qualifiedType.ElementType is CppEnum @enum)
                {
                    return GetCsTypeNameInternal(@enum, true);
                }

                return GetCsTypeNameInternal(qualifiedType.ElementType, true);
            }

            if (pointerType.ElementType is CppFunctionType functionType)
            {
                return $"delegate*<{GetNamelessParameterSignature(functionType.Parameters, false)}>";
            }

            if (pointerType.ElementType is CppPointerType subPointer)
            {
                return GetCsTypeNameInternal(subPointer) + "*";
            }

            return GetCsTypeNameInternal(pointerType.ElementType, true);
        }

        private string GetCsTypeNameInternal(CppPrimitiveType primitiveType, bool isPointer)
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

        private readonly ConcurrentDictionary<CppType, string> wrapperTypeNameCache = new();

        public string GetCsWrapperTypeName(CppType? type, bool isPointer = false)
        {
            if (type == null)
                return string.Empty;

            if (wrapperTypeNameCache.TryGetValue(type, out var typeName))
                return typeName;

            var name = GetCsWrapperTypeNameInternal(type, isPointer);
            wrapperTypeNameCache.TryAdd(type, name);
            return name;
        }

        public string GetCsWrapperTypeName(CppPointerType type)
        {
            if (type == null)
                return string.Empty;

            if (wrapperTypeNameCache.TryGetValue(type, out var typeName))
                return typeName;

            var name = GetCsWrapperTypeNameInternal(type);
            wrapperTypeNameCache.TryAdd(type, name);
            return name;
        }

        public string GetCsWrapperTypeName(CppPrimitiveType type, bool isPointer)
        {
            if (type == null)
                return string.Empty;

            if (wrapperTypeNameCache.TryGetValue(type, out var typeName))
                return typeName;

            var name = GetCsWrapperTypeNameInternal(type, isPointer);
            wrapperTypeNameCache.TryAdd(type, name);
            return name;
        }

        private string GetCsWrapperTypeNameInternal(CppType? type, bool isPointer = false)
        {
            if (type is CppPrimitiveType primitiveType)
            {
                return GetCsWrapperTypeNameInternal(primitiveType, isPointer);
            }

            if (type is CppQualifiedType qualifiedType)
            {
                return GetCsWrapperTypeNameInternal(qualifiedType.ElementType, isPointer);
            }

            if (type is CppReferenceType referenceType)
            {
                return GetCsWrapperTypeNameInternal(referenceType.ElementType, true);
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
                return GetCsWrapperTypeNameInternal(pointerType);
            }

            if (type is CppArrayType arrayType && arrayType.Size > 0)
            {
                if (TryGetArrayMapping(arrayType, out string? mapping))
                {
                    return mapping;
                }

                return GetCsWrapperTypeNameInternal(arrayType.ElementType, true);
            }
            else if (type is CppArrayType arrayType1 && arrayType1.Size < 0)
            {
                var arrayName = GetCsTypeName(arrayType1.ElementType, false);
                return arrayName + "*";
            }

            return string.Empty;
        }

        private string GetCsWrapperTypeNameInternal(CppPrimitiveType primitiveType, bool isPointer)
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

        private string GetCsWrapperTypeNameInternal(CppPointerType pointerType)
        {
            if (pointerType.ElementType is CppQualifiedType qualifiedType)
            {
                if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
                {
                    return GetCsWrapperTypeNameInternal(primitiveType, true);
                }
                else if (qualifiedType.ElementType is CppClass @classType)
                {
                    return GetCsWrapperTypeNameInternal(@classType, true);
                }
                else if (qualifiedType.ElementType is CppPointerType subPointerType)
                {
                    return GetCsWrapperTypeNameInternal(subPointerType, true) + "*";
                }
                else if (qualifiedType.ElementType is CppTypedef typedef)
                {
                    return GetCsWrapperTypeNameInternal(typedef, true);
                }
                else if (qualifiedType.ElementType is CppEnum @enum)
                {
                    return GetCsWrapperTypeNameInternal(@enum, true);
                }

                return GetCsWrapperTypeNameInternal(qualifiedType.ElementType, true);
            }

            if (pointerType.ElementType is CppFunctionType functionType)
            {
                return $"delegate*<{GetNamelessParameterSignature(functionType.Parameters, false)}>";
            }

            if (pointerType.ElementType is CppPointerType subPointer)
            {
                return GetCsWrapperTypeNameInternal(subPointer) + "*";
            }

            return GetCsWrapperTypeNameInternal(pointerType.ElementType, true);
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

        private readonly ConcurrentDictionary<string, string> parameterNameCache = new();

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
            if (parameterNameCache.TryGetValue(name, out var newName))
            {
                return newName;
            }

            newName = NamingHelper.ConvertTo(name, ParameterNamingConvention);

            if (Keywords.Contains(newName))
            {
                return "@" + newName;
            }

            parameterNameCache.TryAdd(name, newName);

            return newName;
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
                        var enumItemName = GetEnumName(value, enumNamePrefix);

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

        public string GetConstantName(string value)
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

        public string GetEnumName(string value, EnumPrefix enumPrefix)
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

        public string GetExtensionName(string value, string extensionPrefix)
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

        public string GetFieldName(string name)
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

        public bool WriteCsSummary(string? comment, CodeWriter writer)
        {
            if (comment == null)
            {
                if (GeneratePlaceholderComments)
                {
                    writer.WriteLine("/// <summary>");
                    writer.WriteLine("/// To be documented.");
                    writer.WriteLine("/// </summary>");
                    return true;
                }
                return false;
            }

            var lines = comment.Replace("/", string.Empty).Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            writer.WriteLine("/// <summary>");
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                writer.WriteLine($"/// {new XText(line)}<br/>");
            }
            writer.WriteLine($"/// </summary>");
            return true;
        }

        public bool WriteCsSummary(CppComment? comment, CodeWriter writer)
        {
            bool result = false;
            if (comment is CppCommentFull full)
            {
                writer.WriteLine("/// <summary>");
                for (int i = 0; i < full.Children.Count; i++)
                {
                    WriteCsSummary(full.Children[i], writer);
                }
                writer.WriteLine("/// </summary>");
                result = true;
            }
            if (comment is CppCommentParagraph paragraph)
            {
                for (int i = 0; i < paragraph.Children.Count; i++)
                {
                    WriteCsSummary(paragraph.Children[i], writer);
                }
                result = true;
            }

            if (comment is CppCommentBlockCommand blockCommand)
            {
            }
            if (comment is CppCommentVerbatimBlockCommand verbatimBlockCommand)
            {
            }

            if (comment is CppCommentVerbatimBlockLine verbatimBlockLine)
            {
            }

            if (comment is CppCommentVerbatimLine line)
            {
            }

            if (comment is CppCommentParamCommand paramCommand)
            {
                // TODO: add param comment support
            }

            if (comment is CppCommentInlineCommand inlineCommand)
            {
                // TODO: add inline comment support
            }

            if (comment is CppCommentText text)
            {
                writer.WriteLine($"/// " + text.Text + "<br/>");
                result = true;
            }

            if (comment == null || comment.Kind == CppCommentKind.Null)
            {
            }

            if (!result && GeneratePlaceholderComments)
            {
                writer.WriteLine("/// <summary>");
                writer.WriteLine("/// To be documented.");
                writer.WriteLine("/// </summary>");
                return true;
            }

            return result;
        }

        public void WriteCsSummary(CppComment? cppComment, out string? comment)
        {
            comment = null;
            StringBuilder sb = new();
            if (cppComment is CppCommentFull full)
            {
                sb.AppendLine("/// <summary>");
                for (int i = 0; i < full.Children.Count; i++)
                {
                    WriteCsSummary(full.Children[i], out var subComment);
                    sb.Append(subComment);
                }
                sb.AppendLine("/// </summary>");
                comment = sb.ToString();
                return;
            }
            if (cppComment is CppCommentParagraph paragraph)
            {
                for (int i = 0; i < paragraph.Children.Count; i++)
                {
                    WriteCsSummary(paragraph.Children[i], out var subComment);
                    sb.Append(subComment);
                }
                comment = sb.ToString();
                return;
            }
            if (cppComment is CppCommentText text)
            {
                sb.AppendLine($"/// " + text.Text + "<br/>");
                comment = sb.ToString();
                return;
            }

            if (cppComment is CppCommentBlockCommand blockCommand)
            {
                comment = null;
            }
            if (cppComment is CppCommentVerbatimBlockCommand verbatimBlockCommand)
            {
                comment = null;
            }

            if (cppComment is CppCommentVerbatimBlockLine verbatimBlockLine)
            {
                comment = null;
            }

            if (cppComment is CppCommentVerbatimLine line)
            {
                comment = null;
            }

            if (cppComment is CppCommentParamCommand paramCommand)
            {
                // TODO: add param comment support
                comment = null;
            }

            if (cppComment is CppCommentInlineCommand inlineCommand)
            {
                // TODO: add inline comment support
                comment = null;
            }

            if (cppComment == null || cppComment.Kind == CppCommentKind.Null)
            {
                comment = null;
            }

            if (comment == null && GeneratePlaceholderComments)
            {
                sb.AppendLine("/// <summary>");
                sb.AppendLine("/// To be documented.");
                sb.AppendLine("/// </summary>");
                comment = sb.ToString();
                return;
            }
        }
    }
}