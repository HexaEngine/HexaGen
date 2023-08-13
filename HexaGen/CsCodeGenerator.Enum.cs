namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.Mapping;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    public partial class CsCodeGenerator
    {
        protected readonly HashSet<string> LibDefinedEnums = new();
        public readonly HashSet<string> DefinedEnums = new();
        protected readonly Dictionary<string, CppEnum> DefinedCppEnums = new();

        protected virtual List<string> SetupEnumUsings()
        {
            List<string> usings = new() { "System", "HexaGen.Runtime" };
            usings.AddRange(settings.Usings);
            return usings;
        }

        protected virtual bool FilterEnumIgnored(GenContext context, ICppMember cppEnum, [NotNullWhen(false)] out string? csName)
        {
            csName = null;

            if (settings.AllowedEnums.Count != 0 && !settings.AllowedEnums.Contains(cppEnum.Name))
                return true;

            if (settings.IgnoredEnums.Contains(cppEnum.Name))
                return true;

            if (LibDefinedEnums.Contains(cppEnum.Name))
                return true;

            csName = settings.GetCsCleanName(cppEnum.Name);

            return false;
        }

        protected virtual bool FilterEnum(GenContext context, CppEnum cppEnum, string csName)
        {
            if (DefinedEnums.Contains(csName))
            {
                var e = DefinedCppEnums[csName];
                if (e.Name == cppEnum.Name)
                {
                    if (e.Items.Count == cppEnum.Items.Count)
                    {
                        bool failed = false;
                        for (int j = 0; j < cppEnum.Items.Count; j++)
                        {
                            var a = cppEnum.Items[j];
                            var b = e.Items[j];
                            if (a.Name != b.Name || a.Value != b.Value)
                            {
                                failed = true;
                                break;
                            }
                        }

                        if (!failed)
                            return true;
                    }
                }

                LogWarn($"{context.FilePath}: {cppEnum}, C#: {csName} is already defined!");
                return true;
            }

            DefinedCppEnums.Add(csName, cppEnum);
            DefinedEnums.Add(csName);

            return false;
        }

        protected virtual void GenerateEnums(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Enumerations.cs");

            using var writer = new CodeWriter(filePath, settings.Namespace, SetupEnumUsings());
            GenContext context = new(compilation, filePath, writer);

            for (int i = 0; i < compilation.Enums.Count; i++)
            {
                CppEnum cppEnum = compilation.Enums[i];
                WriteEnum(context, cppEnum, cppEnum);
            }

            for (int i = 0; i < compilation.Typedefs.Count; i++)
            {
                var typeDef = compilation.Typedefs[i];
                if (!typeDef.IsEnum(out var cppEnum))
                {
                    continue;
                }
                WriteEnum(context, cppEnum, typeDef);
            }
        }

        protected virtual void WriteEnum(GenContext context, CppEnum cppEnum, ICppMember cppMember)
        {
            if (FilterEnumIgnored(context, cppMember, out string? csName))
                return;

            if (FilterEnum(context, cppEnum, csName))
                return;

            EnumPrefix enumNamePrefix = settings.GetEnumNamePrefix(cppMember.Name);
            var writer = context.Writer;

            if (csName.EndsWith("_"))
            {
                csName = csName.Remove(csName.Length - 1);
            }

            var mapping = settings.GetEnumMapping(cppEnum.Name);

            csName = mapping?.FriendlyName ?? csName;

            LogInfo("defined enum " + csName);

            // Remove extension suffix from enum item values
            string extensionPrefix = "";

            bool noneAdded = false;
            cppEnum.Comment.WriteCsSummary(writer);
            writer.WriteLine($"[NativeName(NativeNameType.Enum, \"{cppEnum.Name}\")]");
            using (writer.PushBlock($"public enum {csName}"))
            {
                for (int j = 0; j < cppEnum.Items.Count; j++)
                {
                    CppEnumItem? enumItem = cppEnum.Items[j];
                    WriteEnumItem(context, mapping, enumItem, j, enumNamePrefix, extensionPrefix, ref noneAdded);
                }
            }

            writer.WriteLine();
        }

        protected virtual void WriteEnumItem(GenContext context, EnumMapping? mapping, CppEnumItem enumItem, int enumIndex, EnumPrefix enumNamePrefix, string extensionPrefix, ref bool noneAdded)
        {
            var writer = context.Writer;
            var itemMapping = mapping?.GetItemMapping(enumItem.Name);
            var enumItemName = settings.GetPrettyEnumName(enumItem.Name, enumNamePrefix);

            if (!string.IsNullOrEmpty(extensionPrefix) && enumItemName.EndsWith(extensionPrefix))
            {
                enumItemName = enumItemName.Remove(enumItemName.Length - extensionPrefix.Length);
            }

            enumItemName = itemMapping?.FriendlyName ?? enumItemName;

            if (enumItemName == "None" && noneAdded)
            {
                return;
            }

            var commentWritten = enumItem.Comment.WriteCsSummary(writer);
            if (!commentWritten)
            {
                commentWritten = FormatHelper.WriteCsSummary(itemMapping?.Comment, writer);
            }
            writer.WriteLine($"[NativeName(NativeNameType.EnumItem, \"{enumItem.Name}\")]");

            if (enumItem.ValueExpression is CppRawExpression rawExpression && !string.IsNullOrEmpty(rawExpression.Text))
            {
                string enumValueName = settings.GetPrettyEnumName(rawExpression.Text, enumNamePrefix);

                if (enumItem.Name == rawExpression.Text)
                {
                    writer.WriteLine($"{enumItemName} = {enumIndex},");
                    return;
                }

                if (!string.IsNullOrEmpty(extensionPrefix) && enumValueName.EndsWith(extensionPrefix))
                {
                    enumValueName = enumValueName.Remove(enumValueName.Length - extensionPrefix.Length);

                    if (enumItemName == enumValueName)
                        return;
                }

                if (rawExpression.Text == "'_'")
                {
                    writer.WriteLine($"{enumItemName} = unchecked((int){rawExpression.Text}),");
                    return;
                }

                if (rawExpression.Kind == CppExpressionKind.Unexposed)
                {
                    writer.WriteLine($"{enumItemName} = unchecked((int){enumValueName.Replace("_", "")}),");
                }
                else
                {
                    writer.WriteLine($"{enumItemName} = {enumValueName},");
                }
            }
            else
            {
                writer.WriteLine($"{enumItemName} = unchecked({enumItem.Value}),");
            }

            if (commentWritten)
                writer.WriteLine();
        }
    }
}