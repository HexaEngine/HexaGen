namespace HexaGen.Language
{
    public class ParserOptions
    {
        public static readonly ParserOptions Default = new();

        public bool ParseComments { get; set; }
    }
}