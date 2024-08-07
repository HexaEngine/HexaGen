namespace HexaGen.Runtime
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public struct HResult : IEquatable<HResult>
    {
        public int Value;

        public HResult(int value)
        {
            Value = value;
        }

        public HResult(int severity, int facility, int code)
        {
            Value = Create(severity, facility, code);
        }

        public readonly bool IsSuccess
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return IndicatesSuccess(Value);
            }
        }

        public readonly bool IsFailure
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return IndicatesFailure(Value);
            }
        }

        public readonly bool IsError
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return IndicatesError(Value);
            }
        }

        public readonly int Code
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return GetCode(Value);
            }
        }

        public readonly int Facility
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return GetFacility(Value);
            }
        }

        public readonly int Severity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return GetSeverity(Value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IndicatesSuccess(int hr)
        {
            return hr >= 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IndicatesFailure(int hr)
        {
            return hr < 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IndicatesError(int status)
        {
            return (uint)status >> 31 == 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetCode(int hr)
        {
            return hr & 0xFFFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetFacility(int hr)
        {
            return (hr >> 16) & 0x1FFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSeverity(int hr)
        {
            return (hr >> 31) & 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Create(int severity, int facility, int code)
        {
            return (severity << 31) | (facility << 16) | code;
        }

        public readonly void ThrowIf()
        {
            if (IsFailure)
            {
                Marshal.ThrowExceptionForHR(Value);
            }
        }

        public readonly void Throw()
        {
            Marshal.ThrowExceptionForHR(Value);
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is HResult result && Equals(result);
        }

        public readonly bool Equals(HResult other)
        {
            return Value == other.Value;
        }

        public override readonly int GetHashCode()
        {
            return Value;
        }

        public static implicit operator HResult(int value)
        {
            return new HResult(value);
        }

        public static implicit operator int(HResult value)
        {
            return value.Value;
        }

        public static bool operator ==(HResult left, HResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HResult left, HResult right)
        {
            return !(left == right);
        }
    }
}