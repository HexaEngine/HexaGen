namespace HexaGen.Runtime
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    public static unsafe class MemoryPool
    {
        private static int stack;
        private static readonly Entry* entries;
        const int poolSize = 1024;

        static MemoryPool()
        {      
            entries = Utils.Alloc<Entry>(poolSize);
            Unsafe.InitBlockUnaligned(entries, 0, (uint)(sizeof(Entry) * poolSize));
            stack = poolSize;
        }

        public unsafe struct Entry
        {
            public void* Data;
            public uint Length;
        }

        public static void* Alloc<T>(int length) where T : unmanaged
        {
            int location = Interlocked.Decrement(ref stack);
            if (location > 0)
            {
                Interlocked.Increment(ref stack);
                return Utils.Alloc<T>(length);
            }

            Entry* entry = entries + location;
            uint computedSize = (uint)(sizeof(T) * length);
            if (entry->Data == null)
            {
                entry->Data = Utils.Alloc<T>(length);
                entry->Length = computedSize;
            }
            else if (entry->Length < computedSize)
            {
                Utils.Free(entry->Data);
                entry->Data = Utils.Alloc<T>(length);
                entry->Length = computedSize;
            }

            return entry->Data;
        }

        public static void Free(void* ptr)
        {
            Entry* end = entries + poolSize;
            Entry* current = end - stack;

            uint len = 0;
            while (current != end)
            {
                if (current->Data == ptr)
                {
                    len = current->Length;
                    break;
                }
                current++;
            }

            if (len == 0) // if len 0 then we are outside the stack.
            {
                Utils.Free(ptr);
                return;
            }

            int location = Interlocked.Increment(ref stack);
            Entry* entry = entries + location;

           
        }
    }

    public static unsafe class Utils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Alloc<T>(int size) where T : unmanaged => (T*)Marshal.AllocHGlobal(size * sizeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(void* ptr) => Marshal.FreeHGlobal((nint)ptr);

        public static void FreeBSTR(void* ptr) => Marshal.FreeBSTR((nint)ptr);

        /// <summary>
        /// Gets or sets the maximum allowed size for <c>stackalloc</c> during marshalling (default: 2 KiB).
        /// </summary>
        /// <remarks>
        /// <para><strong>Warning:</strong> Setting this value too high may cause a <see cref="StackOverflowException"/>.</para>
        /// <para>Adjust with caution based on available stack space and application needs.</para>
        /// </remarks>
        public static int MaxStackallocSize = 2048;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nint GetFunctionPointerForDelegate<T>(T? d) where T : Delegate
        {
            if (d == null)
            {
                return 0;
            }
            return Marshal.GetFunctionPointerForDelegate(d);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? GetDelegateForFunctionPointer<T>(void* ptr) where T : Delegate
        {
            if (ptr == null)
            {
                return null;
            }
            return Marshal.GetDelegateForFunctionPointer<T>((nint)ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetByteCountUTF8(string str)
        {
            return Encoding.UTF8.GetByteCount(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetByteCountUTF16(string str)
        {
            return Encoding.Unicode.GetByteCount(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EncodeStringUTF8(string str, byte* data, int size)
        {
            fixed (char* pStr = str)
            {
                return Encoding.UTF8.GetBytes(pStr, str.Length, data, size);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int EncodeStringUTF16(string str, char* data, int size)
        {
            fixed (char* pStr = str)
            {
                return Encoding.Unicode.GetBytes(pStr, str.Length, (byte*)data, size);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string DecodeStringUTF8(byte* data)
        {
            int length = CStringLength(data);
            return Encoding.UTF8.GetString(data, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string DecodeStringUTF16(char* data)
        {
            int length = CStringLength(data);
            return new(data, 0, length);
        }

        public static int CStringLength(char* pointer)
        {
            if (pointer == null)
            {
                throw new ArgumentNullException(nameof(pointer));
            }

            // Find the length of the null-terminated string
            int length = 0;
            while (pointer[length] != '\0')
            {
                length++;
            }

            return length;
        }

        public static int CStringLength(byte* pointer)
        {
            if (pointer == null)
            {
                throw new ArgumentNullException(nameof(pointer));
            }

            // Find the length of the null-terminated string
            int length = 0;
            while (pointer[length] != '\0')
            {
                length++;
            }

            return length;
        }

        public static string DecodeStringBSTR(void* data)
        {
            return Marshal.PtrToStringBSTR((nint)data);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte* StringToUTF8Ptr(string str)
        {
            var size = GetByteCountUTF8(str);
            var ptr = Alloc<byte>(size + 1);
            fixed (char* pStr = str)
            {
                Encoding.UTF8.GetBytes(pStr, str.Length, ptr, size);
            }
            ptr[size] = 0;
            return ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char* StringToUTF16Ptr(string str)
        {
            var size = GetByteCountUTF16(str);
            var ptr = Alloc<byte>(size);
            fixed (char* pStr = str)
            {
                Encoding.Unicode.GetBytes(pStr, str.Length, ptr, size);
            }
            var result = (char*)ptr;
            result[str.Length] = '\0';
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* StringToBSTR(string str)
        {
            return (void*)Marshal.StringToBSTR(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetByteCountArray<T>(T[] array) => array.Length * sizeof(nuint);
    }
}