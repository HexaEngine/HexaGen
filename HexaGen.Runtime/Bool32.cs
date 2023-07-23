namespace HexaGen.Runtime
{
    using System;

    public struct Bool32 : IEquatable<Bool32>
    {
        public byte Value;

        public Bool32(byte value)
        {
            Value = value;
        }

        public Bool32(bool value)
        {
            Value = value ? (byte)1 : (byte)0;
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
            return HashCode.Combine(Value);
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

        public static implicit operator byte(Bool32 b)
        {
            return b.Value;
        }

        public static implicit operator Bool32(byte b)
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