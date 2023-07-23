namespace HexaGen.Core.Mapping
{
    public class HandleMapping
    {
        public HandleMapping(string exportedName, string friendlyName)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
        }

        public string ExportedName { get; set; }

        public string? FriendlyName { get; set; }
    }
}