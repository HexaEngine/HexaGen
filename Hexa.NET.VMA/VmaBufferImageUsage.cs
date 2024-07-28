namespace Hexa.NET.VMA
{
    using System.Runtime.CompilerServices;
    using Vulkan;
#if VMA_KHR_MAINTENANCE5
    using BaseType = ulong; // VkFlags64
#else
    using BaseType = uint; // VkFlags32
#endif
    public struct VmaBufferImageUsage
    {
        public static readonly VmaBufferImageUsage UNKNOWN = new(0);

        public BaseType Value;

        public VmaBufferImageUsage()
        {
            Value = UNKNOWN.Value;
        }

        public VmaBufferImageUsage(BaseType usage)
        {
            Value = usage;
        }

        public VmaBufferImageUsage(VkBufferCreateInfo createInfo, bool useKhrMaintenance5)
        {
#if VMA_KHR_MAINTENANCE5
            if (useKhrMaintenance5)
            {
                // If VkBufferCreateInfo::pNext chain contains VkBufferUsageFlags2CreateInfoKHR,
                // take usage from it and ignore VkBufferCreateInfo::usage, per specification
                // of the VK_KHR_maintenance5 extension.
                VkBufferUsageFlagBits2CreateInfoKHR* usageFlags2 = VmaPnextChainFind<VkBufferUsageFlags2CreateInfoKHR>(&createInfo, VK_STRUCTURE_TYPE_BUFFER_USAGE_FLAGS_2_CREATE_INFO_KHR);
                if (usageFlags2)
                {
                    Value = usageFlags2->usage;
                    return;
                }
            }
#endif

            Value = (BaseType)createInfo.Usage;
        }

        public VmaBufferImageUsage(VkImageCreateInfo createInfo)
        {
            Value = (BaseType)createInfo.Usage;
        }
    }
}