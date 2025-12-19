namespace HexaGen.Cpp2C
{
    using HexaGen.Core.CSharp;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Types;
    using System;
    using System.Text;

    public partial class Cpp2CGeneratorConfig
    {
        public string GetCType(CppType type)
        {
            // TODO: Improve type mapping

            var currentType = type;

            List<CppType> chain = new();

            while (currentType != null)
            {
                chain.Add(currentType);
                if (currentType is CppPointerType pointer)
                {
                    currentType = pointer.ElementType;
                }
                else if (currentType is CppArrayType array)
                {
                    currentType = array.ElementType;
                }
                else if (currentType is CppReferenceType reference)
                {
                    currentType = reference.ElementType;
                }
                else if (currentType is CppPrimitiveType)
                {
                    break;
                }
                else if (currentType is CppTypedef)
                {
                    break;
                }
                else if (currentType is CppClass)
                {
                    break;
                }
                else if (currentType is CppEnum)
                {
                    break;
                }
            }

            StringBuilder sb = new();
            for (int i = chain.Count - 1; i >= 0; i--)
            {
                var t = chain[i];
                if (currentType is CppPointerType)
                {
                    sb.Append('*');
                }
                else if (currentType is CppArrayType)
                {
                    sb.Append('*');
                }
                else if (currentType is CppReferenceType)
                {
                    sb.Append('*');
                }
                else if (currentType is CppPrimitiveType primitiveType)
                {
                    sb.Append(GetPrimitiveName(primitiveType));
                }
                else if (currentType is CppTypedef typedefType)
                {
                    sb.Append(typedefType.Name);
                }
                else if (currentType is CppClass classType)
                {
                    sb.Append(GetCTypeName(classType));
                }
                else if (currentType is CppEnum enumType)
                {
                    sb.Append(enumType.Name);
                }
            }

            return sb.ToString();
        }

        private static string GetPrimitiveName(CppPrimitiveType primitiveType)
        {
            return primitiveType.Kind switch
            {
                CppPrimitiveKind.Void => "void",
                CppPrimitiveKind.Bool => "bool",
                CppPrimitiveKind.WChar => "wchar_t",
                CppPrimitiveKind.Char => "char",
                CppPrimitiveKind.Short => "short",
                CppPrimitiveKind.Int => "int",
                CppPrimitiveKind.Long => "long",
                CppPrimitiveKind.LongLong => "long long",
                CppPrimitiveKind.UnsignedChar => "unsigned char",
                CppPrimitiveKind.UnsignedShort => "unsigned short",
                CppPrimitiveKind.UnsignedInt => "unsigned int",
                CppPrimitiveKind.UnsignedLong => "unsigned long",
                CppPrimitiveKind.UnsignedLongLong => "unsigned long long",
                CppPrimitiveKind.Float => "float",
                CppPrimitiveKind.Double => "double",
                CppPrimitiveKind.LongDouble => "long double",
                CppPrimitiveKind.ObjCId => "void*",
                CppPrimitiveKind.ObjCSel => "void*",
                CppPrimitiveKind.ObjCClass => "void*",
                CppPrimitiveKind.ObjCObject => "void*",
                CppPrimitiveKind.Int128 => "__int128",
                CppPrimitiveKind.UInt128 => "unsigned __int128",
                CppPrimitiveKind.Float16 => "_Float16",
                CppPrimitiveKind.BFloat16 => "unsigned short",
                _ => throw new NotSupportedException(),
            };
        }

        public string GetCTypeName(CppClass c)
        {
            return $"{NamePrefix}{c.Name}";
        }
    }
}