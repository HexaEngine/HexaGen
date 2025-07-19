namespace HexaGen
{
    using HexaGen.Core.Logging;
    using HexaGen.Core.Mapping;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Types;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public interface IGeneratorConfig
    {
        HashSet<string> AllowedConstants { get; set; }
        HashSet<string> AllowedDelegates { get; set; }
        HashSet<string> AllowedEnums { get; set; }
        HashSet<string> AllowedFunctions { get; set; }
        HashSet<string> AllowedTypedefs { get; set; }
        HashSet<string> AllowedTypes { get; set; }
        string ApiName { get; set; }
        List<ArrayMapping> ArrayMappings { get; set; }
        BoolType BoolType { get; set; }
        List<TypeMapping> ClassMappings { get; set; }
        List<ConstantMapping> ConstantMappings { get; set; }
        LogSeverity CppLogLevel { get; set; }
        List<DelegateMapping> DelegateMappings { get; set; }
        bool DelegatesAsVoidPointer { get; set; }
        List<EnumMapping> EnumMappings { get; set; }
        List<FunctionMapping> FunctionMappings { get; set; }
        bool GenerateConstants { get; set; }
        bool GenerateDelegates { get; set; }
        bool GenerateEnums { get; set; }
        bool GenerateExtensions { get; set; }
        bool GenerateFunctions { get; set; }
        bool GenerateHandles { get; set; }
        bool GenerateSizeOfStructs { get; set; }
        bool GenerateTypes { get; set; }
        List<HandleMapping> HandleMappings { get; set; }
        HashSet<string> IgnoredConstants { get; set; }
        HashSet<string> IgnoredDelegates { get; set; }
        HashSet<string> IgnoredEnums { get; set; }
        HashSet<string> IgnoredFunctions { get; set; }
        HashSet<string> IgnoredParts { get; set; }
        HashSet<string> IgnoredTypedefs { get; set; }
        HashSet<string> IgnoredTypes { get; set; }
        Dictionary<string, string> IIDMappings { get; set; }
        HashSet<string> Keywords { get; set; }
        Dictionary<string, string> KnownConstantNames { get; set; }
        Dictionary<string, List<string>> KnownConstructors { get; set; }
        Dictionary<string, string> KnownDefaultValueNames { get; set; }
        Dictionary<string, string> KnownEnumPrefixes { get; set; }
        Dictionary<string, string> KnownEnumValueNames { get; set; }
        Dictionary<string, string> KnownExtensionNames { get; set; }
        Dictionary<string, string> KnownExtensionPrefixes { get; set; }
        Dictionary<string, List<string>> KnownMemberFunctions { get; set; }
        string LibName { get; set; }
        LogSeverity LogLevel { get; set; }
        Dictionary<string, string> NameMappings { get; set; }
        string Namespace { get; set; }

        Dictionary<string, string> TypeMappings { get; set; }
        List<string> Usings { get; set; }

        string GetBoolType();

        string GetCsCleanName(string name);

        string GetCsSubTypeName(CppClass parentClass, string parentCsName, CppClass subClass, int idxSubClass);

        string GetCsTypeName(CppPointerType pointerType);

        string GetCsTypeName(CppPrimitiveType primitiveType, bool isPointer);

        string GetCsTypeName(CppType? type, bool isPointer = false);

        string GetCsWrapperTypeName(CppPointerType pointerType);

        string GetCsWrapperTypeName(CppPrimitiveType primitiveType, bool isPointer);

        string GetCsWrapperTypeName(CppType? type, bool isPointer = false);

        DelegateMapping? GetDelegateMapping(string delegateName);

        EnumMapping? GetEnumMapping(string enumName);

        EnumPrefix GetEnumNamePrefix(string typeName);

        string GetExtensionNamePrefix(string typeName);

        FunctionMapping? GetFunctionMapping(string functionName);

        string GetNamelessParameterSignature(IList<CppParameter> parameters, bool canUseOut, bool delegateType = false, bool compatibility = false);

        string GetParameterName(int paramIdx, string name);

        string GetParameterSignature(IList<CppParameter> parameters, bool canUseOut, bool attributes = true, bool names = true, bool delegateType = false, bool compatibility = false);

        string GetConstantName(string value);

        string GetEnumName(string value, EnumPrefix enumPrefix);

        string GetExtensionName(string value, string extensionPrefix);

        string GetCsFunctionName(string function);

        TypeMapping? GetTypeMapping(string typeName);

        string GetFieldName(string name);

        string NormalizeParameterName(string name);

        string? NormalizeValue(string value, bool sanitize);

        void Save(string path);

        bool TryGetArrayMapping(CppArrayType arrayType, [NotNullWhen(true)] out string? mapping);

        bool TryGetDefaultValue(string functionName, CppParameter parameter, bool sanitize, out string? defaultValue);

        bool TryGetDelegateMapping(string delegateName, [NotNullWhen(true)] out DelegateMapping? mapping);

        bool TryGetEnumMapping(string enumName, [NotNullWhen(true)] out EnumMapping? mapping);

        bool TryGetFunctionMapping(string functionName, [NotNullWhen(true)] out FunctionMapping? mapping);

        bool TryGetTypeMapping(string typeName, [NotNullWhen(true)] out TypeMapping? mapping);
    }
}