namespace HexaGen.Runtime
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    public static unsafe class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static T* Alloc<T>(int size) where T : unmanaged => (T*)Marshal.AllocHGlobal(size * sizeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void Free(void* ptr) => Marshal.FreeHGlobal((nint)ptr);

        public const int MaxStackallocSize = 2048;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int GetByteCountUTF8(string str)
        {
            return Encoding.UTF8.GetByteCount(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int GetByteCountUTF16(string str)
        {
            return Encoding.Unicode.GetByteCount(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int EncodeStringUTF8(string str, byte* data, int size)
        {
            return Encoding.UTF8.GetBytes(str, new Span<byte>(data, size));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int EncodeStringUTF16(string str, char* data, int size)
        {
            return Encoding.Unicode.GetBytes(str, new Span<byte>(data, size));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string DecodeStringUTF8(byte* data)
        {
            return Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(data));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static string DecodeStringUTF16(char* data)
        {
            return new(MemoryMarshal.CreateReadOnlySpanFromNullTerminated(data));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static byte* StringToUTF8Ptr(string str)
        {
            var size = GetByteCountUTF8(str);
            var ptr = Alloc<byte>(size + 1);
            Encoding.UTF8.GetBytes(str, new Span<byte>(ptr, size));
            ptr[size] = 0;
            return ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static char* StringToUTF16Ptr(string str)
        {
            var size = GetByteCountUTF16(str);
            var ptr = Alloc<byte>(size);
            Encoding.Unicode.GetBytes(str, new Span<byte>(ptr, size));
            var result = (char*)ptr;
            result[str.Length] = '\0';
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int GetByteCountArray<T>(T[] array) => array.Length * sizeof(nuint);
    }
}