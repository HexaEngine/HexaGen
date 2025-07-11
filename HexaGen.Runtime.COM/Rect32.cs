namespace HexaGen.Runtime.COM
{
    using System;

    public struct Rect32 : IEquatable<Rect32>
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public Rect32(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Rect32 rect && Equals(rect);
        }

        public readonly bool Equals(Rect32 other)
        {
            return Left == other.Left &&
                   Top == other.Top &&
                   Right == other.Right &&
                   Bottom == other.Bottom;
        }

        public override readonly int GetHashCode()
        {
#if NET5_0_OR_GREATER
            return HashCode.Combine(Left, Top, Right, Bottom);
#else
            return Left.GetHashCode() ^ Top.GetHashCode() ^ Right.GetHashCode() ^ Bottom.GetHashCode();
#endif
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
}