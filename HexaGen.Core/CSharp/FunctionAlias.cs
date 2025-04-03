namespace HexaGen.Core.CSharp
{
    public class FunctionAlias
    {
        public FunctionAlias(string exportedName, string exportedAliasName, string friendlyName, string? comment)
        {
            ExportedName = exportedName;
            ExportedAliasName = exportedAliasName;
            FriendlyName = friendlyName;
            Comment = comment;
        }

        public string ExportedName { get; set; }

        public string ExportedAliasName { get; set; }

        public string FriendlyName { get; set; }

        public string? Comment { get; set; }
    }
}