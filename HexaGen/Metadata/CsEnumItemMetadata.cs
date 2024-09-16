namespace HexaGen.Metadata
{
    using System.Text.Json.Serialization;

    public class CsEnumItemMetadata : IHasIdentifier
    {
        public CsEnumItemMetadata(string cppName, string cppValue)
        {
            CppName = cppName;
            CppValue = cppValue;
            Attributes = new();
        }

        [JsonConstructor]
        public CsEnumItemMetadata(string cppName, string cppValue, string? name, string? value, List<string> attributes, string? comment)
        {
            CppName = cppName;
            CppValue = cppValue;
            Name = name;
            Value = value;
            Attributes = attributes;
            Comment = comment;
        }

        public string Identifier => CppName;

        public string CppName { get; set; }

        public string CppValue { get; set; }

        public string? Name { get; set; }

        public string? Value { get; set; }

        public List<string> Attributes { get; set; }

        public string? Comment { get; set; }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }
    }
}