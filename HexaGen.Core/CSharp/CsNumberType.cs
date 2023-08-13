namespace HexaGen
{
    public enum CsNumberType
    {
        None = 0,

        /// <summary>
        /// 1
        /// </summary>
        Int,

        /// <summary>
        /// 1.0 or 1.0d
        /// </summary>
        Double,

        /// <summary>
        /// 1.0f
        /// </summary>
        Float,

        /// <summary>
        /// 1.0m
        /// </summary>
        Decimal,

        /// <summary>
        /// 1u
        /// </summary>
        UInt,

        /// <summary>
        /// 1l or 1L
        /// </summary>
        Long,

        /// <summary>
        /// 1ul or 1UL
        /// </summary>
        ULong
    }
}