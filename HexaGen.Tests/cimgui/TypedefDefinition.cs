namespace Test
{
    internal class TypedefDefinition
    {
        public TypedefDefinition(string name, string definition)
        {
            Name = name;
            Definition = definition;
        }

        public string Name { get; set; }

        public string Definition { get; set; }

        public bool IsStruct => Definition.StartsWith("struct");
    }
}