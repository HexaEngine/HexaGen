namespace HexaGen.Metadata
{
    using HexaGen.Core.CSharp;
    using Microsoft.CodeAnalysis.CSharp;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class CsCodeGeneratorMetadata
    {
        public CsCodeGeneratorConfig Settings { get; set; } = null!;

        public List<CsConstantMetadata> DefinedConstants { get; set; } = new();

        public List<CsEnumMetadata> DefinedEnums { get; set; } = new();

        public List<string> DefinedTypeExtensions { get; set; } = new();

        public List<CsFunction> DefinedExtensions { get; set; } = new();

        public List<string> CppDefinedFunctions { get; set; } = new();

        public List<CsFunction> DefinedFunctions { get; set; } = new();

        public List<string> DefinedTypedefs { get; set; } = new();

        public List<string> DefinedTypes { get; set; } = new();

        public List<CsDelegate> DefinedDelegates { get; set; } = new();

        public Dictionary<string, string> WrappedPointers { get; set; } = new();

        public CsFunctionTableMetadata FunctionTable { get; set; } = null!;

        public void Merge(CsCodeGeneratorMetadata from, bool mergeFunctionTable)
        {
            DefinedConstants.AddRange(from.DefinedConstants);
            DefinedEnums.AddRange(from.DefinedEnums);
            DefinedTypeExtensions.AddRange(from.DefinedTypeExtensions);
            DefinedExtensions.AddRange(from.DefinedExtensions);
            CppDefinedFunctions.AddRange(from.CppDefinedFunctions);
            DefinedFunctions.AddRange(from.DefinedFunctions);
            DefinedTypedefs.AddRange(from.DefinedTypedefs);
            DefinedTypes.AddRange(from.DefinedTypes);
            DefinedDelegates.AddRange(from.DefinedDelegates);
            Copy(from.WrappedPointers, WrappedPointers);
            if (mergeFunctionTable)
            {
                FunctionTable.Merge(from.FunctionTable);
            }
        }

        public CsCodeGeneratorMetadata Clone()
        {
            var clonedMetadata = new CsCodeGeneratorMetadata
            {
                Settings = Settings,
                DefinedConstants = DefinedConstants.Select(constant => constant.Clone()).ToList(),
                DefinedEnums = DefinedEnums.Select(enumItem => enumItem.Clone()).ToList(),
                DefinedTypeExtensions = new List<string>(DefinedTypeExtensions),
                DefinedExtensions = DefinedExtensions.Select(x => x.Clone()).ToList(),
                CppDefinedFunctions = new List<string>(CppDefinedFunctions),
                DefinedFunctions = DefinedFunctions.Select(function => function.Clone()).ToList(),
                DefinedTypedefs = new List<string>(DefinedTypedefs),
                DefinedTypes = new List<string>(DefinedTypes),
                DefinedDelegates = DefinedDelegates.Select(del => del.Clone()).ToList(),
                WrappedPointers = new Dictionary<string, string>(WrappedPointers),
                FunctionTable = FunctionTable.Clone()
            };

            return clonedMetadata;
        }

        public static void Copy<TKey, TValue>(Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> destination) where TKey : notnull
        {
            foreach (var item in source)
            {
                destination[item.Key] = item.Value;
            }
        }

        private static readonly JsonSerializerOptions options = new(JsonSerializerDefaults.General)
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public void Save(string path)
        {
            using var fs = File.Create(path);
            JsonSerializer.Serialize(fs, this, options);
        }

        public static CsCodeGeneratorMetadata Load(string path)
        {
            using var fs = File.OpenRead(path);
            return (CsCodeGeneratorMetadata?)JsonSerializer.Deserialize(fs, typeof(CsCodeGeneratorMetadata), options) ?? new();
        }
    }

    public static class TextHelper
    {
        /// <summary>
        /// Returns a C# string literal with the given value.
        /// </summary>
        /// <param name="value">The value that the resulting string literal should have.</param>
        /// <param name="quote">True to put (double) quotes around the string literal.</param>
        /// <returns>A string literal with the given value.</returns>
        /// <remarks>
        /// Escapes non-printable characters.
        /// </remarks>
        public static string ToLiteral(this string value, bool quote = false)
        {
            var literal = SymbolDisplay.FormatLiteral(value, quote);
            return literal.Replace("\"", "\\\"");
        }
    }
}