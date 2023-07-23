namespace HexaGen.Core.Mapping
{
    public class EnumItemMapping
    {
        public EnumItemMapping(string exportedName, string? friendlyName, string? comment, string? value)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
            Comment = comment;
            Value = value;
        }

        public string ExportedName { get; set; }

        public string? FriendlyName { get; set; }

        public string? Comment { get; set; }

        public string? Value { get; set; }
    }
}