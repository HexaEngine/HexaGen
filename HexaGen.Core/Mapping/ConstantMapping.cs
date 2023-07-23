namespace HexaGen.Core.Mapping
{
    public class ConstantMapping
    {
        public ConstantMapping(string exportedName, string friendlyName, string comment, string type, string value)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
            Comment = comment;
            Type = type;
            Value = value;
        }

        public string ExportedName { get; set; }

        public string? FriendlyName { get; set; }

        public string? Comment { get; set; }

        public string? Type { get; set; }

        public string? Value { get; set; }
    }
}