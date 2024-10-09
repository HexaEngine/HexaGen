namespace HexaGen.Metadata
{
    using System.Text.Json.Serialization;

    public class CsEnumMetadata : IHasIdentifier
    {
        [JsonConstructor]
        public CsEnumMetadata(string cppName, string name, List<string> attributes, string? comment, string baseType, List<CsEnumItemMetadata> items)
        {
            CppName = cppName;
            Name = name;
            Attributes = attributes;
            Comment = comment;
            Items = items;
            BaseType = baseType;
            Items = items;
        }

        public CsEnumMetadata(string cppName, string name, List<string> attributes, string? comment, List<CsEnumItemMetadata> items)
        {
            CppName = cppName;
            Name = name;
            Attributes = attributes;
            Comment = comment;
            Items = items;
            BaseType = "int";
        }

        public CsEnumMetadata(string cppName, string name, List<string> attributes, string? comment)
        {
            CppName = cppName;
            Name = name;
            Attributes = attributes;
            Comment = comment;
            Items = new();
            BaseType = "int";
        }

        public string Identifier => CppName;

        public string CppName { get; set; }

        public string Name { get; set; }

        public List<string> Attributes { get; set; }

        public string? Comment { get; set; }

        public string BaseType { get; set; }

        public List<CsEnumItemMetadata> Items { get; set; } = new();

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public CsEnumMetadata Clone()
        {
            return new CsEnumMetadata(CppName, Name, new List<string>(Attributes), Comment, BaseType, Items.Select(item => item.Clone()).ToList()
            );
        }
    }
}