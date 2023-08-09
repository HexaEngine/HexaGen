namespace HexaGen
{
    using CppAst;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public partial class CsCodeGenerator
    {
        private readonly HashSet<string> LibDefinedEnums = new();
        public readonly HashSet<string> DefinedEnums = new();
        private readonly Dictionary<string, CppEnum> DefinedCppEnums = new();

        protected virtual void GenerateEnums(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Enumerations.cs");
            string[] usings = { "System", "HexaGen.Runtime" };

            using var writer = new CodeWriter(filePath, settings.Namespace, usings.Concat(settings.Usings).ToArray());

            for (int i = 0; i < compilation.Enums.Count; i++)
            {
                CppEnum cppEnum = compilation.Enums[i];
                if (settings.AllowedEnums.Count != 0 && !settings.AllowedEnums.Contains(cppEnum.Name))
                    continue;
                if (settings.IgnoredEnums.Contains(cppEnum.Name))
                    continue;
                if (LibDefinedEnums.Contains(cppEnum.Name))
                    continue;

                string csName = settings.GetCsCleanName(cppEnum.Name);

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
                                continue;
                        }
                    }

                    LogWarn($"{filePath}: {cppEnum}, C#: {csName} is already defined!");
                    continue;
                }

                DefinedCppEnums.Add(csName, cppEnum);
                DefinedEnums.Add(csName);

                EnumPrefix enumNamePrefix = settings.GetEnumNamePrefix(cppEnum.Name);

                WriteEnum(writer, cppEnum, csName, enumNamePrefix);
            }

            for (int i = 0; i < compilation.Typedefs.Count; i++)
            {
                var typeDef = compilation.Typedefs[i];
                if (!typeDef.IsEnum(out var cppEnum))
                {
                    continue;
                }

                if (settings.AllowedEnums.Count != 0 && !settings.AllowedEnums.Contains(typeDef.Name))
                    continue;
                if (settings.IgnoredEnums.Contains(typeDef.Name))
                    continue;
                if (LibDefinedEnums.Contains(typeDef.Name))
                    continue;

                string csName = settings.GetCsCleanName(typeDef.Name);

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
                                continue;
                        }
                    }

                    LogWarn($"{filePath}: {cppEnum}, C#: {csName} is already defined!");
                    continue;
                }

                DefinedCppEnums.Add(csName, cppEnum);
                DefinedEnums.Add(csName);

                EnumPrefix enumNamePrefix = settings.GetEnumNamePrefix(typeDef.Name);

                WriteEnum(writer, cppEnum, csName, enumNamePrefix);
            }
        }

        private void WriteEnum(CodeWriter writer, CppEnum cppEnum, string csName, EnumPrefix enumNamePrefix)
        {
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
                    writer.WriteLine($"[NativeName(NativeNameType.EnumItem, \"{enumItem.Name}\")]");

                    if (enumItem.ValueExpression is CppRawExpression rawExpression)
                    {
                        string enumValueName = settings.GetPrettyEnumName(rawExpression.Text, enumNamePrefix);

                        if (enumItem.Name == rawExpression.Text)
                        {
                            writer.WriteLine($"{enumItemName} = {j},");
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