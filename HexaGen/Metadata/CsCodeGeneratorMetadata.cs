namespace HexaGen
{
    using Microsoft.CodeAnalysis.CSharp;
    using System.Diagnostics.CodeAnalysis;

    public interface IHasIdentifier
    {
        public string Identifier { get; }
    }

    public class CsConstantMetadataIdComparer : IEqualityComparer<CsConstantMetadata>
    {
        public static readonly CsConstantMetadataIdComparer Default = new();

        public bool Equals(CsConstantMetadata? x, CsConstantMetadata? y)
        {
            return x?.Identifier == y?.Identifier;
        }

        public int GetHashCode([DisallowNull] CsConstantMetadata obj)
        {
            return obj.Identifier.GetHashCode();
        }
    }

    public class IdentifierComparer<T> : IEqualityComparer<T> where T : class, IHasIdentifier
    {
        public static readonly IdentifierComparer<T> Default = new();

        public bool Equals(T? x, T? y)
        {
            return x?.Identifier == y?.Identifier;
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return obj.Identifier.GetHashCode();
        }
    }

    public class CsConstantMetadata : IHasIdentifier
    {
        public CsConstantMetadata(string cppName, string cppValue)
        {
            Identifier = cppName;
            CppName = cppName;
            CppValue = cppValue;
        }

        public CsConstantMetadata(string cppName, string cppValue, string? name, string? value, string? comment) : this(cppName, cppValue)
        {
            CppValue = cppValue;
            Name = name;
            Value = value;
            Comment = comment;
        }

        public string Identifier { get; set; }

        public string CppName { get; set; }

        public string CppValue { get; set; }

        public string EscapedCppValue => CppValue.ToLiteral();

        public string? Name { get; set; }

        public string? Value { get; set; }

        public string? Comment { get; set; }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public override string ToString()
        {
            return $"Constant: {CppName} = {CppValue}";
        }
    }

    public class CsEnumMetadata : IHasIdentifier
    {
        public CsEnumMetadata(string cppName)
        {
            Identifier = cppName;
            CppName = cppName;
            BaseType = "int";
        }

        public CsEnumMetadata(string identifier, string cppName, string? name, string? comment, List<CsEnumItemMetadata> items) : this(identifier)
        {
            CppName = cppName;
            Name = name;
            Comment = comment;
            Items = items;
            BaseType = "int";
        }

        public CsEnumMetadata(string identifier, string cppName, string? name, string? comment) : this(identifier)
        {
            CppName = cppName;
            Name = name;
            Comment = comment;
            Items = new();
            BaseType = "int";
        }

        public string Identifier { get; set; }

        public string CppName { get; set; }

        public string? Name { get; set; }

        public string? Comment { get; set; }

        public string BaseType { get; set; }

        public List<CsEnumItemMetadata> Items { get; set; } = new();

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }
    }

    public class CsEnumItemMetadata : IHasIdentifier
    {
        public CsEnumItemMetadata(string cppName, string cppValue)
        {
            Identifier = cppName;
            CppName = cppName;
            CppValue = cppValue;
        }

        public CsEnumItemMetadata(string cppName, string cppValue, string? name, string? value, string? comment) : this(cppName, cppValue)
        {
            CppValue = cppValue;
            Name = name;
            Value = value;
            Comment = comment;
        }

        public string Identifier { get; set; }

        public string CppName { get; set; }

        public string CppValue { get; set; }

        public string EscapedCppValue => CppValue.ToLiteral();

        public string? Name { get; set; }

        public string? Value { get; set; }

        public string? Comment { get; set; }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }
    }

    public class CsCodeGeneratorMetadata
    {
        public List<CsConstantMetadata> DefinedConstants { get; set; } = new();

        public List<CsEnumMetadata> DefinedEnums { get; set; } = new();

        public List<string> DefinedExtensions { get; set; } = new();

        public List<string> DefinedFunctions { get; set; } = new();

        public List<string> DefinedTypedefs { get; set; } = new();

        public List<string> DefinedTypes { get; set; } = new();

        public List<string> DefinedDelegates { get; set; } = new();

        public void CopyFrom(CsCodeGenerator generator)
        {
            var constants = generator.DefinedConstants.ToArray();
            var enums = generator.DefinedEnums.ToArray();
            var extensions = generator.DefinedExtensions.ToArray();
            var functions = generator.DefinedFunctions.ToArray();
            var typedefs = generator.DefinedTypedefs.ToArray();
            var types = generator.DefinedTypes.ToArray();
            var delegates = generator.DefinedDelegates.ToArray();
            for (int i = 0; i < constants.Length; i++)
            {
                DefinedConstants.Add(constants[i]);
            }
            for (int i = 0; i < enums.Length; i++)
            {
                DefinedEnums.Add(enums[i]);
            }
            for (int i = 0; i < extensions.Length; i++)
            {
                DefinedExtensions.Add(extensions[i]);
            }
            for (int i = 0; i < functions.Length; i++)
            {
                DefinedFunctions.Add(functions[i]);
            }
            for (int i = 0; i < typedefs.Length; i++)
            {
                DefinedTypedefs.Add(typedefs[i]);
            }
            for (int i = 0; i < types.Length; i++)
            {
                DefinedTypes.Add(types[i]);
            }
            for (int i = 0; i < delegates.Length; i++)
            {
                DefinedDelegates.Add(delegates[i]);
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