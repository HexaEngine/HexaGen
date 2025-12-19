namespace HexaGen.Conversion
{
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Interfaces;
    using HexaGen.CppAst.Model.Types;
    using System.Text;

    public enum CsTypeStyle
    {
        Raw,
        Ref,
        Wrapped,
    }

    public class CppTypeConverter
    {
        private readonly CsCodeGeneratorConfig config;
        private readonly Dictionary<CppType, AnalysisResult> typedefCache = [];
        private readonly Lock syncObj = new();
        private Dictionary<string, CppEnum> typeDefToEnum = [];

        public CppTypeConverter(CsCodeGeneratorConfig config)
        {
            this.config = config;
        }

        public void Initialize(ParseResult result)
        {
            typedefCache.Clear();

            var compilation = result.Compilation;
            typeDefToEnum = compilation.Enums.ToDictionary(e => PreprocessEnumName(e.Name));

            var enumMap = compilation.Enums.ToDictionary(e => e.Name);
            foreach (var pair in config.TypedefToEnumMappings)
            {
                if (pair.Value == null)
                {
                    typeDefToEnum.Remove(pair.Key);
                }
                else if (enumMap.TryGetValue(pair.Value, out var cppEnum))
                {
                    typeDefToEnum[pair.Key] = cppEnum;
                }
            }
        }

        private static string PreprocessEnumName(ReadOnlySpan<char> name)
        {
            if (name.EndsWith("_t"))
            {
                name = name[..^2];
            }
            else if (name.EndsWith('_'))
            {
                name = name[..^1];
            }
            return name.ToString();
        }

        private struct AnalysisResult
        {
            public string BaseType;
            public int PointerLevel;
            public bool IsConst;
            public CppFunctionType? Function;

            public readonly bool IsFunctionPointer => PointerLevel == 1 && Function != null;

            public void Merge(in AnalysisResult result)
            {
                BaseType = result.BaseType;
                PointerLevel += result.PointerLevel;
                IsConst |= result.IsConst && PointerLevel == 1;
                Function = result.Function;
            }
        }

        public string Convert(CppType type, CsTypeStyle style)
        {
            var result = AnalyzeType(type);
            return Format(result, style);
        }

        private string Format(in AnalysisResult result, CsTypeStyle style)
        {
            return style switch
            {
                CsTypeStyle.Raw => FormatRaw(result),
                CsTypeStyle.Ref => FormatRef(result),
                CsTypeStyle.Wrapped => FormatWrapped(result),
                _ => throw new NotSupportedException(),
            };
        }

        private string FormatRaw(AnalysisResult result)
        {
            if (result.Function != null)
            {
                result.BaseType = config.MakeDelegatePointer(result.Function);
                --result.PointerLevel;
            }
            return result.BaseType + new string('*', result.PointerLevel);
        }

        private string FormatRef(AnalysisResult result)
        {
            if (result.BaseType == "void" && result.PointerLevel > 0)
            {
                result.BaseType = "nint";
                --result.PointerLevel;
            }

            if (result.Function != null)
            {
                if (result.PointerLevel > 1 || string.IsNullOrWhiteSpace(result.BaseType))
                {
                    return FormatRaw(result);
                }
                --result.PointerLevel;
            }

            StringBuilder sb = new();
            if (result.PointerLevel > 0)
            {
                sb.Append(result.IsConst ? "in " : "ref ");
                --result.PointerLevel;
            }

            sb.Append(result.BaseType);
            while (result.PointerLevel-- != 0)
            {
                sb.Append('*');
            }

            return sb.ToString();
        }

        private string FormatWrapped(AnalysisResult result)
        {
            if (result.BaseType == "void" && result.PointerLevel > 0)
            {
                result.BaseType = "nint";
                --result.PointerLevel;
            }

            if (result.Function != null)
            {
                if (result.PointerLevel > 1 || string.IsNullOrWhiteSpace(result.BaseType))
                {
                    return FormatRaw(result);
                }
                --result.PointerLevel;
            }

            StringBuilder sb = new();
            for (int i = 0; i < result.PointerLevel; ++i)
            {
                sb.Append("Pointer<");
            }
            sb.Append(result.BaseType);
            for (int i = 0; i < result.PointerLevel; ++i)
            {
                sb.Append('>');
            }

            return sb.ToString();
        }

        private AnalysisResult AnalyzeType(CppType type)
        {
            AnalysisResult result = new();
            CppType? currentType = type;
            while (currentType != null)
            {
                if (currentType is CppPointerType pointerType)
                {
                    ++result.PointerLevel;
                    currentType = pointerType.ElementType;
                }
                else if (currentType is CppReferenceType referenceType)
                {
                    ++result.PointerLevel;
                    currentType = referenceType.ElementType;
                }
                else if (currentType is CppQualifiedType qualifiedType)
                {
                    result.IsConst |= qualifiedType.Qualifier == CppTypeQualifier.Const && result.PointerLevel == 1;
                    currentType = qualifiedType.ElementType;
                }
                else if (currentType is CppPrimitiveType primitiveType)
                {
                    result.BaseType = ConvertPrimitiveType(primitiveType);
                    break;
                }
                else if (currentType is CppTypedef typedef)
                {
                    result.Merge(ResolveTypedef(typedef));
                    break;
                }
                else if (currentType is CppEnum cppEnum)
                {
                    result.BaseType = GetMapping(cppEnum);
                    break;
                }
                else if (currentType is CppClass cppClass)
                {
                    result.BaseType = GetMapping(cppClass);
                    break;
                }
                else if (currentType is CppArrayType arrayType)
                {
                    if (arrayType.Size > 0 && config.TryGetArrayMapping(arrayType, out string? mapping))
                    {
                        result.BaseType = mapping;
                        break;
                    }

                    ++result.PointerLevel;
                    currentType = arrayType.ElementType;
                }
                else if (currentType is CppFunctionType functionType)
                {
                    result.Function = functionType;
                    break;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return result;
        }

        private AnalysisResult ResolveTypedef(CppTypedef typedef)
        {
            if (typeDefToEnum.TryGetValue(typedef.Name, out var cppEnum))
            {
                return new() { BaseType = GetMapping(cppEnum) };
            }
            if (config.TypeMappings.TryGetValue(typedef.Name, out var name))
            {
                return new() { BaseType = name };
            }
            lock (syncObj)
            {
                if (!typedefCache.TryGetValue(typedef, out var result))
                {
                    result = AnalyzeType(typedef.ElementType);
                    if (result.IsFunctionPointer)
                    {
                        result.BaseType = GetMapping(typedef);
                    }
                    typedefCache.Add(typedef, result);
                }
                return result;
            }
        }

        private string GetMapping<T>(T member) where T : ICppMember
        {
            return config.GetCsCleanName(member.Name);
        }

        private string ConvertPrimitiveType(CppPrimitiveType primitiveType)
        {
            return primitiveType.Kind switch
            {
                CppPrimitiveKind.Void => "void",
                CppPrimitiveKind.Char => "byte",
                CppPrimitiveKind.Bool => "bool",
                CppPrimitiveKind.WChar => "char",
                CppPrimitiveKind.Short => "short",
                CppPrimitiveKind.Int => "int",
                CppPrimitiveKind.Long => "int",
                CppPrimitiveKind.UnsignedLong => "uint",
                CppPrimitiveKind.LongLong => "long",
                CppPrimitiveKind.UnsignedChar => "byte",
                CppPrimitiveKind.UnsignedShort => "ushort",
                CppPrimitiveKind.UnsignedInt => "uint",
                CppPrimitiveKind.UnsignedLongLong => "ulong",
                CppPrimitiveKind.Float => "float",
                CppPrimitiveKind.Double => "double",
                CppPrimitiveKind.LongDouble => "double",
                _ => string.Empty,
            };
        }
    }
}