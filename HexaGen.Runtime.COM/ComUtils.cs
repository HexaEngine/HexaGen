namespace HexaGen.Runtime.COM
{
    using System;
    using System.Runtime.CompilerServices;

    public static class ComUtils
    {
        private static class TypeGuid<T>
        {
            public static readonly unsafe Guid* Riid = CreateRiid();

            private static unsafe Guid* CreateRiid()
            {
                Guid* ptr = (Guid*)(void*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(T), sizeof(Guid));
                *ptr = typeof(T)!.GUID;
                return ptr;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe ref Guid GuidOf<T>()
        {
            return ref *TypeGuid<T>.Riid;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe Guid* GuidPtrOf<T>()
        {
            return TypeGuid<T>.Riid;
        }
    }
}