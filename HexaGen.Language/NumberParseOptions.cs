namespace HexaGen.Language
{
    using System;

    [Flags]
    public enum NumberParseOptions
    {
        None = 0,
        AllowHex = 1,
        AllowBinary = 2,
        AllowExponent = 4,
        AllowNegative = 8,
        AllowPositive = 16,
        AllowSuffix = 32,
        AllowDecimal = 64,
        AllowAll = AllowHex | AllowBinary | AllowExponent | AllowNegative | AllowPositive | AllowSuffix | AllowDecimal,
    }
}