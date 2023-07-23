namespace Test
{
    using System;
    using System.Linq;

    internal class TypeReference
    {
        public string Name { get; }
        public string? Comment { get; }
        public string Type { get; }
        public string? TemplateType { get; }
        public int ArraySize { get; }
        public bool IsFunctionPointer { get; }
        public string[]? TypeVariants { get; }
        public bool IsEnum { get; }

        public TypeReference(string name, string? comment, string type, int asize, EnumDefinition[] enums)
            : this(name, comment, type, asize, null, enums, null) { }

        public TypeReference(string name, string? comment, string type, int asize, EnumDefinition[] enums, string[]? typeVariants)
            : this(name, comment, type, asize, null, enums, typeVariants) { }

        public TypeReference(string name, string? comment, string type, int asize, string? templateType, EnumDefinition[] enums)
            : this(name, comment, type, asize, templateType, enums, null) { }

        public TypeReference(string name, string? comment, string type, int asize, string? templateType, EnumDefinition[] enums, string[]? typeVariants)
        {
            Name = name;
            Comment = comment;
            Type = type.Replace("const", string.Empty).Trim();

            if (Type.StartsWith("ImVector_"))
            {
                if (Type.EndsWith("*"))
                {
                    Type = "ImVector*";
                }
                else
                {
                    Type = "ImVector";
                }
            }

            if (Type.StartsWith("ImChunkStream_"))
            {
                if (Type.EndsWith("*"))
                {
                    Type = "ImChunkStream*";
                }
                else
                {
                    Type = "ImChunkStream";
                }
            }

            TemplateType = templateType;
            ArraySize = asize;
            int startBracket = name.IndexOf('[');
            if (startBracket != -1)
            {
                //This is only for older cimgui binding jsons
                int endBracket = name.IndexOf(']');
                string sizePart = name.Substring(startBracket + 1, endBracket - startBracket - 1);
                if (ArraySize == 0)
                    ArraySize = ParseSizeString(sizePart, enums);
                Name = Name.Substring(0, startBracket);
            }
            IsFunctionPointer = Type.IndexOf('(') != -1;

            TypeVariants = typeVariants;

            IsEnum = enums.Any(t => t.Names.Contains(type) || t.FriendlyNames.Contains(type) || ImguiDefinitions.WellKnownEnums.Contains(type));
        }

        private int ParseSizeString(string sizePart, EnumDefinition[] enums)
        {
            int plusStart = sizePart.IndexOf('+');
            if (plusStart != -1)
            {
                string first = sizePart.Substring(0, plusStart);
                string second = sizePart.Substring(plusStart, sizePart.Length - plusStart);
                int firstVal = int.Parse(first);
                int secondVal = int.Parse(second);
                return firstVal + secondVal;
            }

            if (!int.TryParse(sizePart, out int ret))
            {
                foreach (EnumDefinition ed in enums)
                {
                    if (ed.Names.Any(n => sizePart.StartsWith(n)))
                    {
                        foreach (EnumMember member in ed.Members)
                        {
                            if (member.Name == sizePart)
                            {
                                return int.Parse(member.Value);
                            }
                        }
                    }
                }

                ret = -1;
            }

            return ret;
        }

        public TypeReference WithVariant(int variantIndex, EnumDefinition[] enums)
        {
            if (TypeVariants == null)
                throw new NotSupportedException();
            if (variantIndex == 0) return this;
            else return new TypeReference(Name, Comment, TypeVariants[variantIndex - 1], ArraySize, TemplateType, enums);
        }
    }
}