namespace HexaGen.Language
{
    public enum TokenType : byte
    {
        Unknown = 0,
        Identifier = 1,
        Keyword = 2,
        Punctuation = 3,
        Operator = 4,
        Literal = 5,
        Comment = 6,
    }
}