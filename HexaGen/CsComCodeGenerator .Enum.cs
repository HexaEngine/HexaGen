namespace HexaGen
{
    using CppAst;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public partial class CsComCodeGenerator
    {
        private readonly HashSet<string> LibDefinedEnums = new();
        public readonly HashSet<string> DefinedEnums = new();

        public void GenerateEnums(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Enumerations.cs");
            string[] usings = { "System", "HexaGen.Runtime", "HexaGen.Runtime.COM" };

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
                string enumNamePrefix = GetEnumNamePrefix(cppEnum.Name);
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
                        var enumItemName = GetEnumItemName(enumItem.Name, enumNamePrefix);

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
                            string enumValueName = GetEnumItemName(rawExpression.Text, enumNamePrefix);

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

        private string GetEnumItemName(string cppEnumItemName, string enumNamePrefix)
        {
            string enumItemName = GetPrettyEnumName(cppEnumItemName, enumNamePrefix);

            return enumItemName;
        }

        private string NormalizeEnumValue(string value)
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

        public string GetEnumNamePrefix(string typeName)
        {
            if (settings.KnownEnumPrefixes.TryGetValue(typeName, out string? knownValue))
            {
                return knownValue;
            }

            string[] parts = typeName.Split('_', StringSplitOptions.RemoveEmptyEntries).SelectMany(x => x.SplitByCase()).ToArray();

            return string.Join("_", parts.Select(s => s.ToUpper()));
        }

        private string GetPrettyEnumName(string value, string enumPrefix)
        {
            if (settings.KnownEnumValueNames.TryGetValue(value, out string? knownName))
            {
                return knownName;
            }

            if (value.StartsWith("0x"))
                return value;

            string[] parts = value.Split('_', StringSplitOptions.RemoveEmptyEntries).SelectMany(x => x.SplitByCase()).ToArray();
            string[] prefixParts = enumPrefix.Split('_', StringSplitOptions.RemoveEmptyEntries);

            bool capture = false;
            var sb = new StringBuilder();
            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                if (settings.IgnoredParts.Contains(part, StringComparer.InvariantCultureIgnoreCase) || (prefixParts.Contains(part, StringComparer.InvariantCultureIgnoreCase) && !capture))
                {
                    continue;
                }

                part = part.ToLower();

                sb.Append(char.ToUpper(part[0]));
                sb.Append(part[1..]);
                capture = true;
            }

            if (sb.Length == 0)
                sb.Append(prefixParts[^1].ToCamelCase());

            string prettyName = sb.ToString();
            return (char.IsNumber(prettyName[0])) ? prefixParts[^1].ToCamelCase() + prettyName : prettyName;
        }
    }
}