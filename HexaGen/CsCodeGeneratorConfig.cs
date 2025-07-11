namespace HexaGen
{
    using HexaGen.Core.Logging;
    using HexaGen.Metadata;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;

    public partial class CsCodeGeneratorConfig : IGeneratorConfig
    {
        [JsonIgnore, System.Text.Json.Serialization.JsonIgnore]
        public HeaderInjectionDelegate? HeaderInjector { get; set; }

        public BaseConfig BaseConfig { get; set; } = new();

        public static CsCodeGeneratorConfig Default { get; } = new CsCodeGeneratorConfig()
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
                {"LONG", "int"},
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
                {"HANDLE", "nint"},
                {"HMODULE", "nint"},
                {"HMONITOR", "nint"},
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
                {"HRESULT", "HResult"}
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
            },
            VaryingTypes = ["ReadOnlySpan<byte>", "string", "ref string"]
        };

        public static CsCodeGeneratorConfig Load(string file)
        {
            CsCodeGeneratorConfig result;
            if (File.Exists(file))
            {
                result = JsonConvert.DeserializeObject<CsCodeGeneratorConfig>(File.ReadAllText(file)) ?? new();
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
                result.Keywords.Add(item);
            }

            foreach (var item in Default.IgnoredTypedefs)
            {
                result.IgnoredTypedefs.Add(item);
            }

            foreach (var item in Default.VaryingTypes)
            {
                result.VaryingTypes.Add(item);
            }

            if (!result.EnableExperimentalOptions)
            {
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
        /// The log level of the generator. (Default <see cref="LogSeverity.Warning"/>)
        /// </summary>
        public LogSeverity LogLevel { get; set; } = LogSeverity.Warning;

        /// <summary>
        /// The log level of the Clang Compiler. (Default <see cref="LogSeverity.Error"/>)
        /// </summary>
        public LogSeverity CppLogLevel { get; set; } = LogSeverity.Error;

        /// <summary>
        /// This allows to use the (EXPERIMENTAL) options, otherwise they will be set back to false. (Default: <see langword="false"/>)
        /// </summary>
        public bool EnableExperimentalOptions { get; set; } = false;

        /// <summary>
        /// This option generates the sizes of the structs. (Default: <see langword="false"/>)
        /// </summary>
        public bool GenerateSizeOfStructs { get; set; } = false;

        /// <summary>
        /// The generator will generate default constructors for all structs. (Default: <see langword="true"/>)
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
        /// This causes the code generator to use <see cref="System.Runtime.InteropServices.LibraryImportAttribute"/>.
        /// </summary>
        public bool UseLibraryImport => ImportType == ImportType.LibraryImport;

        /// <summary>
        /// This causes the code generator to use a FunctionTable.
        /// </summary>
        public bool UseFunctionTable => ImportType == ImportType.FunctionTable;

        /// <summary>
        /// Specifies the existing entries in the function table.
        /// </summary>
        public List<CsFunctionTableEntry> FunctionTableEntries { get; set; } = [];

        public bool UseCustomContext { get; set; }

        public string GetLibraryNameFunctionName { get; set; } = "GetLibraryName";

        public string? GetLibraryExtensionFunctionName { get; set; } = null;

        /// <summary>
        /// Determines the import type. (Default: <see cref="ImportType.LibraryImport"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ImportType ImportType { get; set; } = ImportType.FunctionTable;

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

        public bool OneFilePerType { get; set; } = true;

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
        /// All extension function names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        public HashSet<string> IgnoredExtensions { get; set; } = new();

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
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on extension functions. (Default: Empty)
        /// </summary>
        public HashSet<string> AllowedExtensions { get; set; } = new();

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
        /// Allows to add or manage usings. (Default: Empty)
        /// </summary>
        public List<string> Usings { get; set; } = new();

        /// <summary>
        /// The naming convention for constants, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.Unknown"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NamingConvention ConstantNamingConvention { get; set; } = NamingConvention.Unknown;

        /// <summary>
        /// The naming convention for enums, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NamingConvention EnumNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for enum items, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NamingConvention EnumItemNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for extension functions, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NamingConvention ExtensionNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for functions, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NamingConvention FunctionNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for handles, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NamingConvention HandleNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for classes and structs, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NamingConvention TypeNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for delegates, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NamingConvention DelegateNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for parameters, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.CamelCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NamingConvention ParameterNamingConvention { get; set; } = NamingConvention.CamelCase;

        /// <summary>
        /// The naming convention for members, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public NamingConvention MemberNamingConvention { get; set; } = NamingConvention.PascalCase;

        public bool AutoSquashTypedef { get; set; } = true;

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

        public readonly List<CsEnumMetadata> CustomEnums = [];

        /// <summary>
        /// A list of allowed types for generating additional overloads.
        /// </summary>
        public HashSet<string> VaryingTypes { get; set; } = new();

        /// <summary>
        /// Generates additional overloads, <c>WARNING</c> this option can really generate many overloads. To filter which type is allowed use <see cref="VaryingTypes"/>
        /// </summary>
        public bool GenerateAdditionalOverloads { get; set; }

        public void Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public void Merge(CsCodeGeneratorConfig baseConfig, MergeOptions mergeOptions)
        {
            if (mergeOptions.HasFlag(MergeOptions.EnableExperimentalOptions))
            {
                EnableExperimentalOptions = baseConfig.EnableExperimentalOptions;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateSizeOfStructs))
            {
                GenerateSizeOfStructs = baseConfig.GenerateSizeOfStructs;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateConstructorsForStructs))
            {
                GenerateConstructorsForStructs = baseConfig.GenerateConstructorsForStructs;
            }

            if (mergeOptions.HasFlag(MergeOptions.DelegatesAsVoidPointer))
            {
                DelegatesAsVoidPointer = baseConfig.DelegatesAsVoidPointer;
            }

            if (mergeOptions.HasFlag(MergeOptions.WrapPointersAsHandle))
            {
                WrapPointersAsHandle = baseConfig.WrapPointersAsHandle;
            }

            if (mergeOptions.HasFlag(MergeOptions.GeneratePlaceholderComments))
            {
                GeneratePlaceholderComments = baseConfig.GeneratePlaceholderComments;
            }

            if (mergeOptions.HasFlag(MergeOptions.ImportType))
            {
                ImportType = baseConfig.ImportType;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateMetadata))
            {
                GenerateMetadata = baseConfig.GenerateMetadata;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateConstants))
            {
                GenerateConstants = baseConfig.GenerateConstants;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateEnums))
            {
                GenerateEnums = baseConfig.GenerateEnums;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateExtensions))
            {
                GenerateExtensions = baseConfig.GenerateExtensions;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateFunctions))
            {
                GenerateFunctions = baseConfig.GenerateFunctions;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateHandles))
            {
                GenerateHandles = baseConfig.GenerateHandles;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateTypes))
            {
                GenerateTypes = baseConfig.GenerateTypes;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateDelegates))
            {
                GenerateDelegates = baseConfig.GenerateDelegates;
            }

            if (mergeOptions.HasFlag(MergeOptions.OneFilePerType))
            {
                OneFilePerType = baseConfig.OneFilePerType;
            }

            if (mergeOptions.HasFlag(MergeOptions.BoolType))
            {
                BoolType = baseConfig.BoolType;
            }

            MergeDictionaries(KnownConstantNames, baseConfig.KnownConstantNames, mergeOptions, MergeOptions.KnownConstantNames);
            MergeDictionaries(KnownEnumValueNames, baseConfig.KnownEnumValueNames, mergeOptions, MergeOptions.KnownEnumValueNames);
            MergeDictionaries(KnownEnumPrefixes, baseConfig.KnownEnumPrefixes, mergeOptions, MergeOptions.KnownEnumPrefixes);
            MergeDictionaries(KnownExtensionPrefixes, baseConfig.KnownExtensionPrefixes, mergeOptions, MergeOptions.KnownExtensionPrefixes);
            MergeDictionaries(KnownExtensionNames, baseConfig.KnownExtensionNames, mergeOptions, MergeOptions.KnownExtensionNames);
            MergeDictionaries(KnownDefaultValueNames, baseConfig.KnownDefaultValueNames, mergeOptions, MergeOptions.KnownDefaultValueNames);
            MergeDictionaries(KnownConstructors, baseConfig.KnownConstructors, mergeOptions, MergeOptions.KnownConstructors);
            MergeDictionaries(KnownMemberFunctions, baseConfig.KnownMemberFunctions, mergeOptions, MergeOptions.KnownMemberFunctions);
            MergeDictionaries(IIDMappings, baseConfig.IIDMappings, mergeOptions, MergeOptions.IIDMappings);
            MergeDictionaries(NameMappings, baseConfig.NameMappings, mergeOptions, MergeOptions.NameMappings);
            MergeDictionaries(TypeMappings, baseConfig.TypeMappings, mergeOptions, MergeOptions.TypeMappings);

            MergeHashSets(IgnoredParts, baseConfig.IgnoredParts, mergeOptions, MergeOptions.IgnoredParts);
            MergeHashSets(Keywords, baseConfig.Keywords, mergeOptions, MergeOptions.Keywords);
            MergeHashSets(IgnoredFunctions, baseConfig.IgnoredFunctions, mergeOptions, MergeOptions.IgnoredFunctions);
            MergeHashSets(IgnoredTypes, baseConfig.IgnoredTypes, mergeOptions, MergeOptions.IgnoredTypes);
            MergeHashSets(IgnoredEnums, baseConfig.IgnoredEnums, mergeOptions, MergeOptions.IgnoredEnums);
            MergeHashSets(IgnoredTypedefs, baseConfig.IgnoredTypedefs, mergeOptions, MergeOptions.IgnoredTypedefs);
            MergeHashSets(IgnoredDelegates, baseConfig.IgnoredDelegates, mergeOptions, MergeOptions.IgnoredDelegates);
            MergeHashSets(IgnoredConstants, baseConfig.IgnoredConstants, mergeOptions, MergeOptions.IgnoredConstants);
            MergeHashSets(AllowedFunctions, baseConfig.AllowedFunctions, mergeOptions, MergeOptions.AllowedFunctions);
            MergeHashSets(AllowedTypes, baseConfig.AllowedTypes, mergeOptions, MergeOptions.AllowedTypes);
            MergeHashSets(AllowedEnums, baseConfig.AllowedEnums, mergeOptions, MergeOptions.AllowedEnums);
            MergeHashSets(AllowedTypedefs, baseConfig.AllowedTypedefs, mergeOptions, MergeOptions.AllowedTypedefs);
            MergeHashSets(AllowedDelegates, baseConfig.AllowedDelegates, mergeOptions, MergeOptions.AllowedDelegates);
            MergeHashSets(AllowedConstants, baseConfig.AllowedConstants, mergeOptions, MergeOptions.AllowedConstants);

            MergeLists(ConstantMappings, baseConfig.ConstantMappings, mergeOptions, MergeOptions.ConstantMappings);
            MergeLists(EnumMappings, baseConfig.EnumMappings, mergeOptions, MergeOptions.EnumMappings);
            MergeLists(FunctionMappings, baseConfig.FunctionMappings, mergeOptions, MergeOptions.FunctionMappings);
            MergeLists(HandleMappings, baseConfig.HandleMappings, mergeOptions, MergeOptions.HandleMappings);
            MergeLists(ClassMappings, baseConfig.ClassMappings, mergeOptions, MergeOptions.ClassMappings);
            MergeLists(DelegateMappings, baseConfig.DelegateMappings, mergeOptions, MergeOptions.DelegateMappings);
            MergeLists(ArrayMappings, baseConfig.ArrayMappings, mergeOptions, MergeOptions.ArrayMappings);
            MergeLists(Usings, baseConfig.Usings, mergeOptions, MergeOptions.Usings);
            MergeLists(IncludeFolders, baseConfig.IncludeFolders, mergeOptions, MergeOptions.IncludeFolders);
            MergeLists(SystemIncludeFolders, baseConfig.SystemIncludeFolders, mergeOptions, MergeOptions.SystemIncludeFolders);
            MergeLists(Defines, baseConfig.Defines, mergeOptions, MergeOptions.Defines);
            MergeLists(AdditionalArguments, baseConfig.AdditionalArguments, mergeOptions, MergeOptions.AdditionalArguments);

            // Naming conventions to be merged
            if (mergeOptions.HasFlag(MergeOptions.ConstantNamingConvention))
            {
                ConstantNamingConvention = baseConfig.ConstantNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.EnumNamingConvention))
            {
                EnumNamingConvention = baseConfig.EnumNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.EnumItemNamingConvention))
            {
                EnumItemNamingConvention = baseConfig.EnumItemNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.ExtensionNamingConvention))
            {
                ExtensionNamingConvention = baseConfig.ExtensionNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.FunctionNamingConvention))
            {
                FunctionNamingConvention = baseConfig.FunctionNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.HandleNamingConvention))
            {
                HandleNamingConvention = baseConfig.HandleNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.TypeNamingConvention))
            {
                TypeNamingConvention = baseConfig.TypeNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.DelegateNamingConvention))
            {
                DelegateNamingConvention = baseConfig.DelegateNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.ParameterNamingConvention))
            {
                ParameterNamingConvention = baseConfig.ParameterNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.MemberNamingConvention))
            {
                MemberNamingConvention = baseConfig.MemberNamingConvention;
            }

            AutoSquashTypedef = baseConfig.AutoSquashTypedef;
        }

        private static void MergeDictionaries<TKey, TValue>(Dictionary<TKey, TValue> target, Dictionary<TKey, TValue> source, MergeOptions mergeOptions, MergeOptions targetOption) where TKey : notnull
        {
            if (!mergeOptions.HasFlag(targetOption))
            {
                return;
            }

            foreach (var kvp in source)
            {
                target.TryAdd(kvp.Key, kvp.Value);
            }
        }

        private static void MergeHashSets<T>(HashSet<T> target, HashSet<T> source, MergeOptions mergeOptions, MergeOptions targetOption)
        {
            if (!mergeOptions.HasFlag(targetOption))
            {
                return;
            }

            foreach (var item in source)
            {
                target.Add(item);
            }
        }

        private static void MergeLists<T>(List<T> target, List<T> source, MergeOptions mergeOptions, MergeOptions targetOption)
        {
            if (!mergeOptions.HasFlag(targetOption))
            {
                return;
            }

            foreach (var item in source)
            {
                if (!target.Contains(item))
                {
                    target.Add(item);
                }
            }
        }
    }
}