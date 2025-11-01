namespace HexaGen
{
    using HexaGen.Conversion;
    using HexaGen.Core.Logging;
    using HexaGen.Metadata;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Collections.Generic;
    using System.ComponentModel;

    public partial class CsCodeGeneratorConfig : IGeneratorConfig
    {
        private readonly CppTypeConverter converter;

        public CsCodeGeneratorConfig()
        {
            converter = new(this);
        }

        [DefaultValue(null)]
        public HeaderInjectionDelegate? HeaderInjector { get; set; }

        [DefaultValue(null)]
        public BaseConfig? BaseConfig { get; set; }

        /// <summary>
        /// The namespace of the generated wrapper. (Default <see cref="string.Empty"/>)
        /// </summary>
        [DefaultValue("")]
        public string Namespace { get; set; } = string.Empty;

        /// <summary>
        /// The api name of the wrapper. (Used for exported functions and macros) (Default <see cref="string.Empty"/>)
        /// </summary>
        [DefaultValue("")]
        public string ApiName { get; set; } = string.Empty;

        /// <summary>
        /// The name of the .dll or .so or .dylib. (Default <see cref="string.Empty"/>)
        /// </summary>
        [DefaultValue("")]
        public string LibName { get; set; } = string.Empty;

        /// <summary>
        /// The log level of the generator. (Default <see cref="LogSeverity.Warning"/>)
        /// </summary>
        [DefaultValue(LogSeverity.Warning)]
        public LogSeverity LogLevel { get; set; } = LogSeverity.Warning;

        /// <summary>
        /// The log level of the Clang Compiler. (Default <see cref="LogSeverity.Error"/>)
        /// </summary>
        [DefaultValue(LogSeverity.Error)]
        public LogSeverity CppLogLevel { get; set; } = LogSeverity.Error;

        /// <summary>
        /// This allows to use the (EXPERIMENTAL) options, otherwise they will be set back to false. (Default: <see langword="false"/>)
        /// </summary>
        [DefaultValue(false)]
        public bool EnableExperimentalOptions { get; set; } = false;

        /// <summary>
        /// This option generates the sizes of the structs. (Default: <see langword="false"/>)
        /// </summary>
        [DefaultValue(false)]
        public bool GenerateSizeOfStructs { get; set; } = false;

        /// <summary>
        /// The generator will generate default constructors for all structs. (Default: <see langword="true"/>)
        /// </summary>
        [DefaultValue(true)]
        public bool GenerateConstructorsForStructs { get; set; } = true;

        /// <summary>
        /// This option makes that the delegates are just void* and not delegate pointer (<see cref="delegate*&lt;void&gt;"/>) (Default: <see langword="true"/>)
        /// </summary>
        [DefaultValue(true)]
        public bool DelegatesAsVoidPointer { get; set; } = true;

        /// <summary>
        /// This option makes the resulting wrapper more "safe" so you don't need unsafe blocks everywhere. (Default: <see langword="false"/>)
        /// </summary>
        [DefaultValue(false)]
        public bool WrapPointersAsHandle { get; set; } = false;

        /// <summary>
        /// This causes the code generator to generate summary xml comments if it's missing with the text "To be documented." (Default: <see langword="true"/>)
        /// </summary>
        [DefaultValue(true)]
        public bool GeneratePlaceholderComments { get; set; } = true;

        /// <summary>
        /// This causes the code generator to use <see cref="System.Runtime.InteropServices.LibraryImportAttribute"/>.
        /// </summary>
        [JsonIgnore]
        public bool UseLibraryImport => ImportType == ImportType.LibraryImport;

        /// <summary>
        /// This causes the code generator to use a FunctionTable.
        /// </summary>
        [JsonIgnore]
        public bool UseFunctionTable => ImportType == ImportType.FunctionTable;

        /// <summary>
        /// Specifies the existing entries in the function table.
        /// </summary>
        [DefaultValue(null)]
        public List<CsFunctionTableEntry> FunctionTableEntries { get; set; } = null!;

        /// <summary>
        /// Indicates whether to use a custom context. (Default: false)
        /// </summary>
        [DefaultValue(false)]
        public bool UseCustomContext { get; set; }

        /// <summary>
        /// The function name to get the library name. (Default: "GetLibraryName")
        /// </summary>
        [DefaultValue("GetLibraryName")]
        public string GetLibraryNameFunctionName { get; set; } = "GetLibraryName";

        /// <summary>
        /// The function name to get the library extension. (Default: null)
        /// </summary>
        [DefaultValue(null)]
        public string? GetLibraryExtensionFunctionName { get; set; } = null;

        /// <summary>
        /// Determines the import type. (Default: <see cref="ImportType.LibraryImport"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(ImportType.FunctionTable)]
        public ImportType ImportType { get; set; } = ImportType.FunctionTable;

        /// <summary>
        /// The generator will generate [NativeName] attributes.
        /// </summary>
        [DefaultValue(false)]
        public bool GenerateMetadata { get; set; } = false;

        /// <summary>
        /// Enables generation for constants (CPP: Macros) (Default: <see langword="true"/>)
        /// </summary>
        [DefaultValue(true)]
        public bool GenerateConstants { get; set; } = true;

        /// <summary>
        /// Enables generation for enums. (Default: <see langword="true"/>)
        /// </summary>
        [DefaultValue(true)]
        public bool GenerateEnums { get; set; } = true;

        /// <summary>
        /// Enables generation for extensions, this option is very useful if you have an handle type or COM objects. (Default: <see langword="true"/>)
        /// </summary>
        [DefaultValue(true)]
        public bool GenerateExtensions { get; set; } = true;

        /// <summary>
        /// Enables generation for functions. This option generates the public API dllexport functions. (Default: <see langword="true"/>)
        /// </summary>
        [DefaultValue(true)]
        public bool GenerateFunctions { get; set; } = true;

        /// <summary>
        /// Enables generation for handles. (CPP: Typedefs) (Default: <see langword="true"/>)
        /// </summary>
        [DefaultValue(true)]
        public bool GenerateHandles { get; set; } = true;

        /// <summary>
        /// Enables generation for types. This includes COM objects and normal C-Structs. (Default: <see langword="true"/>)
        /// </summary>
        [DefaultValue(true)]
        public bool GenerateTypes { get; set; } = true;

        /// <summary>
        /// Enables generation for delegates. (Default: <see langword="true"/>)
        /// </summary>
        [DefaultValue(true)]
        public bool GenerateDelegates { get; set; } = true;

        [DefaultValue(true)]
        public bool OneFilePerType { get; set; } = true;

        /// <summary>
        /// This option controls the bool type eg. 8Bit Bool and 32Bit Bool. (Default: <see cref="BoolType.Bool8"/>)
        /// </summary>
        [DefaultValue(BoolType.Bool8)]
        public BoolType BoolType { get; set; } = BoolType.Bool8;

        /// <summary>
        /// Allows to map names for constants. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public Dictionary<string, string> KnownConstantNames { get; set; } = null!;

        /// <summary>
        /// Allows to map names for enums. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public Dictionary<string, string> KnownEnumValueNames { get; set; } = null!;

        /// <summary>
        /// Allows to map names for enum prefixes. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public Dictionary<string, string> KnownEnumPrefixes { get; set; } = null!;

        /// <summary>
        /// Allows to map names for extension prefixes. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public Dictionary<string, string> KnownExtensionPrefixes { get; set; } = null!;

        /// <summary>
        /// Allows to map names for extension. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public Dictionary<string, string> KnownExtensionNames { get; set; } = null!;

        /// <summary>
        /// Allows to map names for default values. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public Dictionary<string, string> KnownDefaultValueNames { get; set; } = null!;

        /// <summary>
        /// Allows to define constructors functions for types. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public Dictionary<string, List<string>> KnownConstructors { get; set; } = null!;

        /// <summary>
        /// Allows to define member functions for types. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public Dictionary<string, List<string>> KnownMemberFunctions { get; set; } = null!;

        /// <summary>
        /// Ignores parts like OpenAl in OpenALFunction -> Function. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> IgnoredParts { get; set; } = null!;

        /// <summary>
        /// C# keywords that would cause issues with naming. (Default: all common C# keywords)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> Keywords { get; set; } = null!;

        /// <summary>
        /// All function names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> IgnoredFunctions { get; set; } = null!;

        /// <summary>
        /// All extension function names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        public HashSet<string> IgnoredExtensions { get; set; } = new();

        /// <summary>
        /// All types names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> IgnoredTypes { get; set; } = null!;

        /// <summary>
        /// All enums names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> IgnoredEnums { get; set; } = null!;

        /// <summary>
        /// All typedefs names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> IgnoredTypedefs { get; set; } = null!;

        /// <summary>
        /// All delegates names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> IgnoredDelegates { get; set; } = null!;

        /// <summary>
        /// All constants names in this HashSet will be ignored in the generation process. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> IgnoredConstants { get; set; } = null!;

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on functions. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> AllowedFunctions { get; set; } = null!;

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on extension functions. (Default: Empty)
        /// </summary>
        public HashSet<string> AllowedExtensions { get; set; } = new();

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on types. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> AllowedTypes { get; set; } = null!;

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on enums. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> AllowedEnums { get; set; } = null!;

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on typedefs. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> AllowedTypedefs { get; set; } = null!;

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on delegates. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> AllowedDelegates { get; set; } = null!;

        /// <summary>
        /// Acts as a whitelist, if the list is empty no whitelisting is applied on constants. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> AllowedConstants { get; set; } = null!;

        /// <summary>
        /// Allows to add or manage usings. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public List<string> Usings { get; set; } = null!;

        /// <summary>
        /// The naming convention for constants, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.Unknown"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(NamingConvention.Unknown)]
        public NamingConvention ConstantNamingConvention { get; set; } = NamingConvention.Unknown;

        /// <summary>
        /// The naming convention for enums, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(NamingConvention.PascalCase)]
        public NamingConvention EnumNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for enum items, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(NamingConvention.PascalCase)]
        public NamingConvention EnumItemNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for extension functions, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(NamingConvention.PascalCase)]
        public NamingConvention ExtensionNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for functions, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(NamingConvention.PascalCase)]
        public NamingConvention FunctionNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for handles, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(NamingConvention.PascalCase)]
        public NamingConvention HandleNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for classes and structs, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(NamingConvention.PascalCase)]
        public NamingConvention TypeNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for delegates, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(NamingConvention.PascalCase)]
        public NamingConvention DelegateNamingConvention { get; set; } = NamingConvention.PascalCase;

        /// <summary>
        /// The naming convention for parameters, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.CamelCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(NamingConvention.CamelCase)]
        public NamingConvention ParameterNamingConvention { get; set; } = NamingConvention.CamelCase;

        /// <summary>
        /// The naming convention for members, set it to <see cref="NamingConvention.Unknown"/> to keep the original name. (Default: <see cref="NamingConvention.PascalCase"/>)
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [DefaultValue(NamingConvention.PascalCase)]
        public NamingConvention MemberNamingConvention { get; set; } = NamingConvention.PascalCase;

        [DefaultValue(true)]
        public bool AutoSquashTypedef { get; set; } = true;

        /// <summary>
        /// List of the include folders. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public List<string> IncludeFolders { get; set; } = null!;

        /// <summary>
        /// List of the system include folders. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public List<string> SystemIncludeFolders { get; set; } = null!;

        /// <summary>
        /// List of macros passed to CppAst. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public List<string> Defines { get; set; } = null!;

        /// <summary>
        /// List of the additional arguments passed directly to the C++ Clang compiler. (Default: Empty)
        /// </summary>
        [DefaultValue(null)]
        public List<string> AdditionalArguments { get; set; } = null!;

        [DefaultValue(null)]
        public List<CsEnumMetadata> CustomEnums { get; set; } = null!;

        /// <summary>
        /// A list of allowed types for generating additional overloads.
        /// </summary>
        [DefaultValue(null)]
        public HashSet<string> VaryingTypes { get; set; } = null!;

        /// <summary>
        /// Generates additional overloads, <c>WARNING</c> this option can really generate many overloads. To filter which type is allowed use <see cref="VaryingTypes"/>
        /// </summary>
        [DefaultValue(false)]
        public bool GenerateAdditionalOverloads { get; set; }
    }
}