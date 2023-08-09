namespace HexaGen.Core.CSharp
{
    using System.Collections.Generic;

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

        public override string ToString()
        {
            return $"{Type.Name} {Name}";
        }
    }
}