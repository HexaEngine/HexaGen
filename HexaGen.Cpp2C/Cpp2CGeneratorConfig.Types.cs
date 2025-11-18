namespace HexaGen.Cpp2C
{
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Types;
    using System;
    using System.Text;

    public partial class Cpp2CGeneratorConfig
    {
        public struct TypeAnalysisResult
        {
            public int PointerDepth;
            public string TypeName;
        }

        public string GetCType(CppType type)
        {
            // TODO: Improve type mapping

            TypeAnalysisResult result = default;
            var currentType = type;
            while (currentType != null)
            {
                if (currentType is CppPointerType pointerType)
                {
                    result.PointerDepth++;
                    currentType = pointerType.ElementType;
                }
                else if (currentType is CppArrayType arrayType)
                {
                    result.PointerDepth++;
                    currentType = arrayType.ElementType;
                }
                else if (currentType is CppReferenceType referenceType)
                {
                    result.PointerDepth++;
                    currentType = referenceType.ElementType;
                }
                else if (currentType is CppPrimitiveType primitiveType)
                {
                    result.TypeName = GetPrimitiveName(primitiveType);
                    break;
                }
                else if (currentType is CppTypedef typedefType)
                {
                    currentType = typedefType.ElementType;
                }
                else if (currentType is CppClass classType)
                {
                    result.TypeName = GetCTypeName(classType);
                    break;
                }
                else if (currentType is CppEnum enumType)
                {
                    result.TypeName = enumType.Name;
                    break;
                }
                else
                {
                    break;
                }
            }

            StringBuilder sb = new();
            sb.Append(result.TypeName);
            for (int i = 0; i < result.PointerDepth; i++)
            {
                sb.Append('*');
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
            return $"{NamePrefix}_{c.Name}";
        }
    }
}
