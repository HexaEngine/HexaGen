namespace HexaGen
{
    using CppAst;
    using HexaGen.Core;
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

    public class UnexposedTypeException : Exception
    {
        public UnexposedTypeException(CppUnexposedType unexposedType) : base($"Cannot handle unexposed type '{unexposedType}'")
        {
        }
    }

    public partial class CsCodeGeneratorSettings : IGeneratorSettings
    {
        public static CsCodeGeneratorSettings Default { get; } = new CsCodeGeneratorSettings()
        {
            TypeMappings = new()
            {
                {"uint8_t", "byte"},
                {"uint16_t", "ushort"},
                {"uint32_t", "uint"},
                {"uint64_t", "ulong"},
                {"int8_t", "sbyte"},
                {"int32_t", "int"},
                {"int16_t", "short"},
                {"int64_t", "long"},
                {"int64_t*", "long*"},
                {"unsigned char", "byte"},
                {"signed char", "sbyte"},
                {"char", "byte"},
                {"size_t", "nuint"},
                {"bool", "int"},
                {"BOOL", "int"},
                {"BYTE", "byte"},
                {"Uint8", "byte"},
                {"Uint16", "ushort"},
                {"Uint32", "uint"},
                {"Uint64", "ulong"},
                {"Sint8", "sbyte"},
                {"Sint16", "short"},
                {"Sint32", "int"},
                {"Sint64", "long"},
                {"UCHAR", "byte"},
                {"WCHAR", "char"},
                {"UINT8", "byte"},
                {"USHORT", "ushort"},
                {"UINT16", "ushort"},
                {"UINT", "uint"},
                {"UINT32", "uint"},
                {"ULONG", "uint"},
                {"DWORD", "uint"},
                {"WORD", "short"},
                {"INT", "int"},
                {"INT32", "int"},
                {"ULONGLONG", "ulong"},
                {"UINT64", "ulong"},
                {"LONGLONG", "long"},
                {"LARGE_INTEGER", "long"},
                {"FLOAT", "float"},
                {"LPCSTR", "byte*"},
                {"LPCWSTR", "char*"},
                {"LPSTR", "byte*"},
                {"LPWSTR", "char*"},
                {"BSTR", "void*"},
                {"GUID", "Guid"},
                {"HWND", "nint"},
                {"LPCVOID", "void*"},
                {"LPVOID", "void*"},
                {"SIZE", "nint"},
                {"SIZE_T", "nuint"},
                {"LUID", "Luid"},
                {"IID", "Guid"},
                {"RECT", "Rect32"},
                {"POINT", "Point32"},
                {"WPARAM", "nuint"},
                {"LPARAM", "nint"},
                {"HDC", "nint"},
                {"HINSTANCE", "nint"},
            },
            Keywords = new()
            {
                "abstract",
                "as",
                "base",
                "bool",
                "break",
                "byte",
                "case",
                "catch",
                "char",
                "checked",
                "class",
                "const",
                "continue",
                "decimal",
                "default",
                "delegate",
                "do",
                "double",
                "else",
                "enum",
                "event",
                "explicit",
                "extern",
                "false",
                "finally",
                "fixed",
                "float",
                "for",
                "foreach",
                "goto",
                "if",
                "implicit",
                "in",
                "int",
                "interface",
                "internal",
                "is",
                "lock",
                "long",
                "namespace",
                "new",
                "null",
                "object",
                "operator",
                "out",
                "override",
                "params",
                "private",
                "protected",
                "public",
                "readonly",
                "ref",
                "return",
                "sbyte",
                "sealed",
                "short",
                "sizeof",
                "stackalloc",
                "static",
                "string",
                "struct",
                "switch",
                "this",
                "throw",
                "true",
                "try",
                "typeof",
                "uint",
                "ulong",
                "unchecked",
                "unsafe",
                "ushort",
                "using",
                "using static",
                "virtual",
                "void",
                "volatile",
                "while",
                "yield"
            },
            IgnoredTypedefs =
            {
                "HWND",
                "nint"
            }
        };

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

            foreach (var item in Default.TypeMappings)
            {
                result.TypeMappings.TryAdd(item.Key, item.Value);
            }

            foreach (var item in Default.Keywords)
            {
                if (!result.Keywords.Contains(item))
                {
                    result.Keywords.Add(item);
                }
            }

            foreach (var item in Default.IgnoredTypedefs)
            {
                if (!result.IgnoredTypedefs.Contains(item))
                {
                    result.IgnoredTypedefs.Add(item);
                }
            }

            if (!result.EnableExperimentalOptions)
            {
                result.GenerateConstructorsForStructs = false;
                result.UseVTable = false;
            }

            if (result.UseVTable)
            {
                result.UseLibraryImport = false;
            }

            result.Save(file);
            return result;
        }

        /// <summary>
        /// The namespace of the generated wrapper. (Default <see cref="string.Empty"/>)
        /// </summary>
        public string Namespace { get; set; } = string.Empty;

        /// <summary>
        /// The api name of the wrapper. (Used for exported functions and macros) (Default <see cref="string.Empty"/>)
        /// </summary>
        public string ApiName { get; set; } = string.Empty;

        /// <summary>
        /// The name of the .dll or .so or .dylib. (Default <see cref="string.Empty"/>)
        /// </summary>
        public string LibName { get; set; } = string.Empty;

        /// <summary>
        /// The log level of the generator. (Default <see cref="LogSevertiy.Warning"/>)
        /// </summary>
        public LogSevertiy LogLevel { get; set; } = LogSevertiy.Warning;

        /// <summary>
        /// The log level of the Clang Compiler. (Default <see cref="LogSevertiy.Error"/>)
        /// </summary>
        public LogSevertiy CppLogLevel { get; set; } = LogSevertiy.Error;

        /// <summary>
        /// This allows to use the (EXPERIMENTAL) options, otherwise they will be set back to false. (Default: <see langword="false"/>)
        /// </summary>
        public bool EnableExperimentalOptions { get; set; } = false;

        /// <summary>
        /// This option generates the sizes of the structs. (Default: <see langword="false"/>)
        /// </summary>
        ///
        public bool GenerateSizeOfStructs { get; set; } = false;

        /// <summary>
        /// The generator will generate default constructors for all structs. (Default: <see langword="true"/>) (EXPERIMENTAL)
        /// </summary>
        public bool GenerateConstructorsForStructs { get; set; } = true;

        /// <summary>
        /// This option makes that the delegates are just void* and not delegate pointer (<see cref="delegate*&lt;void&gt;"/>) (Default: <see langword="true"/>)
        /// </summary>
        public bool DelegatesAsVoidPointer { get; set; } = true;

        /// <summary>
        /// This option makes the resulting wrapper more "safe" so you don't need unsafe blocks everywhere. (Default: <see langword="false"/>)
        /// </summary>
        public bool WrapPointersAsHandle { get; set; } = false;

        /// <summary>
        /// This causes the code generator to generate summary xml comments if it's missing with the text "To be documented." (Default: <see langword="true"/>)
        /// </summary>
        public bool GeneratePlaceholderComments { get; set; } = true;

        /// <summary>
        /// This causes the code generator to use <see cref="System.Runtime.InteropServices.LibraryImportAttribute"/> instead of <see cref="System.Runtime.InteropServices.DllImportAttribute"/>
        /// </summary>
        public bool UseLibraryImport { get; set; } = true;

        /// <summary>
        /// This causes the code generator to use a VTable instead of <see cref="System.Runtime.InteropServices.LibraryImportAttribute"/> <see cref="System.Runtime.InteropServices.DllImportAttribute"/> (EXPERIMENTAL)
        /// </summary>
        public bool UseVTable { get; set; } = false;

        /// <summary>
        /// The generator will generate [NativeName] attributes.
        /// </summary>
        public bool GenerateMetadata { get; set; } = false;

        /// <summary>
        /// Enables generation for constants (CPP: Macros) (Default: <see langword="true"/>)
        /// </summary>
        public bool GenerateConstants { get; set; } = true;

        /// <summary>
        /// Enables generation for enums. (Default: <see langword="true"/>)
        /// </summary>
        public bool GenerateEnums { get; set; } = true;

        /// <summary>
        /// Enables generation for extensions, this option is very useful if you have an handle type or COM objects. (Default: <see langword="true"/>)
        /// </summary>
        public bool GenerateExtensions { get; set; } = true;

        /// <summary>
        /// Enables generation for functions. This option generates the public API dllexport functions. (Default: <see langword="true"/>)
        /// </summary>
        public bool GenerateFunctions { get; set; } = true;

        /// <summary>
        /// Enables generation for handles. (CPP: Typedefs) (Default: <see langword="true"/>)
        /// </summary>
        public bool GenerateHandles { get; set; } = true;

        /// <summary>
        /// Enables generation for types. This includes COM objects and normal C-Structs. (Default: <see langword="true"/>)
        /// </summary>
        public bool GenerateTypes { get; set; } = true;

        /// <summary>
        /// Enables generation for delegates. (Default: <see langword="true"/>)
        /// </summary>
        public bool GenerateDelegates { get; set; } = true;

        /// <summary>
        /// This option controls the bool type eg. 8Bit Bool and 32Bit Bool. (Default: <see cref="BoolType.Bool8"/>)
        /// </summary>
        public BoolType BoolType { get; set; } = BoolType.Bool8;

        /// <summary>
        /// Allows to map names for constants. (Default: Empty)
        /// </summary>
        public Dictionary<string, string> KnownConstantNames { get; set; } = new();

        /// <summary>
        /// Allows to map names for enums. (Default: Empty)
        /// </summary>
        public Dictionary<string, string> KnownEnumValueNames { get; set; } = new();

        /// <summary>
        /// Allows to map names for enum prefixes. (Default: Empty)
        /// </summary>
        public Dictionary<string, string> KnownEnumPrefixes { get; set; } = new();

        /// <summary>
        /// Allows to map names for extension prefixes. (Default: Empty)
        /// </summary>
        public Dictionary<string, string> KnownExtensionPrefixes { get; set; } = new();

        /// <summary>
        /// Allows to map names for extension. (Default: Empty)
        /// </summary>
        public Dictionary<string, string> KnownExtensionNames { get; set; } = new();

        /// <summary>
        /// Allows to map names for default values. (Default: Empty)
        /// </summary>
        public Dictionary<string, string> KnownDefaultValueNames { get; set; } = new();

        /// <summary>
        /// Allows to define constructors functions for types. (Default: Empty)
        /// </summary>
        public Dictionary<string, List<string>> KnownConstructors { get; set; } = new();

        /// <summary>
        /// Allows to define member functions for types. (Default: Empty)
        /// </summary>
        public Dictionary<string, List<string>> KnownMemberFunctions { get; set; } = new();

        /// <summary>
        /// Ignores parts like OpenAl in OpenALFunction -> Function. (Default: Empty)
        /// </summary>
        public HashSet<string> IgnoredParts { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// C# keywords that would cause issues with naming. (Default: all common C# keywords)
        /// </summary>
        public HashSet<string> Keywords { get; set; } = new();

        /// <summary>
        /// All function names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        public HashSet<string> IgnoredFunctions { get; set; } = new();

        /// <summary>
        /// All types names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        public HashSet<string> IgnoredTypes { get; set; } = new();

        /// <summary>
        /// All enums names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        public HashSet<string> IgnoredEnums { get; set; } = new();

        /// <summary>
        /// All typedefs names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        public HashSet<string> IgnoredTypedefs { get; set; } = new();

        /// <summary>
        /// All delegates names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        public HashSet<string> IgnoredDelegates { get; set; } = new();

        /// <summary>
        /// All constants names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        public HashSet<string> IgnoredConstants { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on functions. (Default: Empty)
        /// </summary>
        public HashSet<string> AllowedFunctions { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on types. (Default: Empty)
        /// </summary>
        public HashSet<string> AllowedTypes { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on enums. (Default: Empty)
        /// </summary>
        public HashSet<string> AllowedEnums { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on typedefs. (Default: Empty)
        /// </summary>
        public HashSet<string> AllowedTypedefs { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on delegates. (Default: Empty)
        /// </summary>
        public HashSet<string> AllowedDelegates { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on constants. (Default: Empty)
        /// </summary>
        public HashSet<string> AllowedConstants { get; set; } = new();

        /// <summary>
        /// Allows to define or overwrite COM object Guids. where the Key is the com object name and the value the guid. (Default: Empty)
        /// </summary>
        public Dictionary<string, string> IIDMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify constants. (Default: Empty)
        /// </summary>
        public List<ConstantMapping> ConstantMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify enums. (Default: Empty)
        /// </summary>
        public List<EnumMapping> EnumMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify functions. (Default: Empty)
        /// </summary>
        public List<FunctionMapping> FunctionMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify handles. (Default: Empty)
        /// </summary>
        public List<HandleMapping> HandleMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify classes. (Default: Empty)
        /// </summary>
        public List<TypeMapping> ClassMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify delegates. (Default: Empty)
        /// </summary>
        public List<DelegateMapping> DelegateMappings { get; set; } = new();

        /// <summary>
        /// Allows to inject data and modify arrays. (Default: Empty)
        /// </summary>
        public List<ArrayMapping> ArrayMappings { get; set; } = new();

        /// <summary>
        /// Allows to modify names fully or partially. newName = newName.Replace(item.Key, item.Value, StringComparison.InvariantCultureIgnoreCase); (Default: Empty)
        /// </summary>
        public Dictionary<string, string> NameMappings { get; set; } = new();

        /// <summary>
        /// Maps type Key to type Value. (Default: a list with common types, like size_t : nuint)
        /// </summary>
        public Dictionary<string, string> TypeMappings { get; set; } = new();

        /// <summary>
        /// Allows to add or manage usings. (Default: Empty)
        /// </summary>
        public List<string> Usings { get; set; } = new();

        /// <summary>
        /// The naming convention for constants, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.Unknown"/>)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention ConstantNamingConvention { get; set; } = NamingConvention.Unknown;

        /// <summary>
        /// The naming convention for enums, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention EnumNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for enum items, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention EnumItemNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for extension functions, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention ExtensionNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for functions, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention FunctionNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for handles, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention HandleNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for classes and structs, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention TypeNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for delegates, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention DelegateNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for parameters, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.CamelCase"/>)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention ParameterNamingConvention { get; set; } = NamingConvention.CamelCase;

        /// <summary>
        /// The naming convention for members, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NamingConvention MemberNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// List of the include folders. (Default: Empty)
        /// </summary>
        public List<string> IncludeFolders { get; set; } = new();

        /// <summary>
        /// List of the system include folders. (Default: Empty)
        /// </summary>
        public List<string> SystemIncludeFolders { get; set; } = new();

        /// <summary>
        /// List of macros passed to CppAst. (Default: Empty)
        /// </summary>
        public List<string> Defines { get; set; } = new();

        /// <summary>
        /// List of the additional arguments passed directly to the C++ Clang compiler. (Default: Empty)
        /// </summary>
        public List<string> AdditionalArguments { get; set; } = new();

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
                if (typedef.ElementType is CppPrimitiveType cppPrimitive)
                {
                    var csPrimitiveName = GetCsTypeNameInternal(cppPrimitive);
                    if (!string.IsNullOrEmpty(csPrimitiveName))
                    {
                        if (isPointer)
                        {
                            return $"{csPrimitiveName}*";
                        }
                        return csPrimitiveName;
                    }
                }

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
                throw new UnexposedTypeException(unexposedType);
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
                if (functionType.Parameters.Count == 0)
                {
                    return $"delegate*<{GetCsTypeNameInternal(functionType.ReturnType)}>";
                }
                else
                {
                    return $"delegate*<{GetNamelessParameterSignature(functionType.Parameters, false)}, {GetCsTypeNameInternal(functionType.ReturnType)}>";
                }
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

        /// <summary>
        /// This method will return <see langword="ref"/> <see cref="T"/> instead of <see cref="T"/>*
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isPointer"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This method will return <see langword="ref"/> <see cref="T"/> instead of <see cref="T"/>*
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This method will return <see langword="ref"/> <see cref="T"/> instead of <see cref="T"/>*
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isPointer"></param>
        /// <returns></returns>
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
                if (typedef.ElementType is CppPrimitiveType cppPrimitive)
                {
                    var csPrimitiveName = GetCsWrapperTypeNameInternal(cppPrimitive);
                    if (!string.IsNullOrEmpty(csPrimitiveName))
                    {
                        if (isPointer)
                        {
                            return $"ref {csPrimitiveName}";
                        }
                        return csPrimitiveName;
                    }
                }

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
                if (functionType.Parameters.Count == 0)
                {
                    return $"delegate*<{GetCsWrapperTypeNameInternal(functionType.ReturnType)}>";
                }
                else
                {
                    return $"delegate*<{GetNamelessParameterSignature(functionType.Parameters, false)}, {GetCsWrapperTypeNameInternal(functionType.ReturnType)}>";
                }
            }

            if (pointerType.ElementType is CppPointerType subPointer)
            {
                return GetCsWrapperTypeNameInternal(subPointer) + "*";
            }

            return GetCsWrapperTypeNameInternal(pointerType.ElementType, true);
        }

        private readonly ConcurrentDictionary<CppType, string> wrappedPointerTypeNameCache = new();

        /// <summary>
        /// This method will return <see cref="Pointer&lt;T&gt;"/> instead of <see cref="T"/>*
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isPointer"></param>
        /// <returns></returns>
        public string GetCsWrappedPointerTypeName(CppType? type, bool isPointer = false)
        {
            if (type == null)
                return string.Empty;

            if (wrappedPointerTypeNameCache.TryGetValue(type, out var typeName))
                return typeName;

            var name = GetCsWrappedPointerTypeNameInternal(type, isPointer);
            wrappedPointerTypeNameCache.TryAdd(type, name);
            return name;
        }

        /// <summary>
        /// This method will return <see cref="Pointer&lt;T&gt;"/> instead of <see cref="T"/>*
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetCsWrappedPointerTypeName(CppPointerType type)
        {
            if (type == null)
                return string.Empty;

            if (wrappedPointerTypeNameCache.TryGetValue(type, out var typeName))
                return typeName;

            var name = GetCsWrappedPointerTypeNameInternal(type);
            wrappedPointerTypeNameCache.TryAdd(type, name);
            return name;
        }

        /// <summary>
        /// This method will return <see cref="Pointer&lt;T&gt;"/> instead of <see cref="T"/>*
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isPointer"></param>
        /// <returns></returns>
        public string GetCsWrappedPointerTypeName(CppPrimitiveType type, bool isPointer)
        {
            if (type == null)
                return string.Empty;

            if (wrappedPointerTypeNameCache.TryGetValue(type, out var typeName))
                return typeName;

            var name = GetCsWrappedPointerTypeNameInternal(type, isPointer);
            wrappedPointerTypeNameCache.TryAdd(type, name);
            return name;
        }

        private string GetCsWrappedPointerTypeNameInternal(CppType? type, bool isPointer = false)
        {
            if (type is CppPrimitiveType primitiveType)
            {
                return GetCsWrappedPointerTypeNameInternal(primitiveType, isPointer);
            }

            if (type is CppQualifiedType qualifiedType)
            {
                return GetCsWrappedPointerTypeNameInternal(qualifiedType.ElementType, isPointer);
            }

            if (type is CppReferenceType referenceType)
            {
                return GetCsWrappedPointerTypeNameInternal(referenceType.ElementType, true);
            }

            if (type is CppEnum enumType)
            {
                var enumCsName = GetCsCleanName(enumType.Name);
                if (isPointer)
                    return $"Pointer<{enumCsName}>";

                return enumCsName;
            }

            if (type is CppTypedef typedef)
            {
                if (typedef.ElementType is CppPrimitiveType cppPrimitive)
                {
                    var csPrimitiveName = GetCsWrappedPointerTypeNameInternal(cppPrimitive);
                    if (!string.IsNullOrEmpty(csPrimitiveName))
                    {
                        if (isPointer)
                        {
                            return $"Pointer<{csPrimitiveName}>";
                        }

                        return csPrimitiveName;
                    }
                }

                var typeDefCsName = GetCsCleanName(typedef.Name);
                if (typedef.IsDelegate())
                {
                    return typeDefCsName;
                }
                if (isPointer && typeDefCsName == "void")
                    return "nint";
                if (isPointer)
                    return $"Pointer<{typeDefCsName}>";

                return typeDefCsName;
            }

            if (type is CppClass @class)
            {
                var className = GetCsCleanName(@class.Name);
                if (isPointer && className == "void")
                    return "nint";
                if (isPointer)
                    return $"Pointer<{className}>";

                return className;
            }

            if (type is CppPointerType pointerType)
            {
                return GetCsWrappedPointerTypeNameInternal(pointerType);
            }

            if (type is CppArrayType arrayType && arrayType.Size > 0)
            {
                if (TryGetArrayMapping(arrayType, out string? mapping))
                {
                    return mapping;
                }

                return GetCsWrappedPointerTypeNameInternal(arrayType.ElementType, true);
            }
            else if (type is CppArrayType arrayType1 && arrayType1.Size < 0)
            {
                var arrayName = GetCsTypeName(arrayType1.ElementType, false);
                return $"Pointer<{arrayName}>";
            }

            return string.Empty;
        }

        private string GetCsWrappedPointerTypeNameInternal(CppPrimitiveType primitiveType, bool isPointer)
        {
            switch (primitiveType.Kind)
            {
                case CppPrimitiveKind.Void:
                    return isPointer ? "nint" : "nint";

                case CppPrimitiveKind.Char:
                    return isPointer ? "Pointer<byte>" : "byte";

                case CppPrimitiveKind.Bool:
                    return isPointer ? $"Pointer<{GetBoolType()}>" : "bool";

                case CppPrimitiveKind.WChar:
                    return isPointer ? "Pointer<char>" : "char";

                case CppPrimitiveKind.Short:
                    return isPointer ? "Pointer<short>" : "short";

                case CppPrimitiveKind.Int:
                    return isPointer ? "Pointer<int>" : "int";

                case CppPrimitiveKind.LongLong:
                    return isPointer ? "Pointer<long>" : "long";

                case CppPrimitiveKind.UnsignedChar:
                    return isPointer ? "Pointer<byte>" : "byte";

                case CppPrimitiveKind.UnsignedShort:
                    return isPointer ? "Pointer<ushort>" : "ushort";

                case CppPrimitiveKind.UnsignedInt:
                    return isPointer ? "Pointer<uint>" : "uint";

                case CppPrimitiveKind.UnsignedLongLong:
                    return isPointer ? "Pointer<ulong>" : "ulong";

                case CppPrimitiveKind.Float:
                    return isPointer ? "Pointer<float>" : "float";

                case CppPrimitiveKind.Double:
                    return isPointer ? "Pointer<double>" : "double";

                case CppPrimitiveKind.LongDouble:
                    break;

                default:
                    return string.Empty;
            }

            return string.Empty;
        }

        private string GetCsWrappedPointerTypeNameInternal(CppPointerType pointerType)
        {
            if (pointerType.ElementType is CppQualifiedType qualifiedType)
            {
                if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
                {
                    return GetCsWrappedPointerTypeNameInternal(primitiveType, true);
                }
                else if (qualifiedType.ElementType is CppClass @classType)
                {
                    return GetCsWrappedPointerTypeNameInternal(@classType, true);
                }
                else if (qualifiedType.ElementType is CppPointerType subPointerType)
                {
                    return $"Pointer<{GetCsWrappedPointerTypeNameInternal(subPointerType, true)}>";
                }
                else if (qualifiedType.ElementType is CppTypedef typedef)
                {
                    return GetCsWrappedPointerTypeNameInternal(typedef, true);
                }
                else if (qualifiedType.ElementType is CppEnum @enum)
                {
                    return GetCsWrappedPointerTypeNameInternal(@enum, true);
                }

                return GetCsWrappedPointerTypeNameInternal(qualifiedType.ElementType, true);
            }

            if (pointerType.ElementType is CppFunctionType functionType)
            {
                if (functionType.Parameters.Count == 0)
                {
                    return $"delegate*<{GetCsWrapperTypeNameInternal(functionType.ReturnType)}>";
                }
                else
                {
                    return $"delegate*<{GetNamelessParameterSignature(functionType.Parameters, false)}, {GetCsWrapperTypeNameInternal(functionType.ReturnType)}>";
                }
            }

            if (pointerType.ElementType is CppPointerType subPointer)
            {
                return $"Pointer<{GetCsWrappedPointerTypeNameInternal(subPointer)}>";
            }

            return GetCsWrappedPointerTypeNameInternal(pointerType.ElementType, true);
        }

        public string GetParameterSignature(IList<CppParameter> parameters, bool canUseOut, bool attributes = true, bool names = true)
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

                argumentBuilder.Append(paramCsTypeName);

                if (names)
                {
                    argumentBuilder.Append(' ').Append(paramCsName);
                }

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

        public string GetDelegateName(string name)
        {
            return GetCsCleanNameWithConvention(name, DelegateNamingConvention, false);
        }

        public string NormalizeParameterName(string name)
        {
            if (parameterNameCache.TryGetValue(name, out var newName))
            {
                return newName;
            }

            name = name.Trim('_');

            newName = NamingHelper.ConvertTo(name, ParameterNamingConvention);

            if (Keywords.Contains(newName))
            {
                newName = "@" + newName;
            }

            if (char.IsDigit(newName[0]))
            {
                newName = "_" + newName;
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

        public string GetEnumNameEx(string value, EnumPrefix enumPrefix)
        {
            return GetEnumName(value, enumPrefix);

            if (KnownEnumValueNames.TryGetValue(value, out string? knownName))
            {
                return knownName;
            }

            string[] parts = GetEnumNamePrefix(value).Parts;
            string[] prefixParts = enumPrefix.Parts;

            var name = GetCsCleanNameWithConvention(value, EnumNamingConvention, false);

            int lastRemoved = -1;
            for (int i = 0; i < prefixParts.Length; i++)
            {
                var part = prefixParts[i];
                if (name.StartsWith(part, StringComparison.InvariantCultureIgnoreCase) && name.Length - part.Length != 0)
                {
                    name = name[part.Length..];
                    lastRemoved = i;
                }
            }

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
                sb.Append(parts[^1].ToCamelCase());
            }

            string prettyName = sb.ToString();

            if (char.IsDigit(name[0]))
            {
                name = prefixParts[lastRemoved].ToCamelCase() + name;
            }

            return name; //char.IsNumber(prettyName[0]) ? parts[^1].ToCamelCase() + prettyName : prettyName;
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
                sb.Append(parts[^1].ToCamelCase());
            }

            string prettyName = sb.ToString();

            return char.IsNumber(prettyName[0]) ? parts[^1].ToCamelCase() + prettyName : prettyName;
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

        public bool WriteCsSummary(string? comment, ICodeWriter writer)
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

        public bool WriteCsSummary(string? comment, out string? com)
        {
            com = null;
            StringBuilder sb = new();
            if (comment == null)
            {
                if (GeneratePlaceholderComments)
                {
                    sb.AppendLine("/// <summary>");
                    sb.AppendLine("/// To be documented.");
                    sb.AppendLine("/// </summary>");
                    com = sb.ToString();
                    return true;
                }
                return false;
            }

            var lines = comment.Replace("/", string.Empty).Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            sb.AppendLine("/// <summary>");
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                sb.AppendLine($"/// {new XText(line)}<br/>");
            }
            sb.AppendLine($"/// </summary>");
            com = sb.ToString();
            return true;
        }

        public string? WriteCsSummary(string? comment)
        {
            WriteCsSummary(comment, out var result);
            return result;
        }

        public bool WriteCsSummary(CppComment? comment, ICodeWriter writer)
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

        public string? WriteCsSummary(CppComment? cppComment)
        {
            WriteCsSummary(cppComment, out var result);
            return result;
        }
    }
}