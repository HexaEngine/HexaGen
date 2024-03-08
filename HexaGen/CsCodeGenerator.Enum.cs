namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.Mapping;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    public partial class CsCodeGenerator
    {
        protected readonly HashSet<CsEnumMetadata> LibDefinedEnums = new(IdentifierComparer<CsEnumMetadata>.Default);
        public readonly HashSet<CsEnumMetadata> DefinedEnums = new(IdentifierComparer<CsEnumMetadata>.Default);
        protected readonly Dictionary<string, CsEnumMetadata> DefinedCppEnums = new();

        protected virtual List<string> SetupEnumUsings()
        {
            List<string> usings = new() { "System", "HexaGen.Runtime" };
            usings.AddRange(settings.Usings);
            return usings;
        }

        protected virtual bool FilterEnum(GenContext context, CsEnumMetadata metadata)
        {
            if (settings.AllowedEnums.Count != 0 && !settings.AllowedEnums.Contains(metadata.Identifier))
                return true;

            if (settings.IgnoredEnums.Contains(metadata.Identifier))
                return true;

            if (LibDefinedEnums.Contains(metadata))
                return true;

            if (DefinedEnums.Contains(metadata))
            {
                var e = DefinedCppEnums[metadata.Identifier];
                if (e.Name == metadata.CppName)
                {
                    if (e.Items.Count == metadata.Items.Count)
                    {
                        bool failed = false;
                        for (int j = 0; j < metadata.Items.Count; j++)
                        {
                            var a = metadata.Items[j];
                            var b = e.Items[j];
                            if (a.CppName != b.CppName || a.CppValue != b.CppValue)
                            {
                                failed = true;
                                break;
                            }
                        }

                        if (!failed)
                            return true;
                    }
                }

                LogWarn($"{context.FilePath}: {metadata} is already defined!");
                return true;
            }

            DefinedCppEnums.Add(metadata.Identifier, metadata);
            DefinedEnums.Add(metadata);

            return false;
        }

        protected virtual void GenerateEnums(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Enumerations.cs");

            using var writer = new CsCodeWriter(filePath, settings.Namespace, SetupEnumUsings());
            GenContext context = new(compilation, filePath, writer);

            for (int i = 0; i < compilation.Enums.Count; i++)
            {
                CppEnum cppEnum = compilation.Enums[i];
                var csEnum = ParseEnum(cppEnum, cppEnum);
                WriteEnum(context, csEnum);
            }

            for (int i = 0; i < compilation.Typedefs.Count; i++)
            {
                var typeDef = compilation.Typedefs[i];
                if (!typeDef.IsEnum(out var cppEnum))
                {
                    continue;
                }
                var csEnum = ParseEnum(cppEnum, typeDef);
                WriteEnum(context, csEnum);
            }
        }

        private int unknownEnumCounter = 0;

        protected virtual CsEnumMetadata ParseEnum(CppEnum cppEnum, ICppMember cppMember)
        {
            CsEnumMetadata csEnum = new(cppEnum.Name);

            string csName = settings.GetCsCleanName(cppEnum.Name);

            if (csName.StartsWith("(unnamed enum at ") && csName.EndsWith(')'))
            {
                LogWarn($"Unnamed enum, {cppMember.Name}");
                csName = $"UnknownEnum{unknownEnumCounter++}";
            }

            EnumPrefix enumNamePrefix = settings.GetEnumNamePrefix(cppMember.Name);

            if (csName.EndsWith("_"))
            {
                csName = csName.Remove(csName.Length - 1);
            }

            var mapping = settings.GetEnumMapping(cppEnum.Name);
            csName = mapping?.FriendlyName ?? csName;

            csEnum.Name = csName;
            csEnum.Comment = settings.WriteCsSummary(cppEnum.Comment);

            bool noneAdded = false;
            for (int j = 0; j < cppEnum.Items.Count; j++)
            {
                var item = ParseEnumItem(mapping, cppEnum.Items[j], j, enumNamePrefix, ref noneAdded);
                if (item != null)
                    csEnum.Items.Add(item);
            }

            return csEnum;
        }

        protected virtual CsEnumItemMetadata? ParseEnumItem(EnumMapping? mapping, CppEnumItem enumItem, int enumIndex, EnumPrefix enumNamePrefix, ref bool noneAdded)
        {
            var itemMapping = mapping?.GetItemMapping(enumItem.Name);
            var enumItemName = settings.GetEnumNameEx(enumItem.Name, enumNamePrefix);

            enumItemName = itemMapping?.FriendlyName ?? enumItemName;

            if (enumItemName == "None" && noneAdded)
            {
                return null;
            }

            var commentWritten = settings.WriteCsSummary(enumItem.Comment);
            if (itemMapping?.Comment != null)
            {
                commentWritten = settings.WriteCsSummary(itemMapping?.Comment);
            }

            string cppValue;
            string csValue;
            if (enumItem.ValueExpression is CppRawExpression rawExpression && !string.IsNullOrEmpty(rawExpression.Text))
            {
                cppValue = rawExpression.Text;
                string enumValueName = settings.GetEnumNameEx(rawExpression.Text, enumNamePrefix);

                if (enumItem.Name == rawExpression.Text)
                {
                    csValue = $"{enumIndex}";
                }
                else if (rawExpression.Text == "'_'")
                {
                    csValue = $"unchecked((int){rawExpression.Text})";
                }
                else if (rawExpression.Kind == CppExpressionKind.Unexposed)
                {
                    csValue = $"unchecked((int){enumValueName.Replace("_", "")})";
                }
                else
                {
                    csValue = $"{enumValueName}";
                }
            }
            else
            {
                cppValue = enumItem.Value.ToString();
                csValue = $"unchecked({enumItem.Value})";
            }

            if (itemMapping?.Value != null)
            {
                csValue = itemMapping.Value;
            }

            return new CsEnumItemMetadata(enumItem.Name, cppValue, enumItemName, csValue, commentWritten);
        }

        protected virtual void WriteEnum(GenContext context, CsEnumMetadata csEnum)
        {
            if (FilterEnum(context, csEnum))
            {
                return;
            }

            var writer = context.Writer;

            LogInfo("defined enum " + csEnum.Name);

            writer.WriteLines(csEnum.Comment);
            if (settings.GenerateMetadata)
            {
                writer.WriteLine($"[NativeName(NativeNameType.Enum, \"{csEnum.CppName.Replace("\\", "\\\\")}\")]");
            }

            using (writer.PushBlock($"public enum {csEnum.Name}"))
            {
                for (int j = 0; j < csEnum.Items.Count; j++)
                {
                    WriteEnumItem(context, csEnum.Items[j]);
                }
            }

            writer.WriteLine();
        }

        protected virtual void WriteEnumItem(GenContext context, CsEnumItemMetadata csEnumItem)
        {
            var writer = context.Writer;
            writer.WriteLines(csEnumItem.Comment);
            if (settings.GenerateMetadata)
            {
                writer.WriteLine($"[NativeName(NativeNameType.EnumItem, \"{csEnumItem.CppName}\")]");
                writer.WriteLine($"[NativeName(NativeNameType.Value, \"{csEnumItem.EscapedCppValue}\")]");
            }
            writer.WriteLine($"{csEnumItem.Name} = {csEnumItem.Value},");
            if (csEnumItem.Comment != null)
            {
                writer.WriteLine();
            }
        }
    }
}