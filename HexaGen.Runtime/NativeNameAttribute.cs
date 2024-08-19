namespace HexaGen.Runtime
{
    using System;

    public enum NativeNameType
    {
        Type,
        Field,
        StructOrClass,
        Typedef,
        Enum,
        EnumItem,
        Func,
        Param,
        Const,
        Delegate,
        Value
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class NativeNameAttribute : Attribute
    {
        public NativeNameAttribute(string name)
        {
            Name = name;
        }

        public NativeNameAttribute(NativeNameType type, string name)
        {
            Type = type;
            Name = name;
        }

        public NativeNameType Type { get; }

        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class SourceLocationAttribute : Attribute
    {
        public SourceLocationAttribute(string file, string start, string end)
        {
            File = file;
            Start = start;
            End = end;
        }

        public string File { get; set; }

        public string Start { get; }

        public string End { get; }
    }
}