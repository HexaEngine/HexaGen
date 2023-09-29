namespace HexaGen.Core.CSharp
{
    using System.Collections.Generic;

    [Flags]
    public enum ParameterFlags
    {
        None = 0,
        Default = 1,
        Out = 2,
        Ref = 4,
        Pointer = 8,
        String = 16,
        Array = 32,
        Bool = 64,
        IID = 128,
        COMPtr = 256,
    }

    public class CsParameterInfo : ICloneable<CsParameterInfo>
    {
        public CsParameterInfo(string name, CsType type, List<string> modifiers, List<string> attributes, Direction direction, string? defaultValue, string? fieldName)
        {
            Name = name;
            Type = type;
            Modifiers = modifiers;
            Attributes = attributes;
            Direction = direction;
            DefaultValue = defaultValue;
            FieldName = fieldName;
        }

        public CsParameterInfo(string name, CsType type, List<string> modifiers, List<string> attributes, Direction direction)
        {
            Name = name;
            Type = type;
            Modifiers = modifiers;
            Attributes = attributes;
            Direction = direction;
        }

        public CsParameterInfo(string name, CsType type, Direction direction, string? defaultValue, string? fieldName)
        {
            Name = name;
            Type = type;
            Modifiers = new();
            Attributes = new();
            Direction = direction;
            DefaultValue = defaultValue;
            FieldName = fieldName;
        }

        public CsParameterInfo(string name, CsType type, Direction direction)
        {
            Name = name;
            Type = type;
            Modifiers = new();
            Attributes = new();
            Direction = direction;
        }

        public string Name { get; set; }

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
            return new CsParameterInfo(Name, Type.Clone(), Modifiers.Clone(), Attributes.Clone(), Direction, DefaultValue, FieldName);
        }
    }
}