namespace Hexa.NET.Vulkan
{
    using HexaGen.Runtime;
    using System.Runtime.InteropServices;

    [NativeName(NativeNameType.StructOrClass, "VkAccelerationStructureGeometryTrianglesDataKHR")]
    [StructLayout(LayoutKind.Sequential)]
    public partial struct VkAccelerationStructureGeometryTrianglesDataKHR
    {
        [NativeName(NativeNameType.Field, "sType")]
        [NativeName(NativeNameType.Type, "VkStructureType")]
        public VkStructureType SType;

        [NativeName(NativeNameType.Field, "pNext")]
        [NativeName(NativeNameType.Type, "const void*")]
        public unsafe void* PNext;

        [NativeName(NativeNameType.Field, "vertexFormat")]
        [NativeName(NativeNameType.Type, "VkFormat")]
        public VkFormat VertexFormat;

        [NativeName(NativeNameType.Field, "vertexData")]
        [NativeName(NativeNameType.Type, "VkDeviceOrHostAddressConstKHR")]
        public VkDeviceOrHostAddressConstKHR VertexData;

        [NativeName(NativeNameType.Field, "vertexStride")]
        [NativeName(NativeNameType.Type, "VkDeviceSize")]
        public ulong VertexStride;

        [NativeName(NativeNameType.Field, "maxVertex")]
        [NativeName(NativeNameType.Type, "uint32_t")]
        public uint MaxVertex;

        [NativeName(NativeNameType.Field, "indexType")]
        [NativeName(NativeNameType.Type, "VkIndexType")]
        public VkIndexType IndexType;

        [NativeName(NativeNameType.Field, "indexData")]
        [NativeName(NativeNameType.Type, "VkDeviceOrHostAddressConstKHR")]
        public VkDeviceOrHostAddressConstKHR IndexData;

        [NativeName(NativeNameType.Field, "transformData")]
        [NativeName(NativeNameType.Type, "VkDeviceOrHostAddressConstKHR")]
        public VkDeviceOrHostAddressConstKHR TransformData;

        public unsafe VkAccelerationStructureGeometryTrianglesDataKHR(VkStructureType sType = default, void* pNext = default, VkFormat vertexFormat = default, VkDeviceOrHostAddressConstKHR vertexData = default, ulong vertexStride = default, uint maxVertex = default, VkIndexType indexType = default, VkDeviceOrHostAddressConstKHR indexData = default, VkDeviceOrHostAddressConstKHR transformData = default)
        {
            SType = sType;
            PNext = pNext;
            VertexFormat = vertexFormat;
            VertexData = vertexData;
            VertexStride = vertexStride;
            MaxVertex = maxVertex;
            IndexType = indexType;
            IndexData = indexData;
            TransformData = transformData;
        }
    }
}