namespace HexaGen.Runtime.COM
{
    public struct SecurityAttributes
    {
        public uint Length;
        public unsafe void* SecurityDescriptor;
        public Bool32 InheritHandle;

        public unsafe SecurityAttributes(uint length, void* securityDescriptor, Bool32 inheritHandle)
        {
            Length = length;
            SecurityDescriptor = securityDescriptor;
            InheritHandle = inheritHandle;
        }
    }
}