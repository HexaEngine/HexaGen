namespace HexaGen
{
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public partial class CsCodeGeneratorConfig
    {
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

        #region FunctionAlias

        public Dictionary<string, List<FunctionAliasMapping>> FunctionAliasMappings { get; set; } = [];

        public bool TryGetFunctionAliasMapping(string name, string aliasName, [NotNullWhen(true)] out FunctionAliasMapping? mapping)
        {
            if (!FunctionAliasMappings.TryGetValue(name, out var aliases))
            {
                mapping = null;
                return false;
            }

            foreach (var aliasMapping in aliases)
            {
                if (aliasMapping.ExportedAliasName == aliasName)
                {
                    mapping = aliasMapping;
                    return true;
                }
            }

            mapping = null;
            return false;
        }

        public FunctionAliasMapping? GetFunctionAliasMapping(string name, string aliasName)
        {
            if (!FunctionAliasMappings.TryGetValue(name, out var aliases))
            {
                return null;
            }

            foreach (var aliasMapping in aliases)
            {
                if (aliasMapping.ExportedAliasName == aliasName)
                {
                    return aliasMapping;
                }
            }

            return null;
        }

        public List<FunctionAliasMapping> AddFunctionAliasMapping(FunctionAliasMapping alias)
        {
            if (!FunctionAliasMappings.TryGetValue(alias.ExportedName, out var aliases))
            {
                aliases = [];
                FunctionAliasMappings.Add(alias.ExportedName, aliases);
            }

            aliases.Add(alias);
            return aliases;
        }

        #endregion FunctionAlias
    }
}