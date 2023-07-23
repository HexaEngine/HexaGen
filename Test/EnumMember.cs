namespace Test
{
    internal class EnumMember
    {
        public EnumMember(string name, string value, string? comment)
        {
            Name = name;
            Value = value;
            Comment = comment;
        }

        public string Name { get; }
        public string Value { get; }
        public string? Comment { get; }
    }
}