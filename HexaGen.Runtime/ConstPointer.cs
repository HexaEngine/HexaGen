namespace HexaGen.Runtime
{
    using System.Diagnostics;

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public readonly unsafe struct ConstPointer<T> : IEquatable<ConstPointer<T>> where T : unmanaged
    {
        public readonly T* Handle;

        public ConstPointer(T* handle)
        {
            Handle = handle;
        }

        public ConstPointer(nint handle)
        {
            Handle = (T*)handle;
        }

        public ConstPointer(nuint handle)
        {
            Handle = (T*)handle;
        }

        public T this[int index]
        {
            get => Handle[index];
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is ConstPointer<T> pointer && Equals(pointer);
        }

        public readonly bool Equals(ConstPointer<T> other)
        {
            return (nint)Handle == (nint)other.Handle;
        }

        public override readonly int GetHashCode()
        {
            return ((nint)Handle).GetHashCode();
        }

        public static bool operator ==(ConstPointer<T> left, ConstPointer<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ConstPointer<T> left, ConstPointer<T> right)
        {
            return !(left == right);
        }

        public static bool operator ==(ConstPointer<T> left, nint right)
        {
            return (nint)left.Handle == right;
        }

        public static bool operator !=(ConstPointer<T> left, nint right)
        {
            return !(left == right);
        }

        public static bool operator ==(ConstPointer<T> left, nuint right)
        {
            return (nuint)left.Handle == right;
        }

        public static bool operator !=(ConstPointer<T> left, nuint right)
        {
            return !(left == right);
        }

        public static bool operator ==(ConstPointer<T> left, T* right)
        {
            return left.Handle == right;
        }

        public static bool operator !=(ConstPointer<T> left, T* right)
        {
            return !(left == right);
        }

        public static implicit operator T*(ConstPointer<T> pointer)
        {
            return pointer.Handle;
        }

        public static implicit operator ConstPointer<T>(T* pointer)
        {
            return new(pointer);
        }

        public static implicit operator nint(ConstPointer<T> pointer)
        {
            return (nint)pointer.Handle;
        }

        public static implicit operator ConstPointer<T>(nint pointer)
        {
            return new(pointer);
        }

        public static implicit operator nuint(ConstPointer<T> pointer)
        {
            return (nuint)pointer.Handle;
        }

        public static implicit operator ConstPointer<T>(nuint pointer)
        {
            return new(pointer);
        }

        public static ConstPointer<T> operator +(ConstPointer<T> pointer, int offset)
        {
            return new(pointer.Handle + offset);
        }

        public static ConstPointer<T> operator -(ConstPointer<T> pointer, int offset)
        {
            return new(pointer.Handle - offset);
        }

        public static ConstPointer<T> operator ++(ConstPointer<T> pointer)
        {
            return new(pointer.Handle + 1);
        }

        public static ConstPointer<T> operator --(ConstPointer<T> pointer)
        {
            return new(pointer.Handle - 1);
        }

        private readonly string DebuggerDisplay => string.Format("[0x{0}]", ((nint)Handle).ToString("X"));
    }
}