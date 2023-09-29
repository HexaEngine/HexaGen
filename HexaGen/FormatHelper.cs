namespace HexaGen
{
    using CppAst;
    using System;
    using System.Text;

    public static class FormatHelper
    {
        public static bool IsCaps(this string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (char.IsSymbol(c))
                    continue;
                if (char.IsLower(c))
                    return false;
            }
            return true;
        }

        public static bool IsPointer(this CppType type)
        {
            if (type is CppPointerType)
            {
                return true;
            }

            if (type is CppQualifiedType qualifiedType)
            {
                return IsPointer(qualifiedType.ElementType);
            }

            return false;
        }

        public static bool IsPointer(this CppType type, ref int depth)
        {
            bool isPointer = false;
            CppType d = type;
            depth = 0;
            while (true)
            {
                if (d is CppPointerType pointer)
                {
                    depth++;
                    d = pointer.ElementType;
                    isPointer = true;
                }
                else
                {
                    break;
                }
            }

            return isPointer;
        }

        public static bool IsPointer(this CppType type, ref int depth, out CppType pointerType)
        {
            bool isPointer = false;
            CppType d = type;
            depth = 0;
            while (true)
            {
                if (d is CppPointerType pointer)
                {
                    depth++;
                    d = pointer.ElementType;
                    isPointer = true;
                }
                else
                {
                    break;
                }
            }
            pointerType = d;
            return isPointer;
        }

        public static bool IsPointerOf(this CppType type, CppType pointer)
        {
            if (pointer is CppPointerType pointerType)
            {
                return pointerType.ElementType.GetDisplayName() == type.GetDisplayName();
            }
            return false;
        }

        public static bool IsPointerOf(this CppType type, CppType pointer, ref int depth)
        {
            if (pointer is CppPointerType pointerType)
            {
                if (pointerType.ElementType is CppPointerType cppPointer)
                {
                    depth++;
                    return IsPointerOf(type, cppPointer, ref depth);
                }
                depth++;
                if (pointerType.ElementType is CppQualifiedType qualifiedType && qualifiedType.Qualifier == CppTypeQualifier.Const)
                    return qualifiedType.ElementType.GetDisplayName() == type.GetDisplayName();
                else
                    return pointerType.ElementType.GetDisplayName() == type.GetDisplayName();
            }
            return false;
        }

        public static bool IsType(this CppType a, CppType b)
        {
            return a.GetDisplayName() == b.GetDisplayName();
        }

        public static bool IsPrimitive(this CppType cppType, out CppPrimitiveType primitive)
        {
            if (cppType is CppPrimitiveType cppPrimitive)
            {
                primitive = cppPrimitive;
                return true;
            }

            if (cppType is CppTypedef cppTypedef)
            {
                return IsPrimitive(cppTypedef.ElementType, out primitive);
            }

            if (cppType is CppPointerType cppPointerType)
            {
                return IsPrimitive(cppPointerType.ElementType, out primitive);
            }

            primitive = null;

            return false;
        }

        public static bool IsUsedAsPointer(this CppClass cppClass, CppCompilation compilation, out List<int> depths)
        {
            depths = new List<int>();
            int depth = 0;
            for (int i = 0; i < compilation.Functions.Count; i++)
            {
                depth = 0;
                var func = compilation.Functions[i];
                if (IsPointerOf(cppClass, func.ReturnType, ref depth))
                {
                    if (!depths.Contains(depth))
                        depths.Add(depth);
                }

                for (int j = 0; j < func.Parameters.Count; j++)
                {
                    depth = 0;
                    var param = func.Parameters[j];
                    if (IsPointerOf(cppClass, param.Type, ref depth))
                    {
                        if (!depths.Contains(depth))
                            depths.Add(depth);
                    }
                }
            }

            for (int i = 0; i < compilation.Classes.Count; i++)
            {
                var cl = compilation.Classes[i];
                for (int j = 0; j < cl.Fields.Count; j++)
                {
                    depth = 0;
                    var field = cl.Fields[j];
                    if (IsPointerOf(cppClass, field.Type, ref depth))
                    {
                        if (!depths.Contains(depth))
                            depths.Add(depth);
                    }
                }
            }

            return depths.Count > 0;
        }

        public static string NormalizeEnumValue(this string value)
        {
            if (value == "(~0U)")
            {
                return "~0u";
            }

            if (value == "(~0ULL)")
            {
                return "~0ul";
            }

            if (value == "(~0U-1)")
            {
                return "~0u - 1";
            }

            if (value == "(~0U-2)")
            {
                return "~0u - 2";
            }

            if (value == "(~0U-3)")
            {
                return "~0u - 3";
            }

            return value.Replace("ULL", "UL");
        }

        public static string NormalizeConstantValue(this string value)
        {
            if (value == "(~0U)")
            {
                return "~0u";
            }

            if (value == "(~0ULL)")
            {
                return "~0ul";
            }

            if (value == "(~0U-1)")
            {
                return "~0u - 1";
            }

            if (value == "(~0U-2)")
            {
                return "~0u - 2";
            }

            if (value == "(~0U-3)")
            {
                return "~0u - 3";
            }

            if (value.StartsWith("L\"") && value.StartsWith("R\"") && value.StartsWith("LR\"") && value.EndsWith("\"") && value.Count(c => c == '"') > 2)
            {
                string[] parts = value.Split('"', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                StringBuilder sb = new();
                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];
                    if (part == "L" || part == "R" || part == "LR")
                        continue;
                    sb.Append(part);
                }
                return $"@\"{sb}\"";
            }
            else
            {
                if (value.StartsWith("L\"") && value.EndsWith("\""))
                {
                    return value[1..];
                }

                if (value.StartsWith("R\"") && value.EndsWith("\""))
                {
                    return $"@{value[1..]}";
                }

                if (value.StartsWith("LR\"") && value.EndsWith("\""))
                {
                    var lines = value[3..^1].Split("\n");
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i] = lines[i].TrimEnd('\r');
                    }
                    return $"@\"{string.Join("\n", lines)}\"";
                }
            }

            return value.Replace("ULL", "UL");
        }

        public static bool IsNumeric(this string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            for (int i = 0; i < name.Length; i++)
            {
                if (!char.IsNumber(name[i]))
                    return false;
            }
            return true;
        }

        public static bool IsNumeric(this string name, out NumberType numberType, bool allowBrackets = true, bool allowHex = true, bool allowMinus = true, bool allowExponent = true, bool allowSuffix = true)
        {
            numberType = NumberType.None;
            if (string.IsNullOrEmpty(name))
                return false;
            int index = 0;
            int length = name.Length;
            bool isHex = false;
            bool isMinus = false;
            bool typeOverwrite = false;

            numberType = NumberType.Int;

            if (allowBrackets && name.StartsWith("(") && name.EndsWith(")"))
            {
                index += 1;
                length -= 1;
            }

            if (allowMinus && name[index..length].StartsWith('-'))
            {
                numberType = NumberType.Int;
                index += 1;
                isMinus = true;
            }

            if (allowHex && name[index..length].StartsWith("0x"))
            {
                index += 2;
                isHex = true;
            }

            if (allowSuffix && name[index..length].EndsWith("UL", StringComparison.InvariantCultureIgnoreCase))
            {
                numberType = NumberType.ULong;
                length -= 2;
                typeOverwrite = true;
            }

            if (allowSuffix && name[index..length].EndsWith("L", StringComparison.InvariantCultureIgnoreCase))
            {
                numberType = NumberType.Long;
                length -= 1;
                typeOverwrite = true;
            }

            if (allowSuffix && name[index..length].EndsWith("U", StringComparison.InvariantCultureIgnoreCase))
            {
                numberType = NumberType.UInt;
                length -= 1;
                typeOverwrite = true;
            }

            if (allowSuffix && !isHex && name[index..length].EndsWith("F", StringComparison.InvariantCultureIgnoreCase))
            {
                numberType = NumberType.Float;
                length -= 1;
                typeOverwrite = true;
            }

            if (allowSuffix && !isHex && name[index..length].EndsWith("D", StringComparison.InvariantCultureIgnoreCase))
            {
                numberType = NumberType.Double;
                length -= 1;
                typeOverwrite = true;
            }

            if (allowSuffix && name[index..length].EndsWith("M", StringComparison.InvariantCultureIgnoreCase))
            {
                numberType = NumberType.Decimal;
                length -= 1;
                typeOverwrite = true;
            }

            if (allowExponent && name.Contains("E-", StringComparison.InvariantCultureIgnoreCase))
            {
                var idx = name.IndexOf("E-", StringComparison.InvariantCultureIgnoreCase);
                for (int i = idx + 2; i < length; i++)
                {
                    if (!char.IsDigit(name[i]))
                    {
                        return false;
                    }
                }
                length = idx;
            }

            if (allowExponent && name.Contains("E+", StringComparison.InvariantCultureIgnoreCase))
            {
                var idx = name.IndexOf("E+", StringComparison.InvariantCultureIgnoreCase);
                for (int i = idx + 2; i < length; i++)
                {
                    if (!char.IsDigit(name[i]))
                    {
                        return false;
                    }
                }
                length = idx;
            }

            for (int i = index; i < length; i++)
            {
                var c = name[i];
                if (!char.IsNumber(c))
                {
                    if (c == '.')
                    {
                        if ((numberType & NumberType.AnyInt) != 0)
                        {
                            numberType = NumberType.Double;
                        }
                    }
                    else if (!isHex || !char.IsAsciiHexDigit(c))
                    {
                        return false;
                    }
                }
            }

            if (typeOverwrite)
            {
                return true;
            }

            if ((numberType & NumberType.AnyInt) != 0 && !isHex)
            {
                var span = name.AsSpan(index, length - index).TrimStart('0');
                if (NumberStringCompareLessEquals(span, int.MaxValue.ToString()))
                {
                    numberType = NumberType.Int;
                    return true;
                }
                if (!isMinus && NumberStringCompareLessEquals(span, uint.MaxValue.ToString()))
                {
                    numberType = NumberType.UInt;
                    return true;
                }
                if (NumberStringCompareLessEquals(span, long.MaxValue.ToString()))
                {
                    numberType = NumberType.Long;
                    return true;
                }
                if (!isMinus && NumberStringCompareLessEquals(span, ulong.MaxValue.ToString()))
                {
                    numberType = NumberType.ULong;
                    return true;
                }

                throw new InvalidDataException($"The number {name} is outside known number ranges!");
            }
            if ((numberType & NumberType.AnyInt) != 0 && isHex)
            {
                var span = name.AsSpan(index, length - index).TrimStart('0');
                if (NumberStringCompareLessEquals(span, "7fffffff"))
                {
                    numberType = NumberType.Int;
                    return true;
                }
                if (!isMinus && NumberStringCompareLessEquals(span, "ffffffff"))
                {
                    numberType = NumberType.UInt;
                    return true;
                }
                if (NumberStringCompareLessEquals(span, "7fffffffffffffff"))
                {
                    numberType = NumberType.Long;
                    return true;
                }
                if (!isMinus && NumberStringCompareLessEquals(span, "ffffffffffffffff"))
                {
                    numberType = NumberType.ULong;
                    return true;
                }

                throw new InvalidDataException($"The number {name} is outside known number ranges!");
            }

            return true;
        }

        public static bool IsNumeric(this string name, bool allowHex = true, bool allowMinus = true, bool allowExponent = true, bool allowSuffix = true)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            int index = 0;
            int length = name.Length;
            bool isHex = false;
            if (allowHex && name.StartsWith("0x"))
            {
                index = 2;
                isHex = true;
            }

            if (allowMinus && name.StartsWith('-'))
            {
                index = 1;
            }

            if (allowSuffix && name.EndsWith("L", StringComparison.InvariantCultureIgnoreCase))
            {
                length = name.Length - 1;
            }

            if (allowSuffix && name.EndsWith("U", StringComparison.InvariantCultureIgnoreCase))
            {
                length = name.Length - 1;
            }

            if (allowSuffix && name.EndsWith("UL", StringComparison.InvariantCultureIgnoreCase))
            {
                length = name.Length - 2;
            }

            if (allowSuffix && name.EndsWith("F", StringComparison.InvariantCultureIgnoreCase))
            {
                length = name.Length - 1;
            }

            if (allowSuffix && name.EndsWith("D", StringComparison.InvariantCultureIgnoreCase))
            {
                length = name.Length - 1;
            }

            if (allowSuffix && name.EndsWith("M", StringComparison.InvariantCultureIgnoreCase))
            {
                length = name.Length - 1;
            }

            if (allowExponent && name.Contains("E-", StringComparison.InvariantCultureIgnoreCase))
            {
                var idx = name.IndexOf("E-", StringComparison.InvariantCultureIgnoreCase);
                for (int i = idx + 2; i < length; i++)
                {
                    if (!char.IsDigit(name[i]))
                    {
                        return false;
                    }
                }
                length = idx;
            }

            if (allowExponent && name.Contains("E+", StringComparison.InvariantCultureIgnoreCase))
            {
                var idx = name.IndexOf("E+", StringComparison.InvariantCultureIgnoreCase);
                for (int i = idx + 2; i < length; i++)
                {
                    if (!char.IsDigit(name[i]))
                    {
                        return false;
                    }
                }
                length = idx;
            }

            for (int i = index; i < length; i++)
            {
                var c = name[i];
                if ((!isHex || !char.IsAsciiHexDigit(c)) && !char.IsNumber(c) && c != '.')
                {
                    return false;
                }
            }

            return true;
        }

        public static bool NumberStringCompareLess(ReadOnlySpan<char> value, string max)
        {
            // Greater
            if (value.Length > max.Length)
                return false;

            // Less
            if (value.Length < max.Length)
                return true;

            for (int i = 0; i < value.Length; i++)
            {
                var c = char.ToLower(value[i]);
                var cmp = char.ToLower(max[i]);

                // Greater
                if (cmp < c)
                {
                    return false;
                }

                // Less
                if (cmp > c)
                {
                    return true;
                }
            }

            // Equals
            return false;
        }

        public static bool NumberStringCompareLessEquals(ReadOnlySpan<char> value, string max)
        {
            // Greater
            if (value.Length > max.Length)
                return false;

            // Less
            if (value.Length < max.Length)
                return true;

            for (int i = 0; i < value.Length; i++)
            {
                var c = char.ToLower(value[i]);
                var cmp = char.ToLower(max[i]);

                // Greater
                if (cmp < c)
                {
                    return false;
                }

                // Less
                if (cmp > c)
                {
                    return true;
                }
            }

            // Equals
            return true;
        }

        public static bool IsConstantExpression(this string expression)
        {
            for (int i = 0; i < expression.Length; i++)
            {
                var c = expression[i];
                if (char.IsLetter(c))
                {
                    continue;
                }
                if (char.IsNumber(c) || c == '.')
                {
                    continue;
                }
                if (c == '+' || c == '-' || c == '*' || c == '/' || c == '<' || c == '>')
                {
                    continue;
                }

                return false;
            }

            return true;
        }

        public static bool IsString(this string name)
        {
            return name.StartsWith("\"") && name.EndsWith("\"");
        }

        public static bool IsVoid(this CppType cppType)
        {
            if (cppType is CppPrimitiveType type)
            {
                return type.Kind == CppPrimitiveKind.Void;
            }
            return false;
        }

        public static bool IsString(this CppType cppType, bool isPointer = false)
        {
            if (cppType is CppPointerType pointer && !isPointer)
            {
                return IsString(pointer.ElementType, true);
            }

            if (cppType is CppQualifiedType qualified)
            {
                return IsString(qualified.ElementType, isPointer);
            }

            if (isPointer && cppType is CppPrimitiveType primitive)
            {
                return primitive.Kind == CppPrimitiveKind.WChar || primitive.Kind == CppPrimitiveKind.Char;
            }

            return false;
        }

        public static bool IsTemplateParameter(this CppType type, CppFunction function)
        {
            if (type is CppPointerType pointer)
            {
                return IsTemplateParameter(pointer.ElementType, function);
            }

            if (type is CppReferenceType reference)
            {
                return IsTemplateParameter(reference.ElementType, function);
            }

            if (type is CppArrayType array)
            {
                return IsTemplateParameter(array.ElementType, function);
            }

            if (type is CppQualifiedType qualified)
            {
                return IsTemplateParameter(qualified.ElementType, function);
            }

            if (type is CppUnexposedType unexposed)
            {
                for (int i = 0; i < function.TemplateParameters.Count; i++)
                {
                    if (function.TemplateParameters[i] == unexposed)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static string GetTemplateParameterCsName(this CppType type, string genericName)
        {
            if (type is CppPointerType pointer)
            {
                return GetTemplateParameterCsName(pointer.ElementType, genericName) + "*";
            }

            if (type is CppReferenceType reference)
            {
                return GetTemplateParameterCsName(reference.ElementType, genericName) + "*";
            }

            if (type is CppArrayType array)
            {
                return GetTemplateParameterCsName(array.ElementType, genericName) + "*";
            }

            if (type is CppQualifiedType qualified)
            {
                return GetTemplateParameterCsName(qualified.ElementType, genericName);
            }

            if (type is CppUnexposedType)
            {
                return genericName;
            }

            return string.Empty;
        }

        public static CppPrimitiveKind GetPrimitiveKind(this CppType cppType, bool isPointer = false)
        {
            if (cppType is CppArrayType arrayType)
            {
                return GetPrimitiveKind(arrayType.ElementType, true);
            }

            if (cppType is CppPointerType pointer)
            {
                return GetPrimitiveKind(pointer.ElementType, true);
            }

            if (cppType is CppQualifiedType qualified)
            {
                return GetPrimitiveKind(qualified.ElementType, isPointer);
            }

            if (isPointer && cppType is CppPrimitiveType primitive)
            {
                return primitive.Kind;
            }

            return CppPrimitiveKind.Void;
        }

        public static string GetNumberType(this NumberType number)
        {
            return number switch
            {
                NumberType.None => throw new InvalidOperationException(),
                NumberType.Int => "int",
                NumberType.Double => "double",
                NumberType.Float => "float",
                NumberType.Decimal => "decimal",
                NumberType.UInt => "uint",
                NumberType.Long => "long",
                NumberType.ULong => "ulong",
                _ => throw new InvalidOperationException(),
            };
        }
    }
}