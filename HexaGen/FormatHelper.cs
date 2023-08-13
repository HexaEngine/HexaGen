namespace HexaGen
{
    using CppAst;
    using System;
    using System.Text;
    using System.Xml.Linq;

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

        public static bool IsNumeric(this string name, out CsNumberType numberType, bool allowHex = true, bool allowTail = true)
        {
            numberType = CsNumberType.None;
            if (string.IsNullOrEmpty(name))
                return false;
            int index = 0;
            int length = name.Length;

            if (allowHex && name.StartsWith("0x"))
            {
                index = 2;
            }

            numberType = CsNumberType.Int;

            if (allowTail && name.EndsWith("L", StringComparison.InvariantCultureIgnoreCase))
            {
                numberType = CsNumberType.Long;
                length = name.Length - 1;
            }

            if (allowTail && name.EndsWith("U", StringComparison.InvariantCultureIgnoreCase))
            {
                numberType = CsNumberType.UInt;
                length = name.Length - 1;
            }

            if (allowTail && name.EndsWith("UL", StringComparison.InvariantCultureIgnoreCase))
            {
                numberType = CsNumberType.ULong;
                length = name.Length - 2;
            }

            if (allowTail && name.EndsWith("F", StringComparison.InvariantCultureIgnoreCase))
            {
                numberType = CsNumberType.Float;
                length = name.Length - 1;
            }

            if (allowTail && name.EndsWith("D", StringComparison.InvariantCultureIgnoreCase))
            {
                numberType = CsNumberType.Double;
                length = name.Length - 1;
            }

            if (allowTail && name.EndsWith("M", StringComparison.InvariantCultureIgnoreCase))
            {
                numberType = CsNumberType.Decimal;
                length = name.Length - 1;
            }

            for (int i = index; i < length; i++)
            {
                var c = name[i];
                if (!char.IsNumber(c))
                {
                    if (c == '.')
                    {
                        if (numberType == CsNumberType.Int)
                            numberType = CsNumberType.Double;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsNumeric(this string name, bool allowHex = true, bool allowTail = true)
        {
            if (string.IsNullOrEmpty(name))
                return false;
            int index = 0;
            int length = name.Length;

            if (allowHex && name.StartsWith("0x"))
            {
                index = 2;
            }

            if (allowTail && name.EndsWith("L", StringComparison.InvariantCultureIgnoreCase))
            {
                length = name.Length - 1;
            }

            if (allowTail && name.EndsWith("U", StringComparison.InvariantCultureIgnoreCase))
            {
                length = name.Length - 1;
            }

            if (allowTail && name.EndsWith("UL", StringComparison.InvariantCultureIgnoreCase))
            {
                length = name.Length - 2;
            }

            if (allowTail && name.EndsWith("F", StringComparison.InvariantCultureIgnoreCase))
            {
                length = name.Length - 1;
            }

            if (allowTail && name.EndsWith("D", StringComparison.InvariantCultureIgnoreCase))
            {
                length = name.Length - 1;
            }

            if (allowTail && name.EndsWith("M", StringComparison.InvariantCultureIgnoreCase))
            {
                length = name.Length - 1;
            }

            for (int i = index; i < length; i++)
            {
                var c = name[i];
                if (!char.IsNumber(c) && c != '.')
                {
                    return false;
                }
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

        public static bool WriteCsSummary(this string? comment, CodeWriter writer)
        {
            if (comment == null)
                return false;

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

        public static bool WriteCsSummary(this CppComment? comment, CodeWriter writer)
        {
            if (comment is CppCommentFull full)
            {
                writer.WriteLine("/// <summary>");
                for (int i = 0; i < full.Children.Count; i++)
                {
                    WriteCsSummary(full.Children[i], writer);
                }
                writer.WriteLine("/// </summary>");
                return true;
            }
            if (comment is CppCommentParagraph paragraph)
            {
                for (int i = 0; i < paragraph.Children.Count; i++)
                {
                    WriteCsSummary(paragraph.Children[i], writer);
                }
                return true;
            }

            if (comment is CppCommentBlockCommand blockCommand)
            {
                return false;
            }
            if (comment is CppCommentVerbatimBlockCommand verbatimBlockCommand)
            {
                return false;
            }

            if (comment is CppCommentVerbatimBlockLine verbatimBlockLine)
            {
                return false;
            }

            if (comment is CppCommentVerbatimLine line)
            {
                return false;
            }

            if (comment is CppCommentParamCommand paramCommand)
            {
                // TODO: add param comment support
                return false;
            }

            if (comment is CppCommentInlineCommand inlineCommand)
            {
                // TODO: add inline comment support
                return false;
            }

            if (comment is CppCommentText text)
            {
                writer.WriteLine($"/// " + text.Text + "<br/>");
                return true;
            }

            if (comment == null || comment.Kind == CppCommentKind.Null)
            {
                return false;
            }

            throw new NotSupportedException($"The comment type {comment.GetType()} is not supported");
        }

        public static void WriteCsSummary(this CppComment? cppComment, out string? comment)
        {
            StringBuilder sb = new();
            if (cppComment is CppCommentFull full)
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
                return;
            }
            if (cppComment is CppCommentVerbatimBlockCommand verbatimBlockCommand)
            {
                comment = null;
                return;
            }

            if (cppComment is CppCommentVerbatimBlockLine verbatimBlockLine)
            {
                comment = null;
                return;
            }

            if (cppComment is CppCommentVerbatimLine line)
            {
                comment = null;
                return;
            }

            if (cppComment is CppCommentParamCommand paramCommand)
            {
                // TODO: add param comment support
                comment = null;
                return;
            }

            if (cppComment is CppCommentInlineCommand inlineCommand)
            {
                // TODO: add inline comment support
                comment = null;
                return;
            }

            if (cppComment == null || cppComment.Kind == CppCommentKind.Null)
            {
                comment = null;
                return;
            }

            throw new NotSupportedException($"The comment type {cppComment.GetType()} is not supported");
        }

        public static string GetNumberType(this CsNumberType number)
        {
            return number switch
            {
                CsNumberType.None => throw new InvalidOperationException(),
                CsNumberType.Int => "int",
                CsNumberType.Double => "double",
                CsNumberType.Float => "float",
                CsNumberType.Decimal => "decimal",
                CsNumberType.UInt => "uint",
                CsNumberType.Long => "long",
                CsNumberType.ULong => "ulong",
                _ => throw new InvalidOperationException(),
            };
        }
    }
}