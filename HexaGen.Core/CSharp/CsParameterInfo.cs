namespace HexaGen.Core.CSharp
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;

    [Flags]
    public enum ParameterFlags
    {
        NoneOrConst = 0,
        Out = 1,
        Ref = 2,
        Pointer = 4,
        String = 8,
        Array = 16,
        Bool = 32,
        IID = 64,
        COMPtr = 128,
    }

    public class CsParameterInfo
    {
        public CsParameterInfo(string name, CsType type, List<string> modifiers, List<string> attributes, Direction direction)
        {
            Name = name;
            Type = type;
            Modifiers = modifiers;
            Attributes = attributes;
            Direction = direction;
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

        public ParameterFlags Flags
        {
            get
            {
                var result = ParameterFlags.NoneOrConst;
                result |= Type.IsOut ? ParameterFlags.Out : ParameterFlags.NoneOrConst;
                result |= Type.IsRef ? ParameterFlags.Ref : ParameterFlags.NoneOrConst;
                result |= Type.IsPointer ? ParameterFlags.Pointer : ParameterFlags.NoneOrConst;
                result |= Type.IsString ? ParameterFlags.String : ParameterFlags.NoneOrConst;
                result |= Type.IsArray ? ParameterFlags.Array : ParameterFlags.NoneOrConst;
                result |= Type.IsBool ? ParameterFlags.Bool : ParameterFlags.NoneOrConst;
                result |= Type.Name.Contains("Guid*") ? ParameterFlags.IID : ParameterFlags.NoneOrConst;
                result |= Type.Name.Contains("ComPtr<") ? ParameterFlags.COMPtr : ParameterFlags.NoneOrConst;
                return result;
            }
        }

        public override string ToString()
        {
            return $"{Type.Name} {Name}";
        }
    }
}