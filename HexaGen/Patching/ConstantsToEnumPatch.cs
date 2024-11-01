using CppAst;
using HexaGen.Metadata;

namespace HexaGen.Patching
{
    public class ConstantsToEnumPatch : PrePatch
    {
        private readonly string macroPrefix;
        private readonly string csEnumName;
        private readonly string baseType;
        private readonly HashSet<string> ignored;
        private readonly HashSet<string> extra;

        public ConstantsToEnumPatch(string macroPrefix, string csEnumName, string baseType, HashSet<string>? ignored = null, HashSet<string>? extra = null)
        {
            this.macroPrefix = macroPrefix;
            this.csEnumName = csEnumName;
            this.baseType = baseType;
            this.ignored = ignored ?? [];
            this.extra = extra ?? [];
        }

        protected override void PatchCompilation(CsCodeGeneratorConfig settings, CppCompilation compilation)
        {
            List<CppMacro> keyEnums = [];
            HashSet<string> itemNames = [];
            foreach (var macro in compilation.Macros)
            {
                if (ignored.Contains(macro.Name)) continue;
                if (macro.Name.StartsWith(macroPrefix) || extra.Contains(macro.Name))
                {
                    keyEnums.Add(macro);
                    itemNames.Add(macro.Name);
                }
            }

            CsEnumMetadata metadata = new(macroPrefix, csEnumName, [], null)
            {
                BaseType = baseType
            };
            var prefix = settings.GetEnumNamePrefixEx(macroPrefix);
            foreach (var macro in keyEnums)
            {
                var itemName = settings.GetEnumName(macro.Name, prefix);

                string csValue = macro.Value;
                if (csValue.IsNumeric(out var numberType, NumberParseOptions.All))
                {
                    if (numberType == NumberType.AnyFloat)
                    {
                        continue;
                    }
                }
                else if (csValue.IsConstantExpression())
                {
                    continue;
                }
                else if (csValue.IsString())
                {
                    continue;
                }
                else if (itemNames.Contains(csValue))
                {
                    csValue = settings.GetEnumName(csValue, prefix);
                }
                else
                {
                    foreach (var item in itemNames)
                    {
                        var index = csValue.IndexOf(item);
                        if (index != -1)
                        {
                            csValue = csValue.Remove(index, item.Length);
                            csValue = csValue.Insert(index, settings.GetEnumName(item, prefix));
                        }
                    }
                }

                CsEnumItemMetadata itemMeta = new(macro.Name, macro.Value, itemName, csValue, [], null);
                metadata.Items.Add(itemMeta);
                settings.IgnoredConstants.Add(macro.Name);
            }
            settings.CustomEnums.Add(metadata);
        }
    }
}