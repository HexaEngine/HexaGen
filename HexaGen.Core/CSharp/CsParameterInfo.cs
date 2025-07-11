namespace HexaGen.Core.CSharp
{
    using CppAst;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [Flags]
    public enum ParameterFlags
    {
        None = 0,
        Default = 1 << 0,
        Out = 1 << 1,
        Ref = 1 << 2,
        Span = 1 << 3,
        Pointer = 1 << 4,
        String = 1 << 5,
        Array = 1 << 6,
        Bool = 1 << 7,
        IID = 1 << 8,
        COMPtr = 1 << 9,
    }

    public class CsParameterInfo : ICloneable<CsParameterInfo>
    {
        [JsonConstructor]
        public CsParameterInfo(string name, CppType cppType, CsType type, List<string> modifiers, List<string> attributes, Direction direction, string? defaultValue, string? fieldName)
        {
            Name = name;
            CppType = cppType;
            Type = type;
            Modifiers = modifiers;
            Attributes = attributes;
            Direction = direction;
            DefaultValue = defaultValue;
            FieldName = fieldName;
        }

        public CsParameterInfo(string name, CppType cppType, CsType type, List<string> modifiers, List<string> attributes, Direction direction)
        {
            Name = name;
            CppType = cppType;
            Type = type;
            Modifiers = modifiers;
            Attributes = attributes;
            Direction = direction;
        }

        public CsParameterInfo(string name, CppType cppType, CsType type, Direction direction, string? defaultValue, string? fieldName)
        {
            Name = name;
            CppType = cppType;
            Type = type;
            Modifiers = new();
            Attributes = new();
            Direction = direction;
            DefaultValue = defaultValue;
            FieldName = fieldName;
        }

        public CsParameterInfo(string name, CppType cppType, CsType type, Direction direction)
        {
            Name = name;
            CppType = cppType;
            Type = type;
            Modifiers = new();
            Attributes = new();
            Direction = direction;
        }

        public string Name { get; set; }

        public string CleanName => Name.Replace("@", string.Empty);

        [XmlIgnore]
        [JsonIgnore]
        public CppType CppType { get; set; }

        public CsType Type { get; set; }

        public List<string> Modifiers { get; set; }

        public List<string> Attributes { get; set; }

        public Direction Direction { get; set; }

        public string? DefaultValue { get; set; }

        public string? FieldName { get; set; }

        public ParameterFlags Flags
        {
            get
            {
                var result = ParameterFlags.None;
                result |= DefaultValue != null ? ParameterFlags.Default : ParameterFlags.None;
                result |= Type.IsOut ? ParameterFlags.Out : ParameterFlags.None;
                result |= Type.IsRef ? ParameterFlags.Ref : ParameterFlags.None;
                result |= Type.IsSpan ? ParameterFlags.Span : ParameterFlags.None;
                result |= Type.IsPointer ? ParameterFlags.Pointer : ParameterFlags.None;
                result |= Type.IsString ? ParameterFlags.String : ParameterFlags.None;
                result |= Type.IsArray ? ParameterFlags.Array : ParameterFlags.None;
                result |= Type.IsBool ? ParameterFlags.Bool : ParameterFlags.None;
                result |= Type.Name.Contains("Guid*") ? ParameterFlags.IID : ParameterFlags.None;
                result |= Type.Name.Contains("ComPtr<") ? ParameterFlags.COMPtr : ParameterFlags.None;
                return result;
            }
        }

        public override string ToString()
        {
            return $"{Type.Name} {Name}";
        }

        public CsParameterInfo Clone()
        {
            return new CsParameterInfo(Name, CppType, Type.Clone(), Modifiers.Clone(), Attributes.Clone(), Direction, DefaultValue, FieldName);
        }
    }
}