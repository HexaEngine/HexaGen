﻿namespace HexaGen.GenerationSteps
{
    using HexaGen;
    using HexaGen.Core;
    using HexaGen.Core.Mapping;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Expressions;
    using HexaGen.CppAst.Model.Interfaces;
    using HexaGen.Metadata;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class EnumGenerationStep : GenerationStep
    {
        public readonly HashSet<CsEnumMetadata> LibDefinedEnums = new(IdentifierComparer<CsEnumMetadata>.Default);
        public readonly HashSet<CsEnumMetadata> DefinedEnums = new(IdentifierComparer<CsEnumMetadata>.Default);
        public readonly Dictionary<string, CsEnumMetadata> DefinedCppEnums = new();
        private int unknownEnumCounter = 0;

        public EnumGenerationStep(CsCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
        }

        public override string Name { get; } = "Enums";

        public override void Configure(CsCodeGeneratorConfig config)
        {
            Enabled = config.GenerateEnums;
        }

        public override void CopyToMetadata(CsCodeGeneratorMetadata metadata)
        {
            metadata.DefinedEnums.AddRange(DefinedEnums);
        }

        public override void CopyFromMetadata(CsCodeGeneratorMetadata metadata)
        {
            LibDefinedEnums.AddRange(metadata.DefinedEnums);
            foreach (var item in metadata.DefinedEnums)
            {
                DefinedCppEnums.Add(item.Identifier, item);
            }
        }

        public override void Reset()
        {
            LibDefinedEnums.Clear();
            DefinedEnums.Clear();
            DefinedCppEnums.Clear();
            unknownEnumCounter = 0;
        }

        protected virtual List<string> SetupEnumUsings()
        {
            List<string> usings = ["System", "HexaGen.Runtime", .. config.Usings];
            return usings;
        }

        protected virtual bool FilterEnum(GenContext? context, CsEnumMetadata metadata)
        {
            if (config.AllowedEnums.Count != 0 && !config.AllowedEnums.Contains(metadata.Identifier))
                return true;

            if (config.IgnoredEnums.Contains(metadata.Identifier))
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

                LogWarn($"{context?.FilePath}: {metadata} is already defined!");
                return true;
            }

            DefinedCppEnums.Add(metadata.Identifier, metadata);
            DefinedEnums.Add(metadata);

            return false;
        }

        public override void Generate(FileSet files, ParseResult result, string outputPath, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata)
        {
            var compilation = result.Compilation;
            string folder = Path.Combine(outputPath, "Enums");
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, "Enums.cs");

            if (config.OneFilePerType)
            {
                for (int i = 0; i < compilation.Enums.Count; i++)
                {
                    CppEnum cppEnum = compilation.Enums[i];
                    if (!files.Contains(cppEnum.SourceFile))
                        continue;

                    if (!files.Contains(cppEnum.SourceFile))
                        continue;

                    var csEnum = ParseEnum(cppEnum, cppEnum);
                    if (FilterEnum(null, csEnum))
                    {
                        continue;
                    }
                    WriteEnumFile(result, folder, filePath, csEnum);
                }

                for (int i = 0; i < compilation.Typedefs.Count; i++)
                {
                    var typeDef = compilation.Typedefs[i];
                    if (!files.Contains(typeDef.SourceFile))
                        continue;

                    if (!typeDef.IsEnum(out var cppEnum))
                    {
                        continue;
                    }
                    var csEnum = ParseEnum(cppEnum, typeDef);
                    if (FilterEnum(null, csEnum))
                    {
                        continue;
                    }
                    WriteEnumFile(result, folder, filePath, csEnum);
                }

                for (int i = 0; i < config.CustomEnums.Count; i++)
                {
                    var csEnum = config.CustomEnums[i];
                    WriteEnumFile(result, folder, filePath, csEnum);
                }
            }
            else
            {
                using var writer = new CsSplitCodeWriter(filePath, config.Namespace, SetupEnumUsings(), config.HeaderInjector, 1);
                GenContext context = new(result, filePath, writer);

                List<CsEnumMetadata> enums = [.. config.CustomEnums];

                for (int i = 0; i < compilation.Enums.Count; i++)
                {
                    CppEnum cppEnum = compilation.Enums[i];
                    if (!files.Contains(cppEnum.SourceFile))
                        continue;

                    var csEnum = ParseEnum(cppEnum, cppEnum);
                    if (FilterEnum(context, csEnum))
                    {
                        continue;
                    }
                    enums.Add(csEnum);
                }

                for (int i = 0; i < compilation.Typedefs.Count; i++)
                {
                    var typeDef = compilation.Typedefs[i];

                    if (!files.Contains(typeDef.SourceFile))
                        continue;

                    if (!typeDef.IsEnum(out var cppEnum))
                    {
                        continue;
                    }
                    var csEnum = ParseEnum(cppEnum, typeDef);
                    if (FilterEnum(context, csEnum))
                    {
                        continue;
                    }
                    enums.Add(csEnum);
                }

                for (int i = 0; i < enums.Count; i++)
                {
                    WriteEnum(context, enums[i]);
                    if (i + 1 != enums.Count)
                    {
                        writer.WriteLine();
                    }
                }
            }
        }

        private void WriteEnumFile(ParseResult result, string folder, string filePath, CsEnumMetadata csEnum)
        {
            using var writer = new CsCodeWriter(Path.Combine(folder, $"{csEnum.Name}.cs"), config.Namespace, SetupEnumUsings(), config.HeaderInjector);
            GenContext context = new(result, filePath, writer);
            WriteEnum(context, csEnum);
        }

        protected virtual CsEnumMetadata ParseEnum(CppEnum cppEnum, ICppMember cppMember)
        {
            string cppName = cppEnum.Name;
            string cppPrefixName = cppMember.Name;
            string csName = config.GetCsCleanName(cppName);

            if (csName.StartsWith("(unnamed enum at ") && csName.EndsWith(')'))
            {
                LogWarn($"Unnamed enum, {cppMember.Name}");
                csName = $"UnknownEnum{unknownEnumCounter++}";
            }

            if (string.IsNullOrEmpty(cppName))
            {
                csName = config.GetCsCleanName($"UnnamedEnum{unknownEnumCounter++}");
                LogWarn($"Unnamed enum, {cppEnum.FormatLocationAttribute()}");
                cppName = csName;
                if (string.IsNullOrEmpty(cppPrefixName))
                {
                    cppPrefixName = csName;
                }
            }

            EnumPrefix enumNamePrefix = config.GetEnumNamePrefixEx(cppPrefixName);

            if (csName.EndsWith("_"))
            {
                csName = csName.Remove(csName.Length - 1);
            }

            var mapping = config.GetEnumMapping(cppName);
            csName = mapping?.FriendlyName ?? csName;

            List<string> attributes = [];
            if (config.GenerateMetadata)
            {
                attributes.AddRange([$"[NativeName(NativeNameType.Enum, \"{cppName.Replace("\\", "\\\\")}\")]"]);
            }
            CsEnumMetadata csEnum = new(cppName, csName, attributes, config.WriteCsSummary(cppEnum.Comment));
            csEnum.BaseType = config.GetCsTypeName(cppEnum.IntegerType);
            bool flags = true;
            bool noneAdded = false;
            for (int j = 0; j < cppEnum.Items.Count; j++)
            {
                var item = ParseEnumItem(mapping, cppEnum.Items[j], j, enumNamePrefix, ref noneAdded);
                if (item != null)
                {
                    config.CustomEnumItemMapper?.Invoke(cppEnum, cppEnum.Items[j], csEnum, item);
                    csEnum.Items.Add(item);
                }

                if (item?.Value == null) continue; // skip on null.
                // do Flags check post mapper, cuz value could change.
                if (long.TryParse(item.CppValue, out var numLong))
                {
                    if (!(numLong == 0 || numLong > 0 && (numLong & numLong - 1) == 0))
                    {
                        flags = false;
                    }
                }
                if (ulong.TryParse(item.CppValue, out ulong numULong))
                {
                    if (!(numULong == 0 || numULong > 0 && (numULong & numULong - 1) == 0))
                    {
                        flags = false;
                    }
                }
            }

            flags |= cppName.Contains("flag", StringComparison.OrdinalIgnoreCase); // another heuristric for flags.

            if (flags)
            {
                attributes.Add("[Flags]");
            }

            return csEnum;
        }

        protected virtual CsEnumItemMetadata? ParseEnumItem(EnumMapping? mapping, CppEnumItem enumItem, int enumIndex, EnumPrefix enumNamePrefix, ref bool noneAdded)
        {
            var itemMapping = mapping?.GetItemMapping(enumItem.Name);
            var enumItemName = config.GetEnumNameEx(enumItem.Name, enumNamePrefix);

            enumItemName = itemMapping?.FriendlyName ?? enumItemName;

            if (enumItemName == "None" && noneAdded)
            {
                return null;
            }

            var commentWritten = config.WriteCsSummary(enumItem.Comment);
            if (itemMapping?.Comment != null)
            {
                commentWritten = config.WriteCsSummary(itemMapping?.Comment);
            }

            string cppValue;
            string csValue;
            if (enumItem.ValueExpression is CppRawExpression rawExpression && !string.IsNullOrEmpty(rawExpression.Text))
            {
                cppValue = rawExpression.Text;
                string enumValueName = config.GetEnumNameEx(rawExpression.Text, enumNamePrefix);

                if (config.KnownEnumValueNames.TryGetValue(rawExpression.Text, out string? knownName))
                {
                    enumValueName = knownName;
                }

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

            List<string> attributes = new();
            if (config.GenerateMetadata)
            {
                attributes.AddRange([$"[NativeName(NativeNameType.EnumItem, \"{enumItem.Name}\")]", $"[NativeName(NativeNameType.Value, \"{cppValue.ToLiteral()}\")]"]);
            }
            return new CsEnumItemMetadata(enumItem.Name, cppValue, enumItemName, csValue, attributes, commentWritten);
        }

        protected virtual void WriteEnum(GenContext context, CsEnumMetadata csEnum)
        {
            var writer = context.Writer;

            LogInfo("defined enum " + csEnum.Name);

            writer.WriteLines(csEnum.Comment);
            writer.WriteLines(csEnum.Attributes);

            using (writer.PushBlock($"public enum {csEnum.Name} : {csEnum.BaseType}"))
            {
                for (int j = 0; j < csEnum.Items.Count; j++)
                {
                    var csEnumItem = csEnum.Items[j];
                    WriteEnumItem(context, csEnumItem);

                    if (csEnumItem.Comment != null && j + 1 != csEnum.Items.Count)
                    {
                        writer.WriteLine();
                    }
                }
            }
        }

        protected virtual void WriteEnumItem(GenContext context, CsEnumItemMetadata csEnumItem)
        {
            var writer = context.Writer;
            writer.WriteLines(csEnumItem.Comment);
            writer.WriteLines(csEnumItem.Attributes);
            writer.WriteLine($"{csEnumItem.Name} = {csEnumItem.Value},");
        }
    }
}