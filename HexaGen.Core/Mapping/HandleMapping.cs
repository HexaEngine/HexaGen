namespace HexaGen.Core.Mapping
{
    public class HandleMapping
    {
        public HandleMapping(string exportedName, string? friendlyName, string? comment)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
            Comment = comment;
        }

        public string ExportedName { get; set; }

        public string? FriendlyName { get; set; }

        public string? Comment { get; set; }
    }
}