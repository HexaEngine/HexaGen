namespace HexaGen.Core.Mapping
{
    using System.Diagnostics.CodeAnalysis;

    public class TypeMapping
    {
        public TypeMapping(string exportedName, string? friendlyName, string? comment)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
            Comment = comment;
        }

        public string ExportedName { get; set; }

        public string? FriendlyName { get; set; }

        public string? Comment { get; set; }

        public List<TypeFieldMapping> FieldMappings { get; set; } = new();

        public bool TryGetFieldMapping(string valueName, [NotNullWhen(true)] out TypeFieldMapping? mapping)
        {
            for (int i = 0; i < FieldMappings.Count; i++)
            {
                var fieldMapping = FieldMappings[i];
                if (fieldMapping.ExportedName == valueName)
                {
                    mapping = fieldMapping;
                    return true;
                }
            }

            mapping = null;
            return false;
        }

        public TypeFieldMapping? GetFieldMapping(string valueName)
        {
            for (int i = 0; i < FieldMappings.Count; i++)
            {
                var fieldMapping = FieldMappings[i];
                if (fieldMapping.ExportedName == valueName)
                {
                    return fieldMapping;
                }
            }

            return null;
        }
    }
}