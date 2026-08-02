namespace HexaGen.Cpp2C
{
    using ClangSharp.Interop;
    using HexaGen.Core;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Interfaces;
    using HexaGen.CppAst.Model.Templates;
    using HexaGen.CppAst.Model.Types;
    using System;
    using System.Text;

    public partial class Cpp2CGeneratorConfig
    {
        public Dictionary<CppType, string> SubClasses { get; } = [];

        public string GetCType(CppType type, string? varName, CppClass? parent = null, bool globalNamespace = false)
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
                else if (currentType is CppQualifiedType qualified)
                {
                    currentType = qualified.ElementType;
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
                else if (currentType is CppUnexposedType)
                {
                    break;
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            StringBuilder sb = new();

            StringBuilder sbArray = new();
            for (int i = chain.Count - 1; i >= 0; i--)
            {
                var t = chain[i];
                if (t is CppPointerType)
                {
                    sb.Append('*');
                }
                else if (t is CppArrayType arrayType)
                {
                    long size = arrayType.Size;
                    if (size == -1)
                    {
                        var sizeExpr = arrayType.Type.SizeExpr;
                        size = EvaluateSize(sizeExpr, parent);
                    }

                    sbArray.Append('[');
                    sbArray.Append(size);
                    sbArray.Append(']');
                }
                else if (t is CppReferenceType)
                {
                    sb.Append('*');
                }
                else if (t is CppQualifiedType qualifiedType)
                {
                    switch (qualifiedType.Qualifier)
                    {
                        case CppTypeQualifier.Const:
                            sb.Append(" const");
                            break;

                        case CppTypeQualifier.Volatile:
                            sb.Append(" volatile");
                            break;
                    }
                }
                else if (t is CppPrimitiveType primitiveType)
                {
                    sb.Append(GetPrimitiveName(primitiveType));
                }
                else if (t is CppTypedef typedefType)
                {
                    if (typedefType.Parent is ICppGlobalDeclarationContainer)
                    {
                        if (globalNamespace)
                        {
                            sb.Append("::");
                        }
                        sb.Append(typedefType.Name);
                    }
                    else
                    {
                        chain[i] = typedefType.ElementType;
                        i++;
                    }
                }
                else if (t is CppClass classType)
                {
                    if (globalNamespace)
                    {
                        sb.Append("::");
                    }
                    sb.Append(GetCTypeName(classType));
                }
                else if (t is CppEnum enumType)
                {
                    if (globalNamespace)
                    {
                        sb.Append("::");
                    }
                    sb.Append(enumType.Name);
                    sb.Append("_t");
                }
                else if (t is CppUnexposedType unexposedType)
                {
                    if (parent != null)
                    {
                        bool found = false;
                        var name = unexposedType.Name;
                        for (int j = 0; j < parent.TemplateParameters.Count; j++)
                        {
                            var tempParam = parent.TemplateParameters[j];
                            if (tempParam is CppTemplateParameterType parameterType && parameterType.Name == name)
                            {
                                var tempType = parent.TemplateSpecializedArguments[j].ArgAsType!;

                                if (globalNamespace)
                                {
                                    sb.Append("::");
                                }
                                sb.Append(GetCType(tempType, null));
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            throw new NotSupportedException();
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            if (varName != null)
            {
                if (sb[^1] != ' ')
                {
                    sb.Append(' ');
                }
                sb.Append(varName);
                sb.Append(sbArray);
            }

            return sb.ToString();
        }

        public struct BinOp
        {
            public CXCursor Left;
            public CXCursor Right;
        }

        private unsafe static long EvaluateSize(CXCursor sizeExpr, CppClass parent)
        {
            Stack<(CXCursor cursor, bool closing)> operators = new();
            Stack<long> operands = new();

            operators.Push((sizeExpr, false));

            while (operators.TryPop(out var pair))
            {
                var (cursor, closing) = pair;

                var eval = cursor.Evaluate;
                if (eval.Kind != CXEvalResultKind.CXEval_UnExposed)
                {
                    operands.Push(eval.AsLongLong);
                    continue;
                }

                switch (cursor.Kind)
                {
                    case CXCursorKind.CXCursor_BinaryOperator:
                        {
                            if (closing)
                            {
                                var right = operands.Pop();
                                var left = operands.Pop();

                                long subResult = cursor.BinaryOperatorKind switch
                                {
                                    CXBinaryOperatorKind.CXBinaryOperator_Mul => left * right,
                                    CXBinaryOperatorKind.CXBinaryOperator_Div => left / right,
                                    CXBinaryOperatorKind.CXBinaryOperator_Rem => left % right,
                                    CXBinaryOperatorKind.CXBinaryOperator_Add => left + right,
                                    CXBinaryOperatorKind.CXBinaryOperator_Sub => left - right,
                                    CXBinaryOperatorKind.CXBinaryOperator_Shl => left << (int)right,
                                    CXBinaryOperatorKind.CXBinaryOperator_Shr => left >> (int)right,
                                    _ => throw new NotSupportedException(),
                                };

                                operands.Push(subResult);
                            }
                            else
                            {
                                BinOp op = default;
                                cursor.VisitChildren(static (cursor, parent, user) =>
                                {
                                    var pOp = (BinOp*)user;
                                    if (pOp->Left == default)
                                    {
                                        pOp->Left = cursor;
                                    }
                                    else
                                    {
                                        pOp->Right = cursor;
                                    }
                                    return CXChildVisitResult.CXChildVisit_Continue;
                                }, new((nint)(void*)&op));

                                operators.Push((cursor, true));
                                operators.Push((op.Right, false));
                                operators.Push((op.Left, false));
                            }

                            break;
                        }

                    case CXCursorKind.CXCursor_UnaryOperator:
                        {
                            if (closing)
                            {
                                var operand = operands.Pop();
                                long subResult = cursor.UnaryOperatorKind switch
                                {
                                    CXUnaryOperatorKind.CXUnaryOperator_Not => ~operand,
                                    _ => throw new NotSupportedException(),
                                };

                                operands.Push(subResult);
                            }
                            else
                            {
                                CXCursor next = default;
                                cursor.VisitChildren(static (cursor, parent, user) =>
                                {
                                    var pNext = (CXCursor*)user;
                                    *pNext = cursor;
                                    return CXChildVisitResult.CXChildVisit_Continue;
                                }, new((nint)(void*)&next));

                                operators.Push((cursor, true));
                                operators.Push((next, false));
                            }
                        }
                        break;

                    case CXCursorKind.CXCursor_DeclRefExpr:
                        {
                            var referenced = cursor.Referenced;
                            if (referenced.Kind == CXCursorKind.CXCursor_NonTypeTemplateParameter)
                            {
                                var index = referenced.TemplateTypeParmIndex;
                                var param = parent.TemplateSpecializedArguments[index];
                                operands.Push(param.ArgAsInteger);
                            }
                            else
                            {
                                throw new NotSupportedException();
                            }

                            break;
                        }

                    default:
                        throw new NotSupportedException();
                }
            }

            return operands.Pop();
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

        public List<TemplateDissolver> TemplateDissolvers { get; } = [];

        public TemplateDissolver? FindTemplateDissolver(CppClass c)
        {
            foreach (var dissolver in TemplateDissolvers)
            {
                if (dissolver.CanDissolve(this, c))
                {
                    return dissolver;
                }
            }
            return null;
        }

        public string GetCTypeName(CppClass c)
        {
            if (SubClasses.TryGetValue(c, out var subName))
            {
                return subName;
            }
            if (c.TemplateKind == CppTemplateKind.TemplateSpecializedClass)
            {
                foreach (var dissolver in TemplateDissolvers)
                {
                    if (dissolver.CanDissolve(this, c))
                    {
                        var cType = dissolver.GetDissolvedType(this, c);
                        return cType.Name;
                    }
                }

                StringBuilder sb = new();
                sb.Append(c.SpecializedTemplate.Name);
                sb.Append('_');
                foreach (var arg in c.TemplateSpecializedArguments)
                {
                    if (arg.ArgKind == CppTemplateArgumentKind.AsType)
                    {
                        var type = arg.ArgAsType;
                        if (type is CppClass cppClass)
                        {
                            sb.Append(cppClass.Name);
                        }
                        else
                        {
                            sb.Append(arg.ArgString);
                        }
                    }
                    else
                    {
                        sb.Append(arg.ArgString);
                    }
                }
                sb.Replace(':', '_');
                sb.Replace("*", "ptr");
                sb.Replace(' ', '_');
                return sb.ToString();
            }
            return $"{NamePrefix}{c.Name}";
        }
    }

    public class CType
    {
        public string Name;

        public CType(string name)
        {
            Name = name;
        }
    }

    public abstract class TemplateDissolver
    {
        public abstract bool CanDissolve(Cpp2CGeneratorConfig config, CppClass type);

        public abstract CType GetDissolvedType(Cpp2CGeneratorConfig config, CppClass type);

        public abstract string WriteDissolveFunc(Cpp2CGeneratorConfig config, ICodeWriter writer, CppClass cppType, CType cType);
    }
}