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

        public string Name { get; init; }
    }
}