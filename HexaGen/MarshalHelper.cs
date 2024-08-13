namespace HexaGen
{
    using CppAst;
    using HexaGen.Core;
    using HexaGen.Core.CSharp;
    using System.Text;

    public static class MarshalHelper
    {
        public static void WriteStringConvertToManaged(StringBuilder sb, CppType type)
        {
            CppPrimitiveKind primitiveKind = type.GetPrimitiveKind();
            if (primitiveKind == CppPrimitiveKind.Char)
            {
                sb.Append("Utils.DecodeStringUTF8(");
            }
            else if (primitiveKind == CppPrimitiveKind.WChar)
            {
                sb.Append("Utils.DecodeStringUTF16(");
            }
            else
            {
                throw new NotSupportedException($"String type ({primitiveKind}) is not supported");
            }
        }

        public static void WriteStringConvertToManaged(StringBuilder sb, CsType type)
        {
            if (type.StringType == CsStringType.StringUTF8)
            {
                sb.Append("Utils.DecodeStringUTF8(");
            }
            else if (type.StringType == CsStringType.StringUTF16)
            {
                sb.Append("Utils.DecodeStringUTF16(");
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static void WriteStringConvertToManaged(ICodeWriter writer, CppType type, string variable, string pointer)
        {
            CppPrimitiveKind primitiveKind = type.GetPrimitiveKind();
            if (primitiveKind == CppPrimitiveKind.Char)
            {
                writer.WriteLine($"{variable} = Marshal.DecodeStringUTF8({pointer});");
            }
            else if (primitiveKind == CppPrimitiveKind.WChar)
            {
                writer.WriteLine($"{variable} = Marshal.DecodeStringUTF16({pointer});");
            }
            else
            {
                throw new NotSupportedException($"String type ({primitiveKind}) is not supported");
            }
        }

        public static void WriteStringConvertToManaged(ICodeWriter writer, CsType type, string variable, string pointer)
        {
            if (type.StringType == CsStringType.StringUTF8)
            {
                writer.WriteLine($"{variable} = Utils.DecodeStringUTF8({pointer});");
            }
            else if (type.StringType == CsStringType.StringUTF16)
            {
                writer.WriteLine($"{variable} = Utils.DecodeStringUTF16({pointer});");
            }
            else
            {
                throw new NotSupportedException($"String type ({type.StringType}) is not supported");
            }
        }

        public static void WriteStringConvertToUnmanaged(ICodeWriter writer, CppType type, string name, int i)
        {
            CppPrimitiveKind primitiveKind = type.GetPrimitiveKind();
            if (primitiveKind == CppPrimitiveKind.Char)
            {
                writer.WriteLine($"byte* pStr{i} = null;");
                writer.WriteLine($"int pStrSize{i} = 0;");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    writer.WriteLine($"pStrSize{i} = Utils.GetByteCountUTF8({name});");
                    using (writer.PushBlock($"if (pStrSize{i} >= Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStr{i} = Utils.Alloc<byte>(pStrSize{i} + 1);");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrStack{i} = stackalloc byte[pStrSize{i} + 1];");
                        writer.WriteLine($"pStr{i} = pStrStack{i};");
                    }
                    writer.WriteLine($"int pStrOffset{i} = Utils.EncodeStringUTF8({name}, pStr{i}, pStrSize{i});");
                    writer.WriteLine($"pStr{i}[pStrOffset{i}] = 0;");
                }
            }
            else if (primitiveKind == CppPrimitiveKind.WChar)
            {
                writer.WriteLine($"char* pStr{i} = null;");
                writer.WriteLine($"int pStrSize{i} = 0;");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    writer.WriteLine($"pStrSize{i} = Utils.GetByteCountUTF16({name});");
                    using (writer.PushBlock($"if (pStrSize{i} >= Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStr{i} = Utils.Alloc<char>(pStrSize{i} + 1);");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrStack{i} = stackalloc byte[pStrSize{i} + 1];");
                        writer.WriteLine($"pStr{i} = (char*)pStrStack{i};");
                    }
                    writer.WriteLine($"int pStrOffset{i} = Utils.EncodeStringUTF16({name}, pStr{i}, pStrSize{i});");
                    writer.WriteLine($"pStr{i}[pStrOffset{i}] = '\\0';");
                }
            }
            else
            {
                throw new NotSupportedException($"String type ({primitiveKind}) is not supported");
            }
        }

        public static void WriteStringConvertToUnmanaged(ICodeWriter writer, CsType type, string name, int i)
        {
            if (type.StringType == CsStringType.StringUTF8)
            {
                writer.WriteLine($"byte* pStr{i} = null;");
                writer.WriteLine($"int pStrSize{i} = 0;");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    writer.WriteLine($"pStrSize{i} = Utils.GetByteCountUTF8({name});");
                    using (writer.PushBlock($"if (pStrSize{i} >= Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStr{i} = Utils.Alloc<byte>(pStrSize{i} + 1);");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrStack{i} = stackalloc byte[pStrSize{i} + 1];");
                        writer.WriteLine($"pStr{i} = pStrStack{i};");
                    }
                    writer.WriteLine($"int pStrOffset{i} = Utils.EncodeStringUTF8({name}, pStr{i}, pStrSize{i});");
                    writer.WriteLine($"pStr{i}[pStrOffset{i}] = 0;");
                }
            }
            else if (type.StringType == CsStringType.StringUTF16)
            {
                writer.WriteLine($"char* pStr{i} = null;");
                writer.WriteLine($"int pStrSize{i} = 0;");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    writer.WriteLine($"pStrSize{i} = Utils.GetByteCountUTF16({name});");
                    using (writer.PushBlock($"if (pStrSize{i} >= Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStr{i} = Utils.Alloc<char>(pStrSize{i} + 1);");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrStack{i} = stackalloc byte[pStrSize{i} + 1];");
                        writer.WriteLine($"pStr{i} = (char*)pStrStack{i};");
                    }
                    writer.WriteLine($"int pStrOffset{i} = Utils.EncodeStringUTF16({name}, pStr{i}, pStrSize{i});");
                    writer.WriteLine($"pStr{i}[pStrOffset{i}] = '\\0';");
                }
            }
            else
            {
                throw new NotSupportedException($"String type ({type.StringType}) is not supported");
            }
        }

        public static void WriteFreeString(ICodeWriter writer, int i)
        {
            using (writer.PushBlock($"if (pStrSize{i} >= Utils.MaxStackallocSize)"))
            {
                writer.WriteLine($"Utils.Free(pStr{i});");
            }
        }

        public static void WriteStringArrayConvertToUnmanaged(ICodeWriter writer, CppType type, string name, int i)
        {
            CppPrimitiveKind primitiveKind = type.GetPrimitiveKind();
            if (primitiveKind == CppPrimitiveKind.Char)
            {
                writer.WriteLine($"byte** pStrArray{i} = null;");
                writer.WriteLine($"int pStrArraySize{i} = Utils.GetByteCountArray({name});");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    using (writer.PushBlock($"if (pStrArraySize{i} > Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStrArray{i} = (byte**)Utils.Alloc<byte>(pStrArraySize{i});");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrArrayStack{i} = stackalloc byte[pStrArraySize{i}];");
                        writer.WriteLine($"pStrArray{i} = (byte**)pStrArrayStack{i};");
                    }
                }
                using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
                {
                    writer.WriteLine($"pStrArray{i}[i] = (byte*)Utils.StringToUTF8Ptr({name}[i]);");
                }
            }
            else if (primitiveKind == CppPrimitiveKind.WChar)
            {
                writer.WriteLine($"char** pStrArray{i} = null;");
                writer.WriteLine($"int pStrArraySize{i} = Utils.GetByteCountArray({name});");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    using (writer.PushBlock($"if (pStrArraySize{i} > Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStrArray{i} = (char**)Utils.Alloc<byte>(pStrArraySize{i});");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrArrayStack{i} = stackalloc byte[pStrArraySize{i}];");
                        writer.WriteLine($"pStrArray{i} = (char**)pStrArrayStack{i};");
                    }
                }
                using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
                {
                    writer.WriteLine($"pStrArray{i}[i] = (char*)Utils.StringToUTF16Ptr({name}[i]);");
                }
            }
            else
            {
                throw new NotSupportedException($"String type ({primitiveKind}) is not supported");
            }
        }

        public static void WriteStringArrayConvertToUnmanaged(ICodeWriter writer, CppType type, string name, string varName)
        {
            CppPrimitiveKind primitiveKind = type.GetPrimitiveKind();
            if (primitiveKind == CppPrimitiveKind.Char)
            {
                writer.WriteLine($"byte** {varName} = null;");
                writer.WriteLine($"int {varName}Size = Utils.GetByteCountArray({name});");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    using (writer.PushBlock($"if ({varName}Size > Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"{varName} = (byte**)Utils.Alloc<byte>({varName});");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* {varName}Stack = stackalloc byte[{varName}];");
                        writer.WriteLine($"{varName} = (byte**){varName}Stack;");
                    }
                }
                using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
                {
                    writer.WriteLine($"{varName}[i] = (byte*)Utils.StringToUTF8Ptr({name}[i]);");
                }
            }
            else if (primitiveKind == CppPrimitiveKind.WChar)
            {
                writer.WriteLine($"char** {varName} = null;");
                writer.WriteLine($"int {varName}Size = Utils.GetByteCountArray({name});");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    using (writer.PushBlock($"if ({varName}Size > Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"{varName} = (char**)Utils.Alloc<byte>({varName}Size);");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* {varName}Stack = stackalloc byte[{varName}Size];");
                        writer.WriteLine($"{varName} = (char**){varName}Stack;");
                    }
                }
                using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
                {
                    writer.WriteLine($"{varName}[i] = (char*)Utils.StringToUTF16Ptr({name}[i]);");
                }
            }
            else
            {
                throw new NotSupportedException($"String type ({primitiveKind}) is not supported");
            }
        }

        public static void WriteStringArrayConvertToUnmanaged(ICodeWriter writer, CsType type, string name, int i)
        {
            if (type.StringType == CsStringType.StringUTF8)
            {
                writer.WriteLine($"byte** pStrArray{i} = null;");
                writer.WriteLine($"int pStrArraySize{i} = Utils.GetByteCountArray({name});");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    using (writer.PushBlock($"if (pStrArraySize{i} > Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStrArray{i} = (byte**)Utils.Alloc<byte>(pStrArraySize{i});");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrArrayStack{i} = stackalloc byte[pStrArraySize{i}];");
                        writer.WriteLine($"pStrArray{i} = (byte**)pStrArrayStack{i};");
                    }
                }
                using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
                {
                    writer.WriteLine($"pStrArray{i}[i] = (byte*)Utils.StringToUTF8Ptr({name}[i]);");
                }
            }
            else if (type.StringType == CsStringType.StringUTF16)
            {
                writer.WriteLine($"char** pStrArray{i} = null;");
                writer.WriteLine($"int pStrArraySize{i} = Utils.GetByteCountArray({name});");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    using (writer.PushBlock($"if (pStrArraySize{i} > Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"pStrArray{i} = (char**)Utils.Alloc<byte>(pStrArraySize{i});");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* pStrArrayStack{i} = stackalloc byte[pStrArraySize{i}];");
                        writer.WriteLine($"pStrArray{i} = (char**)pStrArrayStack{i};");
                    }
                }
                using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
                {
                    writer.WriteLine($"pStrArray{i}[i] = (char*)Utils.StringToUTF16Ptr({name}[i]);");
                }
            }
            else
            {
                throw new NotSupportedException($"String type ({type.StringType}) is not supported");
            }
        }

        public static void WriteStringArrayConvertToUnmanaged(ICodeWriter writer, CsType type, string name, string varName)
        {
            CsPrimitiveType primitiveKind = type.PrimitiveType;
            if (primitiveKind == CsPrimitiveType.Byte)
            {
                writer.WriteLine($"byte** {varName} = null;");
                writer.WriteLine($"int {varName}Size = Utils.GetByteCountArray({name});");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    using (writer.PushBlock($"if ({varName}Size > Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"{varName} = (byte**)Utils.Alloc<byte>({varName}Size);");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* {varName}Stack = stackalloc byte[{varName}Size];");
                        writer.WriteLine($"{varName} = (byte**){varName}Stack;");
                    }
                }
                using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
                {
                    writer.WriteLine($"{varName}[i] = (byte*)Utils.StringToUTF8Ptr({name}[i]);");
                }
            }
            else if (primitiveKind == CsPrimitiveType.Char)
            {
                writer.WriteLine($"char** {varName} = null;");
                writer.WriteLine($"int {varName}Size = Utils.GetByteCountArray({name});");
                using (writer.PushBlock($"if ({name} != null)"))
                {
                    using (writer.PushBlock($"if ({varName}Size > Utils.MaxStackallocSize)"))
                    {
                        writer.WriteLine($"{varName} = (char**)Utils.Alloc<byte>({varName}Size);");
                    }
                    using (writer.PushBlock("else"))
                    {
                        writer.WriteLine($"byte* {varName}Stack = stackalloc byte[{varName}Size];");
                        writer.WriteLine($"{varName} = (char**){varName}Stack;");
                    }
                }
                using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
                {
                    writer.WriteLine($"{varName}[i] = (char*)Utils.StringToUTF16Ptr({name}[i]);");
                }
            }
            else
            {
                throw new NotSupportedException($"String type ({primitiveKind}) is not supported");
            }
        }

        public static void WriteFreeUnmanagedStringArray(ICodeWriter writer, string name, int i)
        {
            using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
            {
                writer.WriteLine($"Utils.Free(pStrArray{i}[i]);");
            }
            using (writer.PushBlock($"if (pStrArraySize{i} >= Utils.MaxStackallocSize)"))
            {
                writer.WriteLine($"Utils.Free(pStrArray{i});");
            }
        }

        public static void WriteFreeUnmanagedStringArray(ICodeWriter writer, string name, string varName)
        {
            using (writer.PushBlock($"for (int i = 0; i < {name}.Length; i++)"))
            {
                writer.WriteLine($"Utils.Free({varName}[i]);");
            }
            using (writer.PushBlock($"if ({varName}Size >= Utils.MaxStackallocSize)"))
            {
                writer.WriteLine($"Utils.Free({varName});");
            }
        }
    }
}