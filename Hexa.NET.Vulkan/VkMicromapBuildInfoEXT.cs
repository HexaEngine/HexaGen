namespace Hexa.NET.Vulkan
{
    using HexaGen.Runtime;
    using System.Runtime.InteropServices;

    [NativeName(NativeNameType.StructOrClass, "VkMicromapBuildInfoEXT")]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct VkMicromapBuildInfoEXT
    {
        [NativeName(NativeNameType.Field, "sType")]
        [NativeName(NativeNameType.Type, "VkStructureType")]
        public VkStructureType SType;

        [NativeName(NativeNameType.Field, "pNext")]
        [NativeName(NativeNameType.Type, "const void*")]
        public unsafe void* PNext;

        [NativeName(NativeNameType.Field, "type")]
        [NativeName(NativeNameType.Type, "VkMicromapTypeEXT")]
        public VkMicromapTypeEXT Type;

        [NativeName(NativeNameType.Field, "flags")]
        [NativeName(NativeNameType.Type, "VkBuildMicromapFlagsEXT")]
        public uint Flags;

        [NativeName(NativeNameType.Field, "mode")]
        [NativeName(NativeNameType.Type, "VkBuildMicromapModeEXT")]
        public VkBuildMicromapModeEXT Mode;

        [NativeName(NativeNameType.Field, "dstMicromap")]
        [NativeName(NativeNameType.Type, "VkMicromapEXT")]
        public VkMicromapEXT DstMicromap;

        [NativeName(NativeNameType.Field, "usageCountsCount")]
        [NativeName(NativeNameType.Type, "uint32_t")]
        public uint UsageCountsCount;

        [NativeName(NativeNameType.Field, "pUsageCounts")]
        [NativeName(NativeNameType.Type, "const VkMicromapUsageEXT*")]
        public unsafe VkMicromapUsageEXT* PUsageCounts;

        [NativeName(NativeNameType.Field, "ppUsageCounts")]
        [NativeName(NativeNameType.Type, "const const VkMicromapUsageEXT**")]
        public unsafe VkMicromapUsageEXT** PpUsageCounts;

        [NativeName(NativeNameType.Field, "data")]
        [NativeName(NativeNameType.Type, "VkDeviceOrHostAddressConstKHR")]
        public VkDeviceOrHostAddressConstKHR Data;

        [NativeName(NativeNameType.Field, "scratchData")]
        [NativeName(NativeNameType.Type, "VkDeviceOrHostAddressKHR")]
        public VkDeviceOrHostAddressKHR ScratchData;

        [NativeName(NativeNameType.Field, "triangleArray")]
        [NativeName(NativeNameType.Type, "VkDeviceOrHostAddressConstKHR")]
        public VkDeviceOrHostAddressConstKHR TriangleArray;

        [NativeName(NativeNameType.Field, "triangleArrayStride")]
        [NativeName(NativeNameType.Type, "VkDeviceSize")]
        public ulong TriangleArrayStride;

        public unsafe VkMicromapBuildInfoEXT(VkStructureType sType = default, void* pNext = default, VkMicromapTypeEXT type = default, uint flags = default, VkBuildMicromapModeEXT mode = default, VkMicromapEXT dstMicromap = default, uint usageCountsCount = default, VkMicromapUsageEXT* pUsageCounts = default, VkMicromapUsageEXT** ppUsageCounts = default, VkDeviceOrHostAddressConstKHR data = default, VkDeviceOrHostAddressKHR scratchdata = default, VkDeviceOrHostAddressConstKHR triangleArray = default, ulong triangleArrayStride = default)
        {
            SType = sType;
            PNext = pNext;
            Type = type;
            Flags = flags;
            Mode = mode;
            DstMicromap = dstMicromap;
            UsageCountsCount = usageCountsCount;
            PUsageCounts = pUsageCounts;
            PpUsageCounts = ppUsageCounts;
            Data = data;
            ScratchData = scratchdata;
            TriangleArray = triangleArray;
            TriangleArrayStride = triangleArrayStride;
        }
    }
}