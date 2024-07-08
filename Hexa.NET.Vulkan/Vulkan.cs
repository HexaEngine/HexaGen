namespace Hexa.NET.Vulkan
{
    public static unsafe partial class Vulkan
    {
        static Vulkan()
        {
            LibraryLoader.SetImportResolver();
        }

        public static uint MakeApiVersion(byte variant, byte major, byte minor, byte patch)
        {
            return (uint)(((variant) << 29) | ((major) << 22) | ((minor) << 12) | (patch));
        }

        public static readonly uint VkApiVersion10 = MakeApiVersion(0, 1, 0, 0);

        public static readonly uint VkApiVersion11 = MakeApiVersion(0, 1, 1, 0);

        public static readonly uint VkApiVersion12 = MakeApiVersion(0, 1, 2, 0);

        public static readonly uint VkApiVersion13 = MakeApiVersion(0, 1, 3, 0);
    }
}