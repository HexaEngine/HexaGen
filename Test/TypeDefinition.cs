namespace Test
{
    internal class TypeDefinition
    {
        public string Name { get; }
        public TypeReference[] Fields { get; }

        public string? Comment { get; }

        public TypeDefinition(string name, TypeReference[] fields, string? comment)
        {
            Name = name;
            Fields = fields;
            Comment = comment;
        }
    }
}