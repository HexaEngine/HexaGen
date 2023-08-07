namespace HexaGen
{
    using ClangSharp;
    using CppAst;
    using HexaGen;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    public partial class CsCodeGenerator
    {
        private readonly HashSet<string> LibDefinedEnums = new();
        public readonly HashSet<string> DefinedEnums = new();

        public void GenerateEnums(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Enumerations.cs");
            string[] usings = { "System" };

            using var writer = new CodeWriter(filePath, settings.Namespace, usings.Concat(settings.Usings).ToArray());

            foreach (CppEnum cppEnum in compilation.Enums)
            {
                if (settings.AllowedEnums.Count != 0 && !settings.AllowedEnums.Contains(cppEnum.Name))
                    continue;
                if (settings.IgnoredEnums.Contains(cppEnum.Name))
                    continue;
                if (LibDefinedEnums.Contains(cppEnum.Name))
                    continue;

                if (DefinedEnums.Contains(cppEnum.Name))
                {
                    LogWarn($"{filePath}: {cppEnum} is already defined!");
                    continue;
                }

                DefinedEnums.Add(cppEnum.Name);

                string csName = settings.GetCsCleanName(cppEnum.Name);
                EnumPrefix enumNamePrefix = settings.GetEnumNamePrefix(cppEnum.Name);
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
                using (writer.PushBlock($"public enum {csName}"))
                {
                    for (int i = 0; i < cppEnum.Items.Count; i++)
                    {
                        CppEnumItem? enumItem = cppEnum.Items[i];
                        var itemMapping = mapping?.GetItemMapping(enumItem.Name);
                        var enumItemName = settings.GetPrettyEnumName(enumItem.Name, enumNamePrefix);

                        if (!string.IsNullOrEmpty(extensionPrefix) && enumItemName.EndsWith(extensionPrefix))
                        {
                            enumItemName = enumItemName.Remove(enumItemName.Length - extensionPrefix.Length);
                        }

                        enumItemName = itemMapping?.FriendlyName ?? enumItemName;

                        if (enumItemName == "None" && noneAdded)
                        {
                            continue;
                        }

                        var commentWritten = enumItem.Comment.WriteCsSummary(writer);
                        if (!commentWritten)
                        {
                            commentWritten = FormatHelper.WriteCsSummary(itemMapping?.Comment, writer);
                        }
                        if (enumItem.ValueExpression is CppRawExpression rawExpression)
                        {
                            string enumValueName = settings.GetPrettyEnumName(rawExpression.Text, enumNamePrefix);

                            if (enumItem.Name == rawExpression.Text)
                            {
                                writer.WriteLine($"{enumItemName} = {i},");
                                continue;
                            }

                            if (!string.IsNullOrEmpty(extensionPrefix) && enumValueName.EndsWith(extensionPrefix))
                            {
                                enumValueName = enumValueName.Remove(enumValueName.Length - extensionPrefix.Length);

                                if (enumItemName == enumValueName)
                                    continue;
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

                writer.WriteLine();
            }
        }
    }
}