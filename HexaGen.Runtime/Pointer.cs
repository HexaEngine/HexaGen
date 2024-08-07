namespace HexaGen.Runtime
{
    using System.Diagnostics;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly unsafe struct Pointer<T> : IEquatable<Pointer<T>> where T : unmanaged
    {
        public readonly T* Handle;

        public Pointer(T* handle)
        {
            Handle = handle;
        }

        public Pointer(nint handle)
        {
            Handle = (T*)handle;
        }

        public Pointer(nuint handle)
        {
            Handle = (T*)handle;
        }

        public T this[int index]
        {
            get => Handle[index];
            set => Handle[index] = value;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Pointer<T> pointer && Equals(pointer);
        }

        public readonly bool Equals(Pointer<T> other)
        {
            return (nint)Handle == (nint)other.Handle;
        }

        public override readonly int GetHashCode()
        {
            return ((nint)Handle).GetHashCode();
        }

        public static bool operator ==(Pointer<T> left, Pointer<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Pointer<T> left, Pointer<T> right)
        {
            return !(left == right);
        }

        public static bool operator ==(Pointer<T> left, nint right)
        {
            return (nint)left.Handle == right;
        }

        public static bool operator !=(Pointer<T> left, nint right)
        {
            return !(left == right);
        }

        public static bool operator ==(Pointer<T> left, nuint right)
        {
            return (nuint)left.Handle == right;
        }

        public static bool operator !=(Pointer<T> left, nuint right)
        {
            return !(left == right);
        }

        public static bool operator ==(Pointer<T> left, T* right)
        {
            return left.Handle == right;
        }

        public static bool operator !=(Pointer<T> left, T* right)
        {
            return !(left == right);
        }

        public static implicit operator T*(Pointer<T> pointer)
        {
            return pointer.Handle;
        }

        public static implicit operator Pointer<T>(T* pointer)
        {
            return new(pointer);
        }

        public static implicit operator nint(Pointer<T> pointer)
        {
            return (nint)pointer.Handle;
        }

        public static implicit operator Pointer<T>(nint pointer)
        {
            return new(pointer);
        }

        public static implicit operator nuint(Pointer<T> pointer)
        {
            return (nuint)pointer.Handle;
        }

        public static implicit operator Pointer<T>(nuint pointer)
        {
            return new(pointer);
        }

        public static Pointer<T> operator +(Pointer<T> pointer, int offset)
        {
            return new(pointer.Handle + offset);
        }

        public static Pointer<T> operator -(Pointer<T> pointer, int offset)
        {
            return new(pointer.Handle - offset);
        }

        public static Pointer<T> operator ++(Pointer<T> pointer)
        {
            return new(pointer.Handle + 1);
        }

        public static Pointer<T> operator --(Pointer<T> pointer)
        {
            return new(pointer.Handle - 1);
        }

        private readonly string DebuggerDisplay => string.Format("[0x{0}]", ((nint)Handle).ToString("X"));
    }
}