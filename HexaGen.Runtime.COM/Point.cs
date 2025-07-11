namespace HexaGen.Runtime.COM
{
    using System;

    public struct Point32 : IEquatable<Point32>
    {
        public uint X;
        public uint Y;

        public Point32(uint x, uint y)
        {
            X = x;
            Y = y;
        }

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
#if NET5_0_OR_GREATER
            return HashCode.Combine(X, Y);
#else
            return X.GetHashCode() ^ Y.GetHashCode();
#endif
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
}