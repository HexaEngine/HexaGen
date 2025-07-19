namespace HexaGen.Runtime
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A utility for accessing bit fields.
    ///
    /// Fast enough™
    ///
    /// .NET 9.0.7 (9.0.725.31616), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
    /// HardwareIntrinsics=AVX-512F+CD+BW+DQ+VL+VBMI,AES,BMI1,BMI2,FMA,LZCNT,PCLMUL,POPCNT VectorSize=256
    ///
    /// | Method | Mean     | Error     | StdDev    |
    /// |------- |---------:|----------:|----------:|
    /// | GetA   | 1.077 ns | 0.0078 ns | 0.0065 ns |
    /// | SetA   | 1.930 ns | 0.0127 ns | 0.0106 ns |
    /// </summary>
    public static unsafe class Bitfield
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ToULong<T>(T value) where T : unmanaged
        {
#if NET8_0_OR_GREATER
            return sizeof(T) switch
            {
                1 => Unsafe.BitCast<T, byte>(value),
                2 => Unsafe.BitCast<T, ushort>(value),
                4 => Unsafe.BitCast<T, uint>(value),
                8 => Unsafe.BitCast<T, ulong>(value),
                _ => throw new Exception($"Type '{typeof(T)} is not supported in bitfields.'"),
            };
#else
            return sizeof(T) switch
            {
                1 => *(byte*)&value,
                2 => *(ushort*)&value,
                4 => *(uint*)&value,
                8 => *(ulong*)&value,
                _ => throw new Exception($"Type '{typeof(T)} is not supported in bitfields.'"),
            };
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>(T raw, int offset, int bitWidth) where T : unmanaged
        {
            ulong rawl = ToULong(raw);
            ulong mask = (1UL << bitWidth) - 1UL;
            ulong value = (rawl >> offset) & mask;
            return *(T*)&value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Set<T>(ref T raw, T value, int offset, int bitWidth) where T : unmanaged
        {
            ulong rawl = ToULong(raw);
            ulong val = ToULong(value);
            ulong mask = ((1UL << bitWidth) - 1UL) << offset;
            var newl = (rawl & ~mask) | ((val & ((1UL << bitWidth) - 1UL)) << offset);
            raw = *(T*)&newl;
        }
    }
}