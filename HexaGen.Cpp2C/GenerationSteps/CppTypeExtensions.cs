using HexaGen.CppAst.Model.Types;

namespace HexaGen.Cpp2C.GenerationSteps
{
    using HexaGen.CppAst.Model.Declarations;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public static class CppTypeExtensions
    {
        public static bool IsReferenceType(this CppType? type, [NotNullWhen(true)] out CppReferenceType? referenceType)
        {
            while (type != null)
            {
                if (type is CppQualifiedType qualifiedType)
                {
                    type = qualifiedType.ElementType;
                }
                else if (type is CppReferenceType cppReferenceType)
                {
                    referenceType = cppReferenceType;
                    return true;
                }
                else
                {
                    break;
                }
            }
            referenceType = null;
            return false;
        }

        public static bool IsReferenceType(this CppType? type) => IsReferenceType(type, out _);

        public static bool IsEnumType(this CppType? type, [NotNullWhen(true)] out CppEnum? cppEnum)
        {
            while (type != null)
            {
                if (type is CppQualifiedType qualifiedType)
                {
                    type = qualifiedType.ElementType;
                }
                else if (type is CppEnum cppEnum2)
                {
                    cppEnum = cppEnum2;
                    return true;
                }
                else
                {
                    break;
                }
            }
            cppEnum = null;
            return false;
        }

        public static bool IsEnumType(this CppType? type) => IsEnumType(type, out _);

        public static bool IsPointerToClassType(this CppType? type, out int depth, [NotNullWhen(true)] out CppClass? cppClass)
        {
            depth = 0;
            while (true)
            {
                if (type is CppPointerType pointerType)
                {
                    type = pointerType.ElementType;
                    depth++;
                }
                else if (type is CppQualifiedType qualifiedType)
                {
                    type = qualifiedType.ElementType;
                }
                else if (type is CppClass cppClass2)
                {
                    cppClass = cppClass2;
                    return depth > 0;
                }
                else
                {
                    break;
                }
            }
            cppClass = null;
            return false;
        }

        public static bool IsPointerType(this CppType? type)
        {
            while (true)
            {
                if (type is CppPointerType pointerType)
                {
                    return true;
                }
                else if (type is CppQualifiedType qualifiedType)
                {
                    type = qualifiedType.ElementType;
                }
                else
                {
                    break;
                }
            }
            return false;
        }

        public static CppClass? TryFlattenByValue(this CppType? type)
        {
            if (type is CppQualifiedType qualifiedType)
            {
                type = qualifiedType.ElementType;
            }
            if (type is CppArrayType arrayType)
            {
                type = arrayType.ElementType;
            }
            if (type is CppClass cppClass)
            {
                return cppClass;
            }
            return null;
        }

        public static string GetFullName(this CppType type)
        {
            List<CppType> stack = [];
            string canonicalName;
            while (true)
            {
                stack.Add(type);
                if (type is CppPointerType pointerType)
                {
                    type = pointerType.ElementType;
                }
                else if (type is CppQualifiedType qualifiedType)
                {
                    type = qualifiedType.ElementType;
                }
                else if (type is CppArrayType arrayType)
                {
                    type = arrayType.ElementType;
                }
                else if (type is CppTypedef typedefType)
                {
                    canonicalName = typedefType.FullName;
                    break;
                }
                else if (type is CppClass cppClass)
                {
                    canonicalName = cppClass.FullName;
                    break;
                }
                else if (type is CppEnum cppEnum)
                {
                    canonicalName = cppEnum.FullName;
                    break;
                }
                else if (type is CppPrimitiveType primitiveType)
                {
                    canonicalName = primitiveType.FullName;
                    break;
                }
                else if (type is CppFunctionType functionType)
                {
                    canonicalName = functionType.FullName;
                    break;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            StringBuilder sb = new();
            sb.Append(canonicalName);
            for (int i = stack.Count - 2; i >= 0; i--)
            {
                var t = stack[i];
                if (t is CppPointerType)
                {
                    sb.Append('*');
                }
                else if (t is CppReferenceType)
                {
                    sb.Append('&');
                }
                else if (t is CppQualifiedType qualifiedType)
                {
                    switch (qualifiedType.Qualifier)
                    {
                        case CppTypeQualifier.Volatile:
                            sb.Append(" volatile");
                            break;

                        case CppTypeQualifier.Const:
                            sb.Append(" const");
                            break;
                    }
                }
                else if (t is CppArrayType)
                {
                    sb.Append('*');
                }
            }
            return sb.ToString();
        }
    }
}