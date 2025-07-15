namespace HexaGen.Core.CSharp
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CsFunction
    {
        [JsonConstructor]
        public CsFunction(string name, string? comment, List<CsFunctionOverload> overloads)
        {
            Name = name;
            Comment = comment;
            Overloads = overloads;
        }

        public CsFunction(string name, string? comment)
        {
            Name = name;
            Comment = comment;
            Overloads = new();
        }

        public string Name { get; set; }

        public string? Comment { get; set; }

        public List<CsFunctionOverload> Overloads { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public CsFunction Clone()
        {
            return new(Name, Comment, Overloads.Select(overload => overload.Clone()).ToList());
        }
    }
}