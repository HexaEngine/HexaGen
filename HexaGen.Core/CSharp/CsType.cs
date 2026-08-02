namespace HexaGen.Core.CSharp
{
    using CppAst;
    using HexaGen.Core;
    using HexaGen.CppAst.Model.Types;
    using Newtonsoft.Json;
    using System.Globalization;

    public class CsType : ICloneable<CsType>
    {
        private TypeFlags flags;

        [JsonConstructor]
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

        public enum TypeFlags
        {
            None = 0,
            IsPointer = 1 << 0,
            IsOut = 1 << 1,
            IsRef = 1 << 2,
            IsIn = 1 << 3,
            IsSpan = 1 << 4,
            IsString = 1 << 5,
            IsPrimitive = 1 << 6,
            IsVoid = 1 << 7,
            IsBool = 1 << 8,
            IsArray = 1 << 9,
            IsEnum = 1 << 10,
        }

        public bool HasFlag(TypeFlags flag)
        {
            return (flags & flag) == flag;
        }

        public void SetFlag(TypeFlags flag, bool value)
        {
            if (value)
            {
                flags |= flag;
            }
            else
            {
                flags &= ~flag;
            }
        }

        public string Name { get; set; }

        public string CleanName { get; set; }

        public bool IsPointer { get => HasFlag(TypeFlags.IsPointer); set => SetFlag(TypeFlags.IsPointer, value); }

        public bool IsOut { get => HasFlag(TypeFlags.IsOut); set => SetFlag(TypeFlags.IsOut, value); }

        public bool IsRef { get => HasFlag(TypeFlags.IsRef); set => SetFlag(TypeFlags.IsRef, value); }

        public bool IsIn { get => HasFlag(TypeFlags.IsIn); set => SetFlag(TypeFlags.IsIn, value); }

        public bool IsSpan { get => HasFlag(TypeFlags.IsSpan); set => SetFlag(TypeFlags.IsSpan, value); }

        public bool IsString { get => HasFlag(TypeFlags.IsString); set => SetFlag(TypeFlags.IsString, value); }

        public bool IsPrimitive { get => HasFlag(TypeFlags.IsPrimitive); set => SetFlag(TypeFlags.IsPrimitive, value); }

        public bool IsVoid { get => HasFlag(TypeFlags.IsVoid); set => SetFlag(TypeFlags.IsVoid, value); }

        public bool IsBool { get => HasFlag(TypeFlags.IsBool); set => SetFlag(TypeFlags.IsBool, value); }

        public bool IsArray { get => HasFlag(TypeFlags.IsArray); set => SetFlag(TypeFlags.IsArray, value); }

        public bool IsEnum { get => HasFlag(TypeFlags.IsEnum); set => SetFlag(TypeFlags.IsEnum, value); }

        public CsStringType StringType { get; set; }

        public CsPrimitiveType PrimitiveType { get; set; }

        public bool IsRefOrIn => IsRef || IsIn;

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

        public void FlagAsString()
        {
            IsString = true;
            if (PrimitiveType == CsPrimitiveType.Byte)
            {
                StringType = CsStringType.StringUTF8;
            }
            else if (PrimitiveType == CsPrimitiveType.Char)
            {
                StringType = CsStringType.StringUTF16;
            }
        }

        public string Classify()
        {
            IsRef = Name.StartsWith("ref ");
            IsIn = Name.StartsWith("in ");
            IsSpan = Name.StartsWith("ReadOnlySpan<") || Name.StartsWith("Span<");
            IsOut = Name.StartsWith("out ");
            IsArray = Name.Contains("[]");
            IsPointer = Name.Contains('*');
            IsBool = Name.Contains("bool");
            IsString = Name.Contains("string");
            IsVoid = Name.StartsWith("void");

            IsPrimitive = !IsOut && !IsRef && !IsIn && !IsArray && !IsPointer && !IsArray && !IsString;

            if (IsString)
            {
                FlagAsString();
            }

            if (IsRef)
            {
                return Name.Replace("ref ", string.Empty);
            }
            if (IsOut)
            {
                return Name.Replace("out ", string.Empty);
            }
            if (IsIn)
            {
                return Name.Replace("in ", string.Empty);
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
                CppPrimitiveKind.UnsignedLong => CsPrimitiveType.UInt,
                CppPrimitiveKind.Long => CsPrimitiveType.Int,

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

        public ReadOnlySpan<char> GetNormalizedName()
        {
            var nameNormalized = Name.AsSpan();
            if (IsRef)
                nameNormalized = nameNormalized["ref ".Length..];
            if (IsIn)
                nameNormalized = nameNormalized["in ".Length..];
            return nameNormalized.Trim();
        }

        public bool Conflicts(CsType other)
        {
            if (IsRefOrIn && other.IsRefOrIn)
            {
                return GetNormalizedName().SequenceEqual(other.GetNormalizedName());
            }
            return Name == other.Name;
        }

        private static readonly CompareInfo CompareInfo = CultureInfo.InvariantCulture.CompareInfo;

        public int GetConflictHashCode()
        {
            HashCode code = new();
            if (IsRefOrIn)
            {
                code.Add("ref");
                code.Add(CompareInfo.GetHashCode(GetNormalizedName(), CompareOptions.None));
            }
            else
            {
                code.Add(CompareInfo.GetHashCode(Name, CompareOptions.None));
            }
            return code.ToHashCode();
        }
    }
}