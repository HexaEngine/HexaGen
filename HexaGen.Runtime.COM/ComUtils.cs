namespace HexaGen.Runtime.COM
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class ComUtils
    {
        private static class TypeGuid<T>
        {
            public static readonly unsafe Guid* Riid = CreateRiid();

#if NET5_0_OR_GREATER

            private static unsafe Guid* CreateRiid()
            {
                Guid* ptr = (Guid*)(void*)RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(T), sizeof(Guid));
                *ptr = typeof(T)!.GUID;
                return ptr;
            }

#else

            private static unsafe Guid* CreateRiid()
            {
                Guid* ptr = (Guid*)Marshal.AllocHGlobal(sizeof(Guid));
                *ptr = typeof(T).GUID;
                return ptr;
            }

            unsafe static TypeGuid()
            {
                AppDomain.CurrentDomain.ProcessExit += (sender, e) =>
                {
                    Marshal.FreeHGlobal((nint)Riid);
                };
            }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ref Guid GuidOf<T>()
        {
            return ref *TypeGuid<T>.Riid;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe Guid* GuidPtrOf<T>()
        {
            return TypeGuid<T>.Riid;
        }
    }
}