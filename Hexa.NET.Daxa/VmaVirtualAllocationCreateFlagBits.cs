namespace Hexa.NET.Daxa
{
    using HexaGen.Runtime;

    /// <summary>
    /// Flags to be passed as VmaVirtualAllocationCreateInfo::flags.<br/>
    /// </summary>
    [NativeName(NativeNameType.Enum, "VmaVirtualAllocationCreateFlagBits")]
    public enum VmaVirtualAllocationCreateFlagBits : int
    {
        /// <summary>
        /// <br/>
        /// This flag is only allowed for virtual blocks created with #VMA_VIRTUAL_BLOCK_CREATE_LINEAR_ALGORITHM_BIT flag.<br/>
        /// </summary>
        [NativeName(NativeNameType.EnumItem, "VMA_VIRTUAL_ALLOCATION_CREATE_UPPER_ADDRESS_BIT")]
        [NativeName(NativeNameType.Value, "VMA_ALLOCATION_CREATE_UPPER_ADDRESS_BIT")]
        UpperAddressBit = unchecked((int)VmaAllocationCreateFlagBits.UpperAddressBit),

        /// <summary>
        /// <br/>
        /// </summary>
        [NativeName(NativeNameType.EnumItem, "VMA_VIRTUAL_ALLOCATION_CREATE_STRATEGY_MIN_MEMORY_BIT")]
        [NativeName(NativeNameType.Value, "VMA_ALLOCATION_CREATE_STRATEGY_MIN_MEMORY_BIT")]
        StrategyMinMemoryBit = unchecked((int)VmaAllocationCreateFlagBits.StrategyMinMemoryBit),

        /// <summary>
        /// <br/>
        /// </summary>
        [NativeName(NativeNameType.EnumItem, "VMA_VIRTUAL_ALLOCATION_CREATE_STRATEGY_MIN_TIME_BIT")]
        [NativeName(NativeNameType.Value, "VMA_ALLOCATION_CREATE_STRATEGY_MIN_TIME_BIT")]
        StrategyMinTimeBit = unchecked((int)VmaAllocationCreateFlagBits.StrategyMinTimeBit),

        /// <summary>
        /// Allocation strategy that chooses always the lowest offset in available space.<br/>
        /// This is not the most efficient strategy but achieves highly packed data.<br/>
        /// </summary>
        [NativeName(NativeNameType.EnumItem, "VMA_VIRTUAL_ALLOCATION_CREATE_STRATEGY_MIN_OFFSET_BIT")]
        [NativeName(NativeNameType.Value, "VMA_ALLOCATION_CREATE_STRATEGY_MIN_OFFSET_BIT")]
        StrategyMinOffsetBit = unchecked((int)VmaAllocationCreateFlagBits.StrategyMinOffsetBit),

        /// <summary>
        /// <br/>
        /// These strategy flags are binary compatible with equivalent flags in #VmaAllocationCreateFlagBits.<br/>
        /// </summary>
        [NativeName(NativeNameType.EnumItem, "VMA_VIRTUAL_ALLOCATION_CREATE_STRATEGY_MASK")]
        [NativeName(NativeNameType.Value, "VMA_ALLOCATION_CREATE_STRATEGY_MASK")]
        StrategyMask = unchecked((int)VmaAllocationCreateFlagBits.StrategyMask),

        /// <summary>
        /// <br/>
        /// These strategy flags are binary compatible with equivalent flags in #VmaAllocationCreateFlagBits.<br/>
        /// </summary>
        [NativeName(NativeNameType.EnumItem, "VMA_VIRTUAL_ALLOCATION_CREATE_FLAG_BITS_MAX_ENUM")]
        [NativeName(NativeNameType.Value, "2147483647")]
        MaxEnum = unchecked(2147483647),

    }
}
