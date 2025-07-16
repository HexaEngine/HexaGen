namespace HexaGen.Core.Mapping
{
    using System.Diagnostics.CodeAnalysis;
    using Newtonsoft.Json;

    public class EnumMapping
    {
        public EnumMapping(string exportedName, string? friendlyName, string? comment)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
            Comment = comment;
            ItemMappings = new();
        }

        [JsonConstructor]
        public EnumMapping(string exportedName, string? friendlyName, string? comment, List<EnumItemMapping> itemMappings)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
            Comment = comment;
            ItemMappings = itemMappings;
        }

        public string ExportedName { get; set; }

        public string? FriendlyName { get; set; }

        public string? Comment { get; set; }

        public List<EnumItemMapping> ItemMappings { get; set; }

        public bool TryGetItemMapping(string valueName, [NotNullWhen(true)] out EnumItemMapping? mapping)
        {
            for (int i = 0; i < ItemMappings.Count; i++)
            {
                var enumItemMapping = ItemMappings[i];
                if (enumItemMapping.ExportedName == valueName)
                {
                    mapping = enumItemMapping;
                    return true;
                }
            }

            mapping = null;
            return false;
        }

        public EnumItemMapping? GetItemMapping(string valueName)
        {
            for (int i = 0; i < ItemMappings.Count; i++)
            {
                var enumItemMapping = ItemMappings[i];
                if (enumItemMapping.ExportedName == valueName)
                {
                    return enumItemMapping;
                }
            }

            return null;
        }
    }
}