namespace HexaGen
{
    using CppAst;
    using HexaGen.Core;
    using HexaGen.Core.Mapping;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    public partial class CsCodeGeneratorConfig
    {
        #region Mapping Helpers

        public bool TryGetEnumMapping(string enumName, [NotNullWhen(true)] out EnumMapping? mapping)
        {
            for (int i = 0; i < EnumMappings.Count; i++)
            {
                var enumMapping = EnumMappings[i];
                if (enumMapping.ExportedName == enumName)
                {
                    mapping = enumMapping;
                    return true;
                }
            }

            mapping = null;
            return false;
        }

        public EnumMapping? GetEnumMapping(string enumName)
        {
            for (int i = 0; i < EnumMappings.Count; i++)
            {
                var enumMapping = EnumMappings[i];
                if (enumMapping.ExportedName == enumName)
                {
                    return enumMapping;
                }
            }

            return null;
        }

        public bool TryGetFunctionMapping(string functionName, [NotNullWhen(true)] out FunctionMapping? mapping)
        {
            for (int i = 0; i < FunctionMappings.Count; i++)
            {
                var functionMapping = FunctionMappings[i];
                if (functionMapping.ExportedName == functionName)
                {
                    mapping = functionMapping;
                    return true;
                }
            }

            mapping = null;
            return false;
        }

        public FunctionMapping? GetFunctionMapping(string functionName)
        {
            for (int i = 0; i < FunctionMappings.Count; i++)
            {
                var functionMapping = FunctionMappings[i];
                if (functionMapping.ExportedName == functionName)
                {
                    return functionMapping;
                }
            }

            return null;
        }

        public bool TryGetTypeMapping(string typeName, [NotNullWhen(true)] out TypeMapping? mapping)
        {
            for (int i = 0; i < ClassMappings.Count; i++)
            {
                var structMapping = ClassMappings[i];
                if (structMapping.ExportedName == typeName)
                {
                    mapping = structMapping;
                    return true;
                }
            }

            mapping = null;
            return false;
        }

        public TypeMapping? GetTypeMapping(string typeName)
        {
            for (int i = 0; i < ClassMappings.Count; i++)
            {
                var structMapping = ClassMappings[i];
                if (structMapping.ExportedName == typeName)
                {
                    return structMapping;
                }
            }

            return null;
        }

        public bool TryGetHandleMapping(string typeName, [NotNullWhen(true)] out HandleMapping? mapping)
        {
            for (int i = 0; i < HandleMappings.Count; i++)
            {
                var handleMapping = HandleMappings[i];
                if (handleMapping.ExportedName == typeName)
                {
                    mapping = handleMapping;
                    return true;
                }
            }

            mapping = null;
            return false;
        }

        public HandleMapping? GetHandleMapping(string typeName)
        {
            for (int i = 0; i < HandleMappings.Count; i++)
            {
                var handleMapping = HandleMappings[i];
                if (handleMapping.ExportedName == typeName)
                {
                    return handleMapping;
                }
            }

            return null;
        }

        public bool TryGetDelegateMapping(string delegateName, [NotNullWhen(true)] out DelegateMapping? mapping)
        {
            for (int i = 0; i < DelegateMappings.Count; i++)
            {
                var delegateMapping = DelegateMappings[i];
                if (delegateMapping.Name == delegateName)
                {
                    mapping = delegateMapping;
                    return true;
                }
            }

            mapping = null;
            return false;
        }

        public DelegateMapping? GetDelegateMapping(string delegateName)
        {
            for (int i = 0; i < DelegateMappings.Count; i++)
            {
                var delegateMapping = DelegateMappings[i];
                if (delegateMapping.Name == delegateName)
                {
                    return delegateMapping;
                }
            }

            return null;
        }

        public bool TryGetArrayMapping(CppArrayType arrayType, [NotNullWhen(true)] out string? mapping)
        {
            for (int i = 0; i < ArrayMappings.Count; i++)
            {
                var map = ArrayMappings[i];
                if (map.Primitive == arrayType.GetPrimitiveKind(false) && map.Size == arrayType.Size)
                {
                    mapping = map.Name;
                    return true;
                }
            }
            mapping = null;
            return false;
        }

        #endregion Mapping Helpers

        public string GetCsReturnType(CppType? type)
        {
            if (type == null)
                return string.Empty;

            if (type.IsDelegate(out var outDelegate))
            {
                return MakeDelegatePointer(outDelegate);
            }

            var name = GetCsTypeNameInternal(type);

            return name;
        }

        public string GetCsTypeName(CppType? type, bool isPointer = false)
        {
            if (type == null)
                return string.Empty;

            var name = GetCsTypeNameInternal(type, isPointer);

            return name;
        }

        public string GetCsTypeName(CppPointerType type)
        {
            if (type == null)
                return string.Empty;

            var name = GetCsTypeNameInternal(type);

            return name;
        }

        public string GetCsTypeName(CppPrimitiveType type, bool isPointer)
        {
            if (type == null)
                return string.Empty;

            var name = GetCsTypeNameInternal(type, isPointer);

            return name;
        }

        internal string GetCsTypeNameInternal(CppType? type, bool isPointer = false)
        {
            if (type is CppPrimitiveType primitiveType)
            {
                return GetCsTypeNameInternal(primitiveType, isPointer);
            }

            if (type is CppQualifiedType qualifiedType)
            {
                return GetCsTypeNameInternal(qualifiedType.ElementType, isPointer);
            }

            if (type is CppReferenceType referenceType)
            {
                return GetCsTypeNameInternal(referenceType.ElementType, true);
            }

            if (type is CppEnum enumType)
            {
                var enumCsName = GetCsCleanName(enumType.Name);
                if (isPointer)
                    return enumCsName + "*";

                return enumCsName;
            }

            if (type is CppTypedef typedef)
            {
                if (DefinedCppEnums.TryGetValue(typedef.Name, out var cppEnum) || DefinedCppEnums.TryGetValue($"{typedef.Name}_", out cppEnum))
                {
                    if (isPointer)
                        return cppEnum.Name + "*";
                    return cppEnum.Name;
                }

                if (typedef.ElementType is CppPrimitiveType cppPrimitive)
                {
                    var csPrimitiveName = GetCsTypeNameInternal(cppPrimitive);
                    if (!string.IsNullOrEmpty(csPrimitiveName))
                    {
                        if (isPointer)
                        {
                            return $"{csPrimitiveName}*";
                        }
                        return csPrimitiveName;
                    }
                }

                var typeDefCsName = GetCsCleanName(typedef.Name);
                if (typedef.IsDelegate(out var cppFunction))
                {
                    if (isPointer)
                    {
                        return MakeDelegatePointer(cppFunction) + "*";
                    }
                    else
                    {
                        return typeDefCsName;
                    }
                }

                if (isPointer)
                {
                    return typeDefCsName + "*";
                }

                return typeDefCsName;
            }

            if (type is CppClass @class)
            {
                var className = GetCsCleanName(@class.Name);
                if (isPointer)
                    return className + "*";

                return className;
            }

            if (type is CppPointerType pointerType)
            {
                var pointerName = GetCsTypeNameInternal(pointerType);
                if (isPointer)
                    return pointerName + "*";

                return pointerName;
            }

            if (type is CppArrayType arrayType)
            {
                var arrayName = GetCsTypeNameInternal(arrayType.ElementType, false);
                return arrayName + "*";
            }

            if (type is CppUnexposedType unexposedType)
            {
                throw new UnexposedTypeException(unexposedType);
            }

            return string.Empty;
        }

        private string GetCsTypeNameInternal(CppPointerType pointerType)
        {
            if (pointerType.ElementType is CppQualifiedType qualifiedType)
            {
                if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
                {
                    return GetCsTypeNameInternal(primitiveType, true);
                }
                else if (qualifiedType.ElementType is CppClass @classType)
                {
                    return GetCsTypeNameInternal(@classType, true);
                }
                else if (qualifiedType.ElementType is CppPointerType subPointerType)
                {
                    return GetCsTypeNameInternal(subPointerType.ElementType, true) + "*";
                }
                else if (qualifiedType.ElementType is CppTypedef typedef)
                {
                    return GetCsTypeNameInternal(typedef, true);
                }
                else if (qualifiedType.ElementType is CppEnum @enum)
                {
                    return GetCsTypeNameInternal(@enum, true);
                }

                return GetCsTypeNameInternal(qualifiedType.ElementType, true);
            }

            if (pointerType.ElementType is CppFunctionType functionType)
            {
                return MakeDelegatePointer(functionType);
            }

            if (pointerType.ElementType is CppPointerType subPointer)
            {
                return GetCsTypeNameInternal(subPointer) + "*";
            }

            return GetCsTypeNameInternal(pointerType.ElementType, true);
        }

        public string MakeDelegatePointer(CppFunctionType functionType)
        {
            if (functionType.Parameters.Count == 0)
            {
                return $"delegate*<{GetCsTypeNameInternal(functionType.ReturnType)}>";
            }
            else
            {
                return $"delegate*<{GetNamelessParameterSignature(functionType.Parameters, false, true)}, {GetCsTypeNameInternal(functionType.ReturnType)}>";
            }
        }

        private string GetCsTypeNameInternal(CppPrimitiveType primitiveType, bool isPointer)
        {
            switch (primitiveType.Kind)
            {
                case CppPrimitiveKind.Void:
                    return isPointer ? "void*" : "void";

                case CppPrimitiveKind.Char:
                    return isPointer ? "byte*" : "byte";

                case CppPrimitiveKind.Bool:
                    return isPointer ? $"{GetBoolType(isPointer)}*" : "bool";

                case CppPrimitiveKind.WChar:
                    return isPointer ? "char*" : "char";

                case CppPrimitiveKind.Short:
                    return isPointer ? "short*" : "short";

                case CppPrimitiveKind.Int:
                    return isPointer ? "int*" : "int";

                case CppPrimitiveKind.LongLong:
                    return isPointer ? "long*" : "long";

                case CppPrimitiveKind.UnsignedChar:
                    return isPointer ? "byte*" : "byte";

                case CppPrimitiveKind.UnsignedShort:
                    return isPointer ? "ushort*" : "ushort";

                case CppPrimitiveKind.UnsignedInt:
                    return isPointer ? "uint*" : "uint";

                case CppPrimitiveKind.UnsignedLongLong:
                    return isPointer ? "ulong*" : "ulong";

                case CppPrimitiveKind.Float:
                    return isPointer ? "float*" : "float";

                case CppPrimitiveKind.Double:
                    return isPointer ? "double*" : "double";

                case CppPrimitiveKind.LongDouble:
                    break;

                default:
                    return string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// This method will return <see langword="ref"/> <see cref="T"/> instead of <see cref="T"/>*
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isPointer"></param>
        /// <returns></returns>
        public string GetCsWrapperTypeName(CppType? type, bool isPointer = false)
        {
            if (type == null)
                return string.Empty;

            var name = GetCsWrapperTypeNameInternal(type, isPointer);
            if (name == "ref void") // exception for void type
            {
                name = "void*";
            }

            return name;
        }

        /// <summary>
        /// This method will return <see langword="ref"/> <see cref="T"/> instead of <see cref="T"/>*
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetCsWrapperTypeName(CppPointerType type)
        {
            if (type == null)
                return string.Empty;

            var name = GetCsWrapperTypeNameInternal(type);

            return name;
        }

        /// <summary>
        /// This method will return <see langword="ref"/> <see cref="T"/> instead of <see cref="T"/>*
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isPointer"></param>
        /// <returns></returns>
        public string GetCsWrapperTypeName(CppPrimitiveType type, bool isPointer)
        {
            if (type == null)
                return string.Empty;

            var name = GetCsWrapperTypeNameInternal(type, isPointer);

            return name;
        }

        private string GetCsWrapperTypeNameInternal(CppType? type, bool isPointer = false)
        {
            if (type is CppPrimitiveType primitiveType)
            {
                return GetCsWrapperTypeNameInternal(primitiveType, isPointer);
            }

            if (type is CppQualifiedType qualifiedType)
            {
                return GetCsWrapperTypeNameInternal(qualifiedType.ElementType, isPointer);
            }

            if (type is CppReferenceType referenceType)
            {
                return GetCsWrapperTypeNameInternal(referenceType.ElementType, true);
            }

            if (type is CppEnum enumType)
            {
                var enumCsName = GetCsCleanName(enumType.Name);
                if (isPointer)
                    return "ref " + enumCsName;

                return enumCsName;
            }

            if (type is CppTypedef typedef)
            {
                if (DefinedCppEnums.TryGetValue(typedef.Name, out var cppEnum) || DefinedCppEnums.TryGetValue($"{typedef.Name}_", out cppEnum))
                {
                    if (isPointer)
                        return cppEnum.Name + "*";
                    return cppEnum.Name;
                }

                if (typedef.ElementType is CppPrimitiveType cppPrimitive)
                {
                    var csPrimitiveName = GetCsWrapperTypeNameInternal(cppPrimitive);
                    if (!string.IsNullOrEmpty(csPrimitiveName))
                    {
                        if (isPointer)
                        {
                            return $"ref {csPrimitiveName}";
                        }
                        return csPrimitiveName;
                    }
                }

                var typeDefCsName = GetCsCleanName(typedef.Name);
                if (typedef.IsDelegate(out var cppFunction))
                {
                    if (isPointer)
                    {
                        return MakeDelegatePointer(cppFunction) + "*";
                    }
                    else
                    {
                        return typeDefCsName;
                    }
                }

                if (isPointer && typeDefCsName == "void")
                    return "void*";

                if (isPointer)
                    return "ref " + typeDefCsName;

                return typeDefCsName;
            }

            if (type is CppClass @class)
            {
                var className = GetCsCleanName(@class.Name);
                if (isPointer && className == "void")
                    return "void*";
                if (isPointer)
                    return "ref " + className;

                return className;
            }

            if (type is CppPointerType pointerType)
            {
                return GetCsWrapperTypeNameInternal(pointerType);
            }

            if (type is CppArrayType arrayType && arrayType.Size > 0)
            {
                if (TryGetArrayMapping(arrayType, out string? mapping))
                {
                    return mapping;
                }

                return GetCsWrapperTypeNameInternal(arrayType.ElementType, true);
            }
            else if (type is CppArrayType arrayType1 && arrayType1.Size < 0)
            {
                var arrayName = GetCsTypeName(arrayType1.ElementType, false);
                return arrayName + "*";
            }

            return string.Empty;
        }

        private string GetCsWrapperTypeNameInternal(CppPrimitiveType primitiveType, bool isPointer)
        {
            switch (primitiveType.Kind)
            {
                case CppPrimitiveKind.Void:
                    return isPointer ? "void*" : "void";

                case CppPrimitiveKind.Char:
                    return isPointer ? "ref byte" : "byte";

                case CppPrimitiveKind.Bool:
                    return isPointer ? $"ref {GetBoolType(isPointer)}" : "bool";

                case CppPrimitiveKind.WChar:
                    return isPointer ? "ref char" : "char";

                case CppPrimitiveKind.Short:
                    return isPointer ? "ref short" : "short";

                case CppPrimitiveKind.Int:
                    return isPointer ? "ref int" : "int";

                case CppPrimitiveKind.LongLong:
                    break;

                case CppPrimitiveKind.UnsignedChar:
                    return isPointer ? "ref byte" : "byte";

                case CppPrimitiveKind.UnsignedShort:
                    return isPointer ? "ref ushort" : "ushort";

                case CppPrimitiveKind.UnsignedInt:
                    return isPointer ? "ref uint" : "uint";

                case CppPrimitiveKind.UnsignedLongLong:
                    break;

                case CppPrimitiveKind.Float:
                    return isPointer ? "ref float" : "float";

                case CppPrimitiveKind.Double:
                    return isPointer ? "ref double" : "double";

                case CppPrimitiveKind.LongDouble:
                    break;

                default:
                    return string.Empty;
            }

            return string.Empty;
        }

        private string GetCsWrapperTypeNameInternal(CppPointerType pointerType)
        {
            if (pointerType.ElementType is CppQualifiedType qualifiedType)
            {
                if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
                {
                    return GetCsWrapperTypeNameInternal(primitiveType, true);
                }
                else if (qualifiedType.ElementType is CppClass @classType)
                {
                    return GetCsWrapperTypeNameInternal(@classType, true);
                }
                else if (qualifiedType.ElementType is CppPointerType subPointerType)
                {
                    return GetCsWrapperTypeNameInternal(subPointerType, true) + "*";
                }
                else if (qualifiedType.ElementType is CppTypedef typedef)
                {
                    return GetCsWrapperTypeNameInternal(typedef, true);
                }
                else if (qualifiedType.ElementType is CppEnum @enum)
                {
                    return GetCsWrapperTypeNameInternal(@enum, true);
                }

                return GetCsWrapperTypeNameInternal(qualifiedType.ElementType, true);
            }

            if (pointerType.ElementType is CppFunctionType functionType)
            {
                return MakeDelegatePointer(functionType);
            }

            if (pointerType.ElementType is CppPointerType subPointer)
            {
                return GetCsWrapperTypeNameInternal(subPointer) + "*";
            }

            return GetCsWrapperTypeNameInternal(pointerType.ElementType, true);
        }

        /// <summary>
        /// This method will return <see cref="Pointer&lt;T&gt;"/> instead of <see cref="T"/>*
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isPointer"></param>
        /// <returns></returns>
        public string GetCsWrappedPointerTypeName(CppType? type, bool isPointer = false)
        {
            if (type == null)
                return string.Empty;

            var name = GetCsWrappedPointerTypeNameInternal(type, isPointer);
            if (type is CppPointerType pointerType && pointerType.ElementType is CppFunctionType)
            {
                name = "nint";
            }

            return name;
        }

        /// <summary>
        /// This method will return <see cref="Pointer&lt;T&gt;"/> instead of <see cref="T"/>*
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetCsWrappedPointerTypeName(CppPointerType type)
        {
            if (type == null)
                return string.Empty;

            var name = GetCsWrappedPointerTypeNameInternal(type);

            return name;
        }

        /// <summary>
        /// This method will return <see cref="Pointer&lt;T&gt;"/> instead of <see cref="T"/>*
        /// </summary>
        /// <param name="type"></param>
        /// <param name="isPointer"></param>
        /// <returns></returns>
        public string GetCsWrappedPointerTypeName(CppPrimitiveType type, bool isPointer)
        {
            if (type == null)
                return string.Empty;

            var name = GetCsWrappedPointerTypeNameInternal(type, isPointer);

            return name;
        }

        private string GetCsWrappedPointerTypeNameInternal(CppType? type, bool isPointer = false)
        {
            if (type is CppPrimitiveType primitiveType)
            {
                return GetCsWrappedPointerTypeNameInternal(primitiveType, isPointer);
            }

            if (type is CppQualifiedType qualifiedType)
            {
                return GetCsWrappedPointerTypeNameInternal(qualifiedType.ElementType, isPointer);
            }

            if (type is CppReferenceType referenceType)
            {
                return GetCsWrappedPointerTypeNameInternal(referenceType.ElementType, true);
            }

            if (type is CppEnum enumType)
            {
                var enumCsName = GetCsCleanName(enumType.Name);
                if (isPointer)
                    return $"Pointer<{enumCsName}>";

                return enumCsName;
            }

            if (type is CppTypedef typedef)
            {
                if (DefinedCppEnums.TryGetValue(typedef.Name, out var cppEnum) || DefinedCppEnums.TryGetValue($"{typedef.Name}_", out cppEnum))
                {
                    if (isPointer)
                        return cppEnum.Name + "*";
                    return cppEnum.Name;
                }

                if (typedef.ElementType is CppPrimitiveType cppPrimitive)
                {
                    var csPrimitiveName = GetCsWrappedPointerTypeNameInternal(cppPrimitive);
                    if (!string.IsNullOrEmpty(csPrimitiveName))
                    {
                        if (isPointer)
                        {
                            return $"Pointer<{csPrimitiveName}>";
                        }

                        return csPrimitiveName;
                    }
                }

                var typeDefCsName = GetCsCleanName(typedef.Name);
                if (typedef.IsDelegate())
                {
                    return typeDefCsName;
                }
                if (isPointer && typeDefCsName == "void")
                    return "nint";
                if (isPointer)
                    return $"Pointer<{typeDefCsName}>";

                return typeDefCsName;
            }

            if (type is CppClass @class)
            {
                var className = GetCsCleanName(@class.Name);
                if (isPointer && className == "void")
                    return "nint";
                if (isPointer)
                    return $"Pointer<{className}>";

                return className;
            }

            if (type is CppPointerType pointerType)
            {
                return GetCsWrappedPointerTypeNameInternal(pointerType);
            }

            if (type is CppArrayType arrayType && arrayType.Size > 0)
            {
                if (TryGetArrayMapping(arrayType, out string? mapping))
                {
                    return mapping;
                }

                return GetCsWrappedPointerTypeNameInternal(arrayType.ElementType, true);
            }
            else if (type is CppArrayType arrayType1 && arrayType1.Size < 0)
            {
                var arrayName = GetCsTypeName(arrayType1.ElementType, false);
                return $"Pointer<{arrayName}>";
            }

            return string.Empty;
        }

        private string GetCsWrappedPointerTypeNameInternal(CppPrimitiveType primitiveType, bool isPointer)
        {
            switch (primitiveType.Kind)
            {
                case CppPrimitiveKind.Void:
                    return isPointer ? "nint" : "nint";

                case CppPrimitiveKind.Char:
                    return isPointer ? "Pointer<byte>" : "byte";

                case CppPrimitiveKind.Bool:
                    return isPointer ? $"Pointer<{GetBoolType(isPointer)}>" : "bool";

                case CppPrimitiveKind.WChar:
                    return isPointer ? "Pointer<char>" : "char";

                case CppPrimitiveKind.Short:
                    return isPointer ? "Pointer<short>" : "short";

                case CppPrimitiveKind.Int:
                    return isPointer ? "Pointer<int>" : "int";

                case CppPrimitiveKind.LongLong:
                    return isPointer ? "Pointer<long>" : "long";

                case CppPrimitiveKind.UnsignedChar:
                    return isPointer ? "Pointer<byte>" : "byte";

                case CppPrimitiveKind.UnsignedShort:
                    return isPointer ? "Pointer<ushort>" : "ushort";

                case CppPrimitiveKind.UnsignedInt:
                    return isPointer ? "Pointer<uint>" : "uint";

                case CppPrimitiveKind.UnsignedLongLong:
                    return isPointer ? "Pointer<ulong>" : "ulong";

                case CppPrimitiveKind.Float:
                    return isPointer ? "Pointer<float>" : "float";

                case CppPrimitiveKind.Double:
                    return isPointer ? "Pointer<double>" : "double";

                case CppPrimitiveKind.LongDouble:
                    break;

                default:
                    return string.Empty;
            }

            return string.Empty;
        }

        private string GetCsWrappedPointerTypeNameInternal(CppPointerType pointerType)
        {
            if (pointerType.ElementType is CppQualifiedType qualifiedType)
            {
                if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
                {
                    return GetCsWrappedPointerTypeNameInternal(primitiveType, true);
                }
                else if (qualifiedType.ElementType is CppClass @classType)
                {
                    return GetCsWrappedPointerTypeNameInternal(@classType, true);
                }
                else if (qualifiedType.ElementType is CppPointerType subPointerType)
                {
                    return $"Pointer<{GetCsWrappedPointerTypeNameInternal(subPointerType, true)}>";
                }
                else if (qualifiedType.ElementType is CppTypedef typedef)
                {
                    return GetCsWrappedPointerTypeNameInternal(typedef, true);
                }
                else if (qualifiedType.ElementType is CppEnum @enum)
                {
                    return GetCsWrappedPointerTypeNameInternal(@enum, true);
                }

                return GetCsWrappedPointerTypeNameInternal(qualifiedType.ElementType, true);
            }

            if (pointerType.ElementType is CppFunctionType functionType)
            {
                return MakeDelegatePointer(functionType);
            }

            if (pointerType.ElementType is CppPointerType subPointer)
            {
                return $"Pointer<{GetCsWrappedPointerTypeNameInternal(subPointer)}>";
            }

            return GetCsWrappedPointerTypeNameInternal(pointerType.ElementType, true);
        }

        public string GetParameterSignature(IList<CppParameter> parameters, bool canUseOut, bool attributes = true, bool names = true, bool delegateType = false, bool compatibility = false)
        {
            StringBuilder argumentBuilder = new();
            int index = 0;

            for (int i = 0; i < parameters.Count; i++)
            {
                CppParameter cppParameter = parameters[i];
                var paramCsTypeName = GetCsTypeName(cppParameter.Type, false);
                var paramCsName = GetParameterName(i, cppParameter.Name);

                CppType ptrType = cppParameter.Type;
                int depth = 0;
                if (cppParameter.Type.IsPointer(ref depth, out var pointerType))
                {
                    ptrType = pointerType;
                }

                if ((delegateType || ptrType != cppParameter.Type) && ptrType is CppTypedef typedef && typedef.ElementType.IsDelegate(out var cppFunction) && !paramCsTypeName.Contains('*'))
                {
                    paramCsTypeName = MakeDelegatePointer(cppFunction);

                    while (depth-- > 0)
                    {
                        paramCsTypeName += "*";
                    }
                }

                if (attributes)
                {
                    argumentBuilder.Append($"[NativeName(NativeNameType.Param, \"{cppParameter.Name}\")] ");
                    argumentBuilder.Append($"[NativeName(NativeNameType.Type, \"{cppParameter.Type.GetDisplayName()}\")] ");
                }

                if (paramCsTypeName == "bool")
                {
                    paramCsTypeName = GetBoolType();
                }

                if (canUseOut && cppParameter.Type.CanBeUsedAsOutput(out CppTypeDeclaration? cppTypeDeclaration))
                {
                    argumentBuilder.Append("out ");
                    paramCsTypeName = GetCsTypeName(cppTypeDeclaration, false);
                }

                if (compatibility && paramCsTypeName.Contains('*'))
                {
                    paramCsTypeName = "nint";
                }

                argumentBuilder.Append(paramCsTypeName);

                if (names)
                {
                    argumentBuilder.Append(' ').Append(paramCsName);
                }

                if (index < parameters.Count - 1)
                {
                    argumentBuilder.Append(", ");
                }

                index++;
            }

            return argumentBuilder.ToString();
        }

        public string GetParameterSignatureNames(IList<CppParameter> parameters)
        {
            StringBuilder argumentBuilder = new();
            int index = 0;

            for (int i = 0; i < parameters.Count; i++)
            {
                CppParameter cppParameter = parameters[i];
                var paramCsTypeName = GetCsTypeName(cppParameter.Type, false);
                var paramCsName = GetParameterName(i, cppParameter.Name);

                CppType ptrType = cppParameter.Type;
                int depth = 0;
                if (cppParameter.Type.IsPointer(ref depth, out var pointerType))
                {
                    ptrType = pointerType;
                }

                argumentBuilder.Append(paramCsName);

                if (index < parameters.Count - 1)
                {
                    argumentBuilder.Append(", ");
                }

                index++;
            }

            return argumentBuilder.ToString();
        }

        public string GetNamelessParameterSignature(IList<CppParameter> parameters, bool canUseOut, bool delegateType = false, bool compatibility = false)
        {
            var argumentBuilder = new StringBuilder();
            int index = 0;

            foreach (CppParameter cppParameter in parameters)
            {
                string direction = string.Empty;
                var paramCsTypeName = GetCsTypeName(cppParameter.Type, false);

                CppType ptrType = cppParameter.Type;
                int depth = 0;
                if (cppParameter.Type.IsPointer(ref depth, out var pointerType))
                {
                    ptrType = pointerType;
                }

                if (cppParameter.Type is CppQualifiedType qualifiedType)
                {
                    ptrType = qualifiedType.ElementType;
                }

                if (delegateType && ptrType is CppTypedef typedef && typedef.ElementType.IsDelegate(out var cppFunction))
                {
                    if (cppFunction.Parameters.Count == 0)
                    {
                        paramCsTypeName = $"delegate*<{GetCsTypeName(cppFunction.ReturnType)}>";
                    }
                    else
                    {
                        paramCsTypeName = $"delegate*<{GetNamelessParameterSignature(cppFunction.Parameters, false, delegateType)}, {GetCsTypeName(cppFunction.ReturnType)}>";
                    }

                    while (depth-- > 0)
                    {
                        paramCsTypeName += "*";
                    }
                }

                if (paramCsTypeName == "bool")
                {
                    paramCsTypeName = "byte";
                }

                if (canUseOut && cppParameter.Type.CanBeUsedAsOutput(out CppTypeDeclaration? cppTypeDeclaration))
                {
                    argumentBuilder.Append("out ");
                    paramCsTypeName = GetCsTypeName(cppTypeDeclaration, false);
                }

                if (compatibility && paramCsTypeName.Contains('*'))
                {
                    paramCsTypeName = "nint";
                }

                argumentBuilder.Append(paramCsTypeName);
                if (index < parameters.Count - 1)
                {
                    argumentBuilder.Append(", ");
                }

                index++;
            }

            return argumentBuilder.ToString();
        }

        public string WriteFunctionMarshalling(IList<CppParameter> parameters, bool compatibility = false)
        {
            var argumentBuilder = new StringBuilder();
            int index = 0;

            for (int i = 0; i < parameters.Count; i++)
            {
                CppParameter cppParameter = parameters[i];
                string direction = string.Empty;
                var paramCsTypeName = GetCsTypeName(cppParameter.Type, false);
                var paramCsName = GetParameterName(i, cppParameter.Name);

                CppType ptrType = cppParameter.Type;
                int depth = 0;
                if (cppParameter.Type.IsPointer(ref depth, out var pointerType))
                {
                    ptrType = pointerType;
                }

                if (cppParameter.Type is CppQualifiedType qualifiedType)
                {
                    ptrType = qualifiedType.ElementType;
                }

                if (ptrType is CppTypedef typedef && typedef.ElementType.IsDelegate(out var cppFunction))
                {
                    if (cppFunction.Parameters.Count == 0)
                    {
                        paramCsTypeName = $"delegate*<{GetCsTypeName(cppFunction.ReturnType)}>";
                    }
                    else
                    {
                        paramCsTypeName = $"delegate*<{GetNamelessParameterSignature(cppFunction.Parameters, false, true)}, {GetCsTypeName(cppFunction.ReturnType)}>";
                    }

                    bool isPointer = depth > 0;

                    while (depth-- > 0)
                    {
                        paramCsTypeName += "*";
                    }

                    if (compatibility && paramCsTypeName.Contains('*'))
                    {
                        paramCsTypeName = "nint";
                    }

                    if (isPointer)
                    {
                        if (compatibility)
                        {
                            argumentBuilder.Append($"(nint){paramCsName}");
                        }
                        else
                        {
                            argumentBuilder.Append($"{paramCsName}");
                        }
                    }
                    else
                    {
                        argumentBuilder.Append($"({paramCsTypeName})Utils.GetFunctionPointerForDelegate({paramCsName})");
                    }
                }
                else
                {
                    if (compatibility && (paramCsTypeName.Contains('*') || depth > 0))
                    {
                        argumentBuilder.Append($"(nint){paramCsName}");
                    }
                    else
                    {
                        argumentBuilder.Append($"{paramCsName}");
                    }
                }

                if (index < parameters.Count - 1)
                {
                    argumentBuilder.Append(", ");
                }

                index++;
            }

            return argumentBuilder.ToString();
        }

        private readonly ConcurrentDictionary<string, string> parameterNameCache = new();

        public string GetParameterName(int paramIdx, string name)
        {
            if (name == "out")
            {
                return "output";
            }
            if (name == "ref")
            {
                return "reference";
            }
            if (name == "in")
            {
                return "input";
            }
            if (name == "base")
            {
                return "baseValue";
            }
            if (name == "void")
            {
                return "voidValue";
            }
            if (name == "int")
            {
                return "intValue";
            }
            if (name == "lock")
            {
                return "lock0";
            }
            if (name == "event")
            {
                return "evnt";
            }
            if (name == "string")
            {
                return "str";
            }
            if (Keywords.Contains(name))
            {
                return "@" + name;
            }

            if (name.StartsWith('p') && name.Length > 1 && char.IsUpper(name[1]))
            {
            }

            if (name == string.Empty)
            {
                return $"unknown{paramIdx}";
            }

            return NormalizeParameterName(name);
        }

        public string GetDelegateName(string name)
        {
            return GetCsCleanNameWithConvention(name, DelegateNamingConvention, false);
        }

        public string NormalizeParameterName(string name)
        {
            if (parameterNameCache.TryGetValue(name, out var newName))
            {
                return newName;
            }

            name = name.Trim('_');

            newName = NamingHelper.ConvertTo(name, ParameterNamingConvention);

            if (Keywords.Contains(newName))
            {
                newName = "@" + newName;
            }

            if (char.IsDigit(newName[0]))
            {
                newName = "_" + newName;
            }

            parameterNameCache.TryAdd(name, newName);

            return newName;
        }

        public string? NormalizeValue(string value, bool sanitize)
        {
            if (KnownDefaultValueNames.TryGetValue(value, out var names))
            {
                return names;
            }

            if (value == "NULL")
                return "default";
            if (value == "FLT_MAX")
                return "float.MaxValue";
            if (value == "-FLT_MAX")
                return "-float.MaxValue";
            if (value == "FLT_MIN")
                return "float.MinValue";
            if (value == "-FLT_MIN")
                return "-float.MinValue";
            if (value == "nullptr")
                return "default";
            if (value == "false")
                return "0";
            if (value == "true")
                return "1";

            // TODO: needs refactoring. remove the ImVec!
            if (value.StartsWith("ImVec") && sanitize)
                return null;
            if (value.StartsWith("ImVec2"))
            {
                value = value[7..][..(value.Length - 8)];
                var parts = value.Split(',');
                return $"new Vector2({NormalizeValue(parts[0], sanitize)},{NormalizeValue(parts[1], sanitize)})";
            }
            if (value.StartsWith("ImVec4"))
            {
                value = value[7..][..(value.Length - 8)];
                var parts = value.Split(',');
                return $"new Vector4({NormalizeValue(parts[0], sanitize)},{NormalizeValue(parts[1], sanitize)},{NormalizeValue(parts[2], sanitize)},{NormalizeValue(parts[3], sanitize)})";
            }
            return value;
        }

        public string GetCsFunctionName(string function)
        {
            if (TryGetFunctionMapping(function, out var mapping))
            {
                if (mapping.FriendlyName != null)
                    return mapping.FriendlyName;
            }

            string[] parts = GetCsCleanName(function).SplitByCase();

            StringBuilder sb = new();
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];

                if (IgnoredParts.Contains(part))
                {
                    continue;
                }

                sb.Append(part);
            }

            return sb.ToString();
        }

        public bool TryGetDefaultValue(string functionName, CppParameter parameter, bool sanitize, out string? defaultValue)
        {
            if (TryGetFunctionMapping(functionName, out var mapping))
            {
                if (mapping.Defaults.TryGetValue(parameter.Name, out var value))
                {
                    if (parameter.Type is CppTypedef typedef && typedef.ElementType is CppPrimitiveType type)
                    {
                        if (value.IsNumeric())
                        {
                            defaultValue = value;
                            return true;
                        }

                        string csName = GetCsCleanName(typedef.Name);
                        EnumPrefix enumNamePrefix = GetEnumNamePrefix(typedef.Name);
                        if (csName.EndsWith("_"))
                        {
                            csName = csName.Remove(csName.Length - 1);
                        }
                        var enumItemName = GetEnumName(value, enumNamePrefix);

                        defaultValue = $"{csName}.{enumItemName}";
                        return true;
                    }
                    defaultValue = NormalizeValue(value, sanitize);
                    return true;
                }
            }
            defaultValue = null;
            return false;
        }

        public string GetConstantName(string value)
        {
            if (KnownConstantNames.TryGetValue(value, out string? knownName))
            {
                return knownName;
            }

            return GetCsCleanNameWithConvention(value, ConstantNamingConvention, false);
        }

        public EnumPrefix GetEnumNamePrefix(string typeName)
        {
            if (KnownEnumPrefixes.TryGetValue(typeName, out string? knownValue))
            {
                return new(knownValue.Split('_'));
            }

            string[] parts = typeName.Split('_', StringSplitOptions.RemoveEmptyEntries).SelectMany(x => x.SplitByCase()).ToArray();
            List<string> partList = new();
            bool mergeWithLast = false;
            int last = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if ((part.IsNumeric()) && !mergeWithLast)
                {
                    if (i == 0 && parts.Length > 1)
                    {
                        mergeWithLast = true;
                    }
                    else if (i > 0)
                    {
                        partList[last] += part;
                    }
                    else
                    {
                        last = partList.Count;
                        partList.Add(part);
                    }
                }
                else if (mergeWithLast)
                {
                    last = partList.Count;
                    partList.Add(parts[last] + part);
                    mergeWithLast = false;
                }
                else
                {
                    last = partList.Count;
                    partList.Add(part);
                }
            }

            return new(partList.ToArray());
        }

        public EnumPrefix GetEnumNamePrefixEx(string typeName)
        {
            if (KnownEnumPrefixes.TryGetValue(typeName, out string? knownValue))
            {
                return new(knownValue.Split('_'));
            }

            string[] parts = typeName.Split('_', StringSplitOptions.RemoveEmptyEntries).SelectMany(x => x.SplitByCase()).ToArray();
            List<string> partList = new();

            string compositeA = "";
            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                partList.Add(part);
                compositeA += part;

                if (!partList.Contains(compositeA))
                {
                    partList.Add(compositeA);
                }

                string compositeB = "";
                for (int j = i; j < parts.Length; j++)
                {
                    compositeB += parts[j];
                    if (!partList.Contains(compositeB))
                    {
                        partList.Add(compositeB);
                    }
                }

                var subParts = part.SplitByCase();
                if (subParts.Length > 1)
                {
                    partList.AddRange(subParts);
                    string composite = "";
                    for (int j = 0; j < subParts.Length - 1; j++)
                    {
                        composite += subParts[j];
                        if (!partList.Contains(composite))
                        {
                            partList.Add(composite);
                        }
                    }
                }
            }

            return new([.. partList]);
        }

        public string GetEnumNameEx(string value, EnumPrefix enumPrefix)
        {
            return GetEnumName(value, enumPrefix);
            /*
            if (KnownEnumValueNames.TryGetValue(value, out string? knownName))
            {
                return knownName;
            }

            string[] parts = GetEnumNamePrefix(value).Parts;
            string[] prefixParts = enumPrefix.Parts;

            var name = GetCsCleanNameWithConvention(value, EnumNamingConvention, false);

            int lastRemoved = -1;
            for (int i = 0; i < prefixParts.Length; i++)
            {
                var part = prefixParts[i];
                if (name.StartsWith(part, StringComparison.InvariantCultureIgnoreCase) && name.Length - part.Length != 0)
                {
                    name = name[part.Length..];
                    lastRemoved = i;
                }
            }

            bool capture = false;
            var sb = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (IgnoredParts.Contains(part, StringComparer.InvariantCultureIgnoreCase) || prefixParts.Contains(part, StringComparer.InvariantCultureIgnoreCase) && !capture)
                {
                    continue;
                }

                part = part.ToLower();

                bool wasNum = false;
                for (int j = 0; j < part.Length; j++)
                {
                    var c = part[j];
                    if (j == 0 || wasNum)
                    {
                        sb.Append(char.ToUpper(c));
                        wasNum = false;
                    }
                    else if (char.IsDigit(c))
                    {
                        sb.Append(c);
                        wasNum = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }

                capture = true;
            }

            if (sb.Length == 0)
            {
                sb.Append(parts[^1].ToCamelCase());
            }

            string prettyName = sb.ToString();

            if (char.IsDigit(name[0]))
            {
                name = prefixParts[lastRemoved].ToCamelCase() + name;
            }

            return name; //char.IsNumber(prettyName[0]) ? parts[^1].ToCamelCase() + prettyName : prettyName;
            */
        }

        public string GetEnumName(string value, EnumPrefix enumPrefix)
        {
            if (value.StartsWith("0x"))
                return value;

            string[] parts = GetEnumNamePrefix(value).Parts;
            string[] prefixParts = enumPrefix.Parts;

            bool capture = false;
            var sb = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (IgnoredParts.Contains(part, StringComparer.InvariantCultureIgnoreCase) || prefixParts.Contains(part, StringComparer.InvariantCultureIgnoreCase) && !capture)
                {
                    continue;
                }

                part = part.ToLower();

                bool wasNum = false;
                for (int j = 0; j < part.Length; j++)
                {
                    var c = part[j];
                    if (j == 0 || wasNum)
                    {
                        sb.Append(char.ToUpper(c));
                        wasNum = false;
                    }
                    else if (char.IsDigit(c))
                    {
                        sb.Append(c);
                        wasNum = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }

                capture = true;
            }

            if (sb.Length == 0)
            {
                sb.Append(parts[^1].ToCamelCase());
            }

            string prettyName = sb.ToString();

            return char.IsNumber(prettyName[0]) ? parts[^1].ToCamelCase() + prettyName : prettyName;
        }

        public string GetExtensionNamePrefix(string typeName)
        {
            if (KnownExtensionPrefixes.TryGetValue(typeName, out string? knownValue))
            {
                return knownValue;
            }

            string[] parts = typeName.Split('_', StringSplitOptions.RemoveEmptyEntries).SelectMany(x => x.SplitByCase()).ToArray();

            return string.Join("_", parts.Select(s => s.ToUpper()));
        }

        public string GetExtensionName(string value, string extensionPrefix)
        {
            if (KnownExtensionNames.TryGetValue(value, out string? knownName))
            {
                return knownName;
            }

            string[] parts = value.Split('_', StringSplitOptions.RemoveEmptyEntries).SelectMany(x => x.SplitByCase()).ToArray();
            string[] prefixParts = extensionPrefix.Split('_', StringSplitOptions.RemoveEmptyEntries);

            bool capture = false;
            var sb = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (prefixParts.Contains(part, StringComparer.InvariantCultureIgnoreCase) && !capture)
                {
                    continue;
                }

                part = part.ToLower();

                sb.Append(char.ToUpper(part[0]));
                sb.Append(part[1..]);
                capture = true;
            }

            if (sb.Length == 0)
                sb.Append(value);

            string prettyName = sb.ToString();
            return (char.IsNumber(prettyName[0])) ? prefixParts[^1].ToCamelCase() + prettyName : prettyName;
        }

        public string GetFieldName(string name)
        {
            var parts = name.Split('_', StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new();
            for (int i = 0; i < parts.Length; i++)
            {
                sb.Append(char.ToUpper(parts[i][0]));
                sb.Append(parts[i][1..]);
            }
            name = sb.ToString();
            if (Keywords.Contains(name))
            {
                return "@" + name;
            }

            if (name.Length == 0)
                return name;

            return char.IsDigit(name[0]) ? '_' + name : name;
        }

        public unsafe string GetBoolType()
        {
            return BoolType switch
            {
                BoolType.Bool8 => "byte",
                BoolType.Bool32 => "int",
                _ => throw new NotSupportedException(),
            };
        }

        public unsafe string GetBoolType(bool ptr)
        {
            if (ptr && BoolType == BoolType.Bool8)
                return "bool";
            return BoolType switch
            {
                BoolType.Bool8 => "byte",
                BoolType.Bool32 => "int",
                _ => throw new NotSupportedException(),
            };
        }

        public bool WriteCsSummary(string? comment, ICodeWriter writer)
        {
            if (comment == null)
            {
                if (GeneratePlaceholderComments)
                {
                    writer.WriteLine("/// <summary>");
                    writer.WriteLine("/// To be documented.");
                    writer.WriteLine("/// </summary>");
                    return true;
                }
                return false;
            }

            var lines = comment.Replace("/", string.Empty).Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            writer.WriteLine("/// <summary>");
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                writer.WriteLine($"/// {new XText(line)}<br/>");
            }
            writer.WriteLine($"/// </summary>");
            return true;
        }

        public bool WriteCsSummary(string? comment, out string? com)
        {
            com = null;
            StringBuilder sb = new();
            if (comment == null)
            {
                if (GeneratePlaceholderComments)
                {
                    sb.AppendLine("/// <summary>");
                    sb.AppendLine("/// To be documented.");
                    sb.AppendLine("/// </summary>");
                    com = sb.ToString();
                    return true;
                }
                return false;
            }

            var lines = comment.Replace("/", string.Empty).Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            sb.AppendLine("/// <summary>");
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                sb.AppendLine($"/// {new XText(line)}<br/>");
            }
            sb.AppendLine($"/// </summary>");
            com = sb.ToString();
            return true;
        }

        public string? WriteCsSummary(string? comment)
        {
            WriteCsSummary(comment, out var result);
            return result;
        }

        public bool WriteCsSummary(CppComment? comment, ICodeWriter writer)
        {
            bool result = false;
            if (comment is CppCommentFull full && full.Children != null)
            {
                writer.WriteLine("/// <summary>");
                for (int i = 0; i < full.Children.Count; i++)
                {
                    WriteCsSummary(full.Children[i], writer);
                }
                writer.WriteLine("/// </summary>");
                result = true;
            }
            if (comment is CppCommentParagraph paragraph)
            {
                for (int i = 0; i < paragraph.Children.Count; i++)
                {
                    WriteCsSummary(paragraph.Children[i], writer);
                }
                result = true;
            }

            if (comment is CppCommentBlockCommand blockCommand)
            {
            }
            if (comment is CppCommentVerbatimBlockCommand verbatimBlockCommand)
            {
            }

            if (comment is CppCommentVerbatimBlockLine verbatimBlockLine)
            {
            }

            if (comment is CppCommentVerbatimLine line)
            {
            }

            if (comment is CppCommentParamCommand paramCommand)
            {
                // TODO: add param comment support
            }

            if (comment is CppCommentInlineCommand inlineCommand)
            {
                // TODO: add inline comment support
            }

            if (comment is CppCommentText text)
            {
                writer.WriteLine($"/// " + text.Text + "<br/>");
                result = true;
            }

            if (comment == null || comment.Kind == CppCommentKind.Null)
            {
            }

            if (!result && GeneratePlaceholderComments)
            {
                writer.WriteLine("/// <summary>");
                writer.WriteLine("/// To be documented.");
                writer.WriteLine("/// </summary>");
                return true;
            }

            return result;
        }

        public void WriteCsSummary(CppComment? cppComment, out string? comment)
        {
            comment = null;
            StringBuilder sb = new();
            if (cppComment is CppCommentFull full && full.Children != null)
            {
                sb.AppendLine("/// <summary>");
                for (int i = 0; i < full.Children.Count; i++)
                {
                    WriteCsSummary(full.Children[i], out var subComment);
                    sb.Append(subComment);
                }
                sb.AppendLine("/// </summary>");
                comment = sb.ToString();
                return;
            }
            if (cppComment is CppCommentParagraph paragraph)
            {
                for (int i = 0; i < paragraph.Children.Count; i++)
                {
                    WriteCsSummary(paragraph.Children[i], out var subComment);
                    sb.Append(subComment);
                }
                comment = sb.ToString();
                return;
            }
            if (cppComment is CppCommentText text)
            {
                sb.AppendLine($"/// " + text.Text + "<br/>");
                comment = sb.ToString();
                return;
            }

            if (cppComment is CppCommentBlockCommand blockCommand)
            {
                comment = null;
            }
            if (cppComment is CppCommentVerbatimBlockCommand verbatimBlockCommand)
            {
                comment = null;
            }

            if (cppComment is CppCommentVerbatimBlockLine verbatimBlockLine)
            {
                comment = null;
            }

            if (cppComment is CppCommentVerbatimLine line)
            {
                comment = null;
            }

            if (cppComment is CppCommentParamCommand paramCommand)
            {
                // TODO: add param comment support
                comment = null;
            }

            if (cppComment is CppCommentInlineCommand inlineCommand)
            {
                // TODO: add inline comment support
                comment = null;
            }

            if (cppComment == null || cppComment.Kind == CppCommentKind.Null)
            {
                comment = null;
            }

            if (comment == null && GeneratePlaceholderComments)
            {
                sb.AppendLine("/// <summary>");
                sb.AppendLine("/// To be documented.");
                sb.AppendLine("/// </summary>");
                comment = sb.ToString();
                return;
            }
        }

        public string? WriteCsSummary(CppComment? cppComment)
        {
            WriteCsSummary(cppComment, out var result);
            return result;
        }
    }
}