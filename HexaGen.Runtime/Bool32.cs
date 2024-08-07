namespace HexaGen.Runtime
{
    using System;

    public struct Bool32 : IEquatable<Bool32>
    {
        public int Value;

        public Bool32(int value)
        {
            Value = value;
        }

        public Bool32(bool value)
        {
            Value = value ? 1 : 0;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Bool32 @bool && Equals(@bool);
        }

        public readonly bool Equals(Bool32 other)
        {
            return Value == other.Value;
        }

        public override readonly int GetHashCode()
        {
            return Value;
        }

        public static bool operator ==(Bool32 left, Bool32 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Bool32 left, Bool32 right)
        {
            return !(left == right);
        }

        public static implicit operator bool(Bool32 b)
        {
            return b.Value != 0;
        }

        public static implicit operator int(Bool32 b)
        {
            return b.Value;
        }

        public static implicit operator Bool32(int b)
        {
            return new(b);
        }

        public static implicit operator Bool32(bool b)
        {
            return new(b);
        }

        public override readonly string ToString()
        {
            return Value == 0 ? "false" : "true";
        }
    }
}