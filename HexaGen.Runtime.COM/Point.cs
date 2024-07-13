namespace HexaGen.Runtime.COM
{
    using System;

    public struct Point32 : IEquatable<Point32>
    {
        public uint X;
        public uint Y;

        public override readonly bool Equals(object? obj)
        {
            return obj is Point32 point && Equals(point);
        }

        public readonly bool Equals(Point32 other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Point32 left, Point32 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Point32 left, Point32 right)
        {
            return !(left == right);
        }
    }

    public struct Rect32 : IEquatable<Rect32>
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public override readonly bool Equals(object? obj)
        {
            return obj is Rect32 rect && Equals(rect);
        }

        public readonly bool Equals(Rect32 other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Width == other.Width &&
                   Height == other.Height;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(X, Y, Width, Height);
        }

        public static bool operator ==(Rect32 left, Rect32 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Rect32 left, Rect32 right)
        {
            return !(left == right);
        }
    }

    public struct SecurityAttributes
    {
        public uint Length;
        public unsafe void* SecurityDescriptor;
        public Bool32 InheritHandle;
    }
}