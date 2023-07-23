namespace HexaGen.Core.Mapping
{
    public class TypeFieldMapping
    {
        public TypeFieldMapping(string exportedName, string? displayName, string? comment)
        {
            ExportedName = exportedName;
            DisplayName = displayName;
            Comment = comment;
        }

        public string ExportedName { get; set; }

        public string? DisplayName { get; set; }

        public string? Comment { get; set; }
    }
}