namespace HexaGen.Cpp2C.Metadata
{
    public class CFunction
    {
        public CFunction(string exportedName, string friendlyName, string? comment)
        {
            ExportedName = exportedName;
            FriendlyName = friendlyName;
            Comment = comment;
        }

        public string ExportedName { get; set; }

        public string FriendlyName { get; set; }

        public string? Comment { get; set; }
    }
}