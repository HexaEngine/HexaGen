namespace HexaGen.Runtime
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Iterator<T> where T : unmanaged
    {
        public T* ptr;
        public nuint index;

        public Iterator(T* ptr, nuint index = 0)
        {
            this.ptr = ptr;
            this.index = index;
        }

        public T* Current => ptr + index;

        public void MoveNext()
        {
            index++;
        }

        public readonly bool Equals(Iterator<T> other)
        {
            return ptr == other.ptr && index == other.index;
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj is Iterator<T> other)
            {
                return Equals(other);
            }
            return false;
        }

        public override readonly int GetHashCode()
        {
            return ((IntPtr)ptr).GetHashCode() ^ index.GetHashCode();
        }

        public static bool operator ==(Iterator<T> left, Iterator<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Iterator<T> left, Iterator<T> right)
        {
            return !(left == right);
        }
    }
}