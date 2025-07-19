namespace HexaGen.Metadata
{
    using CppAst;
    using HexaGen.Core.CSharp;

    public class CsPropertyMetadata
    {
        public CsPropertyMetadata(CppType cppType, CsType type, string name, string getter, string setter, string? comment = null, List<string>? attributes = null)
        {
            CppType = cppType;
            Type = type;
            Name = name;
            Comment = comment;
            Getter = getter;
            Setter = setter;
            Attributes = attributes;
        }

        public CppType CppType { get; set; }

        public CsType Type { get; set; }

        public string Name { get; set; }

        public string? Comment { get; set; }

        public string Getter { get; set; }

        public string Setter { get; set; }

        public List<string>? Attributes { get; set; }
    }
}