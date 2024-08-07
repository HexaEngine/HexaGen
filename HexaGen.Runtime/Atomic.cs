#if NET7_0_OR_GREATER
namespace HexaGen.Runtime
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public struct Atomic<T> where T : unmanaged
    {
        public ulong value;

        public Atomic(T value)
        {
            this.value = Unsafe.As<T, ulong>(ref value);
        }

        public T Value
        {
            get { var n = Interlocked.Read(ref value); return Unsafe.As<ulong, T>(ref n); }
            set => Interlocked.Exchange(ref this.value, Unsafe.As<T, ulong>(ref value));
        }

        public T Increment()
        {
            ulong result = (ulong)Interlocked.Increment(ref Unsafe.As<ulong, long>(ref value));
            return Unsafe.As<ulong, T>(ref result);
        }

        public T Decrement()
        {
            ulong result = (ulong)Interlocked.Decrement(ref Unsafe.As<ulong, long>(ref value));
            return Unsafe.As<ulong, T>(ref result);
        }

        public T Add(T amount)
        {
            ulong result = (ulong)Interlocked.Add(ref Unsafe.As<ulong, long>(ref value), Unsafe.As<T, long>(ref amount));
            return Unsafe.As<ulong, T>(ref result);
        }

        public bool CompareAndSwap(T expected, T newValue)
        {
            return Interlocked.CompareExchange(ref value, Unsafe.As<T, ulong>(ref newValue), Unsafe.As<T, ulong>(ref expected)) == Unsafe.As<T, ulong>(ref expected);
        }
    }
}
#endif