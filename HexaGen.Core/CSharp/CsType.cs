namespace HexaGen.Core.CSharp
{
    using CppAst;

    public class CsType : ICloneable<CsType>
    {
        public CsType(string name, string cleanName, bool isPointer, bool isOut, bool isRef, bool isSpan, bool isString, bool isPrimitive, bool isVoid, bool isBool, bool isArray, bool isEnum, CsStringType stringType, CsPrimitiveType primitiveType)
        {
            Name = name;
            CleanName = cleanName;
            IsPointer = isPointer;
            IsOut = isOut;
            IsRef = isRef;
            IsSpan = isSpan;
            IsString = isString;
            IsPrimitive = isPrimitive;
            IsVoid = isVoid;
            IsBool = isBool;
            IsArray = isArray;
            IsEnum = isEnum;
            StringType = stringType;
            PrimitiveType = primitiveType;
        }

        public CsType(string name, bool isPointer, bool isRef, bool isString, bool isPrimitive, bool isVoid, bool isArray, CsPrimitiveType primitiveType)
        {
            Name = name;
            IsPointer = isPointer;
            IsRef = isRef;
            IsString = isString;
            IsPrimitive = isPrimitive;
            IsVoid = isVoid;
            IsArray = isArray;
            PrimitiveType = primitiveType;
            CleanName = Classify();
        }

        public CsType(string name, CsPrimitiveType primitiveType)
        {
            Name = name;
            PrimitiveType = primitiveType;
            CleanName = Classify();
        }

        public CsType(string name, CppPrimitiveKind primitiveType)
        {
            Name = name;
            PrimitiveType = Map(primitiveType);
            CleanName = Classify();
        }

        public CsType(string name, bool isEnum, CppPrimitiveKind primitiveType)
        {
            Name = name;
            PrimitiveType = Map(primitiveType);
            IsEnum = isEnum;
            CleanName = Classify();
        }

        public string Name { get; set; }

        public string CleanName { get; set; }

        public bool IsPointer { get; set; }

        public bool IsOut { get; set; }

        public bool IsRef { get; set; }

        public bool IsSpan { get; set; }

        public bool IsString { get; set; }

        public bool IsPrimitive { get; set; }

        public bool IsVoid { get; set; }

        public bool IsBool { get; set; }

        public bool IsArray { get; set; }

        public bool IsEnum { get; set; }

        public CsStringType StringType { get; set; }

        public CsPrimitiveType PrimitiveType { get; set; }

        public static bool IsKnownPrimitive(string name)
        {
            if (name.StartsWith("void"))
                return true;
            if (name.StartsWith("bool"))
                return true;
            if (name.StartsWith("byte"))
                return true;
            if (name.StartsWith("sbyte"))
                return true;
            if (name.StartsWith("char"))
                return true;
            if (name.StartsWith("short"))
                return true;
            if (name.StartsWith("ushort"))
                return true;
            if (name.StartsWith("int"))
                return true;
            if (name.StartsWith("uint"))
                return true;
            if (name.StartsWith("long"))
                return true;
            if (name.StartsWith("ulong"))
                return true;
            if (name.StartsWith("float"))
                return true;
            if (name.StartsWith("double"))
                return true;
            if (name.StartsWith("Vector2"))
                return true;
            if (name.StartsWith("Vector3"))
                return true;
            if (name.StartsWith("Vector4"))
                return true;
            return false;
        }

        public string Classify()
        {
            IsRef = Name.StartsWith("ref ");
            IsSpan = Name.StartsWith("ReadOnlySpan<") || Name.StartsWith("Span<");
            IsOut = Name.StartsWith("out ");
            IsArray = Name.Contains("[]");
            IsPointer = Name.Contains('*');
            IsBool = Name.Contains("bool");
            IsString = Name.Contains("string");
            IsVoid = Name.StartsWith("void");

            IsPrimitive = !IsOut && !IsRef && !IsArray && !IsPointer && !IsArray && !IsString;

            if (IsString)
            {
                if (PrimitiveType == CsPrimitiveType.Byte)
                {
                    StringType = CsStringType.StringUTF8;
                }
                if (PrimitiveType == CsPrimitiveType.Char)
                {
                    StringType = CsStringType.StringUTF16;
                }
            }

            if (IsRef)
            {
                return Name.Replace("ref ", string.Empty);
            }
            if (IsSpan)
            {
                var temp = Name.AsSpan();

                temp = temp.StartsWith("ReadOnlySpan<") ? temp["ReadOnlySpan<".Length..] : temp;
                temp = temp.StartsWith("Span<") ? temp["Span<".Length..] : temp;
                temp = temp.TrimEndFirstOccurrence('>');

                return temp.ToString();
            }
            else if (IsArray)
            {
                return Name.Replace("[]", string.Empty);
            }
            else if (IsPointer)
            {
                return Name.Replace("*", string.Empty);
            }
            else
            {
                return Name;
            }
        }

        public static CsPrimitiveType Map(CppPrimitiveKind kind)
        {
            return kind switch
            {
                CppPrimitiveKind.Void => CsPrimitiveType.Void,
                CppPrimitiveKind.Bool => CsPrimitiveType.Bool,
                CppPrimitiveKind.WChar => CsPrimitiveType.Char,
                CppPrimitiveKind.Char => CsPrimitiveType.Byte,
                CppPrimitiveKind.Short => CsPrimitiveType.Short,
                CppPrimitiveKind.Int => CsPrimitiveType.Int,
                CppPrimitiveKind.LongLong => CsPrimitiveType.Long,
                CppPrimitiveKind.UnsignedChar => CsPrimitiveType.Byte,
                CppPrimitiveKind.UnsignedShort => CsPrimitiveType.UShort,
                CppPrimitiveKind.UnsignedInt => CsPrimitiveType.UInt,
                CppPrimitiveKind.UnsignedLongLong => CsPrimitiveType.ULong,
                CppPrimitiveKind.Float => CsPrimitiveType.Float,
                CppPrimitiveKind.Double => CsPrimitiveType.Double,
                CppPrimitiveKind.LongDouble => CsPrimitiveType.Double,

                _ => throw new NotSupportedException($"The kind '{kind}' is not supported"),
            };
        }

        public override string ToString()
        {
            return Name;
        }

        public CsType Clone()
        {
            return new CsType(Name, CleanName, IsPointer, IsOut, IsRef, IsSpan, IsString, IsPrimitive, IsVoid, IsBool, IsArray, IsEnum, StringType, PrimitiveType);
        }
    }
}