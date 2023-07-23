namespace HexaGen.Core.Mapping
{
    public class FunctionMapping
    {
        public FunctionMapping(string exportedName, string friendlyName, string? comment, Dictionary<string, string> defaults, List<Dictionary<string, string>> customVariations)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
            Comment = comment;
            Defaults = defaults;
            CustomVariations = customVariations;
        }

        public string ExportedName { get; set; }

        public string? FriendlyName { get; set; }

        public string? Comment { get; set; }

        public Dictionary<string, string> Defaults { get; set; }

        public List<Dictionary<string, string>> CustomVariations { get; set; }
    }
}