namespace HexaGen.Runtime
{
    using System;

    public struct Bool8 : IEquatable<Bool8>
    {
        public byte Value;

        public Bool8(byte value)
        {
            Value = value;
        }

        public Bool8(bool value)
        {
            Value = value ? (byte)1 : (byte)0;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Bool8 @bool && Equals(@bool);
        }

        public readonly bool Equals(Bool8 other)
        {
            return Value == other.Value;
        }

        public override readonly int GetHashCode()
        {
            return Value;
        }

        public static bool operator ==(Bool8 left, Bool8 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Bool8 left, Bool8 right)
        {
            return !(left == right);
        }

        public static implicit operator bool(Bool8 b)
        {
            return b.Value != 0;
        }

        public static implicit operator byte(Bool8 b)
        {
            return b.Value;
        }

        public static implicit operator Bool8(byte b)
        {
            return new(b);
        }

        public static implicit operator Bool8(bool b)
        {
            return new(b);
        }

        public override readonly string ToString()
        {
            return Value == 0 ? "false" : "true";
        }
    }
}