namespace HexaGen.Metadata
{
    using CppAst;
    using Newtonsoft.Json;
    using System.Xml.Serialization;

    public class CsHandleMetadata
    {
        public CsHandleMetadata(string name, CppTypedef cppType, string? comment, bool isDispatchable)
        {
            Name = name;
            CppType = cppType;
            Comment = comment;
            IsDispatchable = isDispatchable;
        }

        public string Name { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public CppTypedef CppType { get; set; }

        public string? Comment { get; set; }

        public bool IsDispatchable { get; set; }
    }
}