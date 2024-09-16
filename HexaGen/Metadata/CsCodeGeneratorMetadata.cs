namespace HexaGen.Metadata
{
    using Microsoft.CodeAnalysis.CSharp;

    public class CsCodeGeneratorMetadata
    {
        public CsCodeGeneratorConfig Settings { get; set; }

        public List<CsConstantMetadata> DefinedConstants { get; set; } = new();

        public List<CsEnumMetadata> DefinedEnums { get; set; } = new();

        public List<string> DefinedExtensions { get; set; } = new();

        public List<string> DefinedFunctions { get; set; } = new();

        public List<string> DefinedTypedefs { get; set; } = new();

        public List<string> DefinedTypes { get; set; } = new();

        public List<string> DefinedDelegates { get; set; } = new();

        public Dictionary<string, string> WrappedPointers { get; set; } = new();

        public int VTableLength { get; set; }

        public void CopyFrom(CsCodeGenerator generator)
        {
            Settings = generator.Settings;
            DefinedConstants.AddRange(generator.DefinedConstants);
            DefinedEnums.AddRange(generator.DefinedEnums);
            DefinedExtensions.AddRange(generator.DefinedExtensions);
            DefinedFunctions.AddRange(generator.CppDefinedFunctions);
            DefinedTypedefs.AddRange(generator.DefinedTypedefs);
            DefinedTypes.AddRange(generator.DefinedTypes);
            DefinedDelegates.AddRange(generator.DefinedDelegates);
            Copy(generator.WrappedPointers, WrappedPointers);
            VTableLength = generator.VTableLength;
        }

        public static void Copy<TKey, TValue>(Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> destination) where TKey : notnull
        {
            foreach (var item in source)
            {
                destination[item.Key] = item.Value;
            }
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