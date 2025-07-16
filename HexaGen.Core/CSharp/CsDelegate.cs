namespace HexaGen.Core.CSharp
{
    using HexaGen.Core;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CsDelegate : IHasIdentifier, ICloneable<CsDelegate>
    {
        [JsonConstructor]
        public CsDelegate(string cppName, string name, CsType returnType, List<CsParameterInfo> parameters, List<string>? attributes = null, string? comment = null)
        {
            CppName = cppName;
            Name = name;
            ReturnType = returnType;
            Parameters = parameters;
            Attributes = attributes ?? [];
            Comment = comment;
        }

        public CsDelegate(string cppName, string name, CsType returnType, List<CsParameterInfo> parameters)
        {
            CppName = cppName;
            Name = name;
            ReturnType = returnType;
            Parameters = parameters;
            Attributes = [];
        }

        public string Identifier => CppName;

        public string CppName { get; set; }

        public string Name { get; set; }

        public CsType ReturnType { get; set; }

        public List<CsParameterInfo> Parameters { get; set; }

        public List<string> Attributes { get; set; }

        public string? Comment { get; set; }

        public CsDelegate Clone()
        {
            return new(CppName, Name, ReturnType.Clone(), Parameters.Select(x => x.Clone()).ToList(), [.. Attributes], Comment);
        }
    }
}