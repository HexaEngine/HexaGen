﻿namespace HexaGen.Metadata
{
    using HexaGen;
    using HexaGen.Core;
    using Newtonsoft.Json;

    public class CsConstantMetadata : IHasIdentifier, ICloneable<CsConstantMetadata>
    {
        public CsConstantMetadata(string cppName, string cppValue, CsConstantType type)
        {
            CppName = cppName;
            CppValue = cppValue;
            Type = type;
        }

        [JsonConstructor]
        public CsConstantMetadata(string cppName, string cppValue, string? name, string? value, CsConstantType type, string? comment)
        {
            CppName = cppName;
            CppValue = cppValue;
            Name = name;
            Value = value;
            Type = type;
            Comment = comment;
        }

        public string Identifier => CppName;

        public string CppName { get; set; }

        public string CppValue { get; set; }

        public string EscapedCppValue => CppValue.ToLiteral();

        public string? Name { get; set; }

        public string? Value { get; set; }

        public CsConstantType Type { get; set; }

        public string? CustomType { get; set; }

        public string? Comment { get; set; }

        public override int GetHashCode()
        {
            return CppName.GetHashCode();
        }

        public override string ToString()
        {
            return $"Constant: {CppName} = {CppValue}";
        }

        public CsConstantMetadata Clone()
        {
            return new CsConstantMetadata(CppName, CppValue, Name, Value, Type, Comment);
        }
    }
}