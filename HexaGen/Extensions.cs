namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class Extensions
    {
        public static CallingConvention GetCallingConvention(this CppCallingConvention convention)
        {
            return convention switch
            {
                CppCallingConvention.C => CallingConvention.Cdecl,
                CppCallingConvention.Win64 => CallingConvention.Winapi,
                CppCallingConvention.X86FastCall => CallingConvention.FastCall,
                CppCallingConvention.X86StdCall => CallingConvention.StdCall,
                CppCallingConvention.X86ThisCall => CallingConvention.ThisCall,
                _ => throw new NotSupportedException(),
            };
        }

        public static string GetCallingConventionDelegate(this CppCallingConvention convention)
        {
            return convention switch
            {
                CppCallingConvention.C => "Cdecl",
                CppCallingConvention.X86FastCall => "Fastcall",
                CppCallingConvention.X86StdCall => "Stdcall",
                CppCallingConvention.X86ThisCall => "Thiscall",
                _ => throw new NotSupportedException(),
            };
        }

        public static string GetCallingConventionLibrary(this CppCallingConvention convention)
        {
            return convention switch
            {
                CppCallingConvention.C => "System.Runtime.CompilerServices.CallConvCdecl",
                CppCallingConvention.X86FastCall => "System.Runtime.CompilerServices.CallConvFastcall",
                CppCallingConvention.X86StdCall => "System.Runtime.CompilerServices.CallConvStdcall",
                CppCallingConvention.X86ThisCall => "System.Runtime.CompilerServices.CallConvThiscall",
                _ => throw new NotSupportedException(),
            };
        }

        public static Direction GetDirection(this CppType type, bool isPointer = false)
        {
            if (type is CppPrimitiveType)
            {
                return isPointer ? Direction.InOut : Direction.In;
            }

            if (type is CppPointerType pointerType)
            {
                return GetDirection(pointerType.ElementType, true);
            }

            if (type is CppReferenceType)
            {
                return Direction.Out;
            }

            if (type is CppQualifiedType qualifiedType)
            {
                return qualifiedType.Qualifier != CppTypeQualifier.Const && isPointer ? Direction.InOut : Direction.In;
            }

            if (type is CppFunctionType)
            {
                return isPointer ? Direction.InOut : Direction.In;
            }

            if (type is CppTypedef)
            {
                return isPointer ? Direction.InOut : Direction.In;
            }

            if (type is CppClass)
            {
                return isPointer ? Direction.InOut : Direction.In;
            }

            if (type is CppEnum)
            {
                return isPointer ? Direction.InOut : Direction.In;
            }

            return isPointer ? Direction.InOut : Direction.In;
        }

        public static bool CanBeUsedAsOutput(this CppType type, out CppTypeDeclaration? elementTypeDeclaration)
        {
            if (type is CppPointerType pointerType)
            {
                if (pointerType.ElementType is CppTypedef typedef)
                {
                    elementTypeDeclaration = typedef;
                    return true;
                }
                else if (pointerType.ElementType is CppClass @class
                    && @class.ClassKind != CppClassKind.Class
                    && @class.SizeOf > 0)
                {
                    elementTypeDeclaration = @class;
                    return true;
                }
                else if (pointerType.ElementType is CppEnum @enum
                    && @enum.SizeOf > 0)
                {
                    elementTypeDeclaration = @enum;
                    return true;
                }
            }

            elementTypeDeclaration = null;
            return false;
        }

        public static bool IsCOMObject(this CppClass cppClass)
        {
            if (cppClass.Fields.Count == 0 && cppClass.Functions.Count > 0 && cppClass.IsAbstract)
                return true;
            return false;
        }

        public static bool IsClass(this CppType cppType, out CppClass cppClass)
        {
            if (cppType is CppPointerType pointerType)
            {
                return IsClass(pointerType.ElementType, out cppClass);
            }

            if (cppType is CppReferenceType referenceType)
            {
                return IsClass(referenceType.ElementType, out cppClass);
            }

            if (cppType is CppQualifiedType qualifiedType)
            {
                return IsClass(qualifiedType.ElementType, out cppClass);
            }

            if (cppType is CppClass cpp)
            {
                cppClass = cpp;
                return true;
            }

            cppClass = null;
            return false;
        }

        public static bool IsDelegate(this CppPointerType cppPointer, out CppFunctionType cppFunction)
        {
            if (cppPointer.ElementType is CppFunctionType functionType)
            {
                cppFunction = functionType;
                return true;
            }
            cppFunction = null;
            return false;
        }

        public static bool IsDelegate(this CppType cppType, out CppFunctionType cppFunction)
        {
            if (cppType is CppTypedef typedefType)
            {
                return IsDelegate(typedefType.ElementType, out cppFunction);
            }
            if (cppType is CppPointerType cppPointer)
            {
                return IsDelegate(cppPointer.ElementType, out cppFunction);
            }
            if (cppType is CppFunctionType functionType)
            {
                cppFunction = functionType;
                return true;
            }
            cppFunction = null;
            return false;
        }

        public static bool IsDelegate(this CppPointerType cppPointer)
        {
            if (cppPointer.ElementType is CppFunctionType)
            {
                return true;
            }

            return false;
        }

        public static bool IsDelegate(this CppType cppType)
        {
            if (cppType is CppTypedef typedefType)
            {
                return IsDelegate(typedefType.ElementType);
            }
            if (cppType is CppPointerType cppPointer)
            {
                return IsDelegate(cppPointer.ElementType);
            }
            if (cppType is CppFunctionType functionType)
            {
                return true;
            }

            return false;
        }

        public static bool IsEnum(this CppType cppType)
        {
            if (cppType is CppTypedef cppTypedef)
            {
                return IsEnum(cppTypedef.ElementType);
            }
            if (cppType is CppPointerType cppPointer)
            {
                return IsEnum(cppPointer.ElementType);
            }
            if (cppType is CppEnum cppEnum)
            {
                return true;
            }
            return false;
        }

        public static bool IsEnum(this CppType cppType, [NotNullWhen(true)] out CppEnum? cppEnum)
        {
            if (cppType is CppTypedef cppTypedef)
            {
                return IsEnum(cppTypedef.ElementType, out cppEnum);
            }
            if (cppType is CppPointerType cppPointer)
            {
                return IsEnum(cppPointer.ElementType, out cppEnum);
            }
            if (cppType is CppEnum cppEnum1)
            {
                cppEnum = cppEnum1;
                return true;
            }
            cppEnum = null;
            return false;
        }

        public static unsafe string ToCamelCase(this string str)
        {
            bool wasNumber = false;
            string output = new('\0', str.Length);
            fixed (char* p = output)
            {
                p[0] = char.ToUpper(str[0]);
                for (int i = 1; i < str.Length; i++)
                {
                    if (wasNumber)
                    {
                        p[i] = char.ToUpper(str[i]);
                    }
                    else
                    {
                        p[i] = char.ToLower(str[i]);
                    }
                    wasNumber = char.IsDigit(str[i]);
                }
            }
            return output;
        }

        private enum SplitByCaseModes
        { None, WhiteSpace, Digit, UpperCase, LowerCase }

        public static string[] SplitByCase(this string s)
        {
            var ʀ = new List<string>();
            var ᴛ = new StringBuilder();
            var previous = SplitByCaseModes.None;
            foreach (var ɪ in s)
            {
                SplitByCaseModes mode_ɪ;
                if (string.IsNullOrWhiteSpace(ɪ.ToString()))
                {
                    mode_ɪ = SplitByCaseModes.WhiteSpace;
                }
                else if (char.IsDigit(ɪ))
                {
                    mode_ɪ = SplitByCaseModes.Digit;
                }
                else if (ɪ == ɪ.ToString().ToUpper()[0])
                {
                    mode_ɪ = SplitByCaseModes.UpperCase;
                }
                else
                {
                    mode_ɪ = SplitByCaseModes.LowerCase;
                }
                if (previous == SplitByCaseModes.None || previous == mode_ɪ)
                {
                    ᴛ.Append(ɪ);
                }
                else if (previous == SplitByCaseModes.UpperCase && mode_ɪ == SplitByCaseModes.LowerCase)
                {
                    if (ᴛ.Length > 1)
                    {
                        ʀ.Add(ᴛ.ToString()[..(ᴛ.Length - 1)]);
                        ᴛ.Remove(0, ᴛ.Length - 1);
                    }
                    ᴛ.Append(ɪ);
                }
                else
                {
                    ʀ.Add(ᴛ.ToString());
                    ᴛ.Clear();
                    ᴛ.Append(ɪ);
                }
                previous = mode_ɪ;
            }
            if (ᴛ.Length != 0) ʀ.Add(ᴛ.ToString());
            return ʀ.ToArray();
        }

        public static CppMacro? FindMacro(this CppCompilation compilation, string name)
        {
            for (int i = 0; i < compilation.Macros.Count; i++)
            {
                var macro = compilation.Macros[i];
                if (macro.Name == name)
                    return macro;
            }
            return null;
        }

        public static bool TryFindMacro(this CppCompilation compilation, string name, [NotNullWhen(true)] out CppMacro? macro)
        {
            macro = FindMacro(compilation, name);
            return macro != null;
        }
    }
}