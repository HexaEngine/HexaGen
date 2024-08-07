namespace HexaGen
{
    using System;
    using System.Linq;

    public static class TextExtensions
    {
        public static bool TryTrimStartFirstOccurrence(this ReadOnlySpan<char> text, char trim, out ReadOnlySpan<char> result)
        {
            text = text.TrimStart(); // remove leading whitespace

            if (text.Length < 1)
            {
                result = text;
                return false;
            }

            if (text[0] != trim)
            {
                result = text;
                return false;
            }

            result = text[1..];
            return true;
        }

        public static bool TryTrimEndFirstOccurrence(this ReadOnlySpan<char> text, char trim, out ReadOnlySpan<char> result)
        {
            text = text.TrimEnd(); // remove trailing whitespace

            if (text.Length < 1)
            {
                result = text;
                return false;
            }

            if (text[^1] != trim)
            {
                result = text;
                return false;
            }

            result = text[..^1];
            return true;
        }

        public static bool TryTrimStartFirstOccurrence(this ReadOnlySpan<char> text, ReadOnlySpan<char> trim, out ReadOnlySpan<char> result)
        {
            text = text.TrimStart(); // remove leading whitespace

            if (text.Length < trim.Length)
            {
                result = text;
                return false;
            }

            if (!text[..trim.Length].SequenceEqual(trim))
            {
                result = text;
                return false;
            }

            result = text[trim.Length..];
            return true;
        }

        public static bool TryTrimEndFirstOccurrence(this ReadOnlySpan<char> text, ReadOnlySpan<char> trim, out ReadOnlySpan<char> result)
        {
            text = text.TrimEnd(); // remove trailing whitespace

            if (text.Length < trim.Length)
            {
                result = text;
                return false;
            }

            if (!text[^trim.Length..].SequenceEqual(trim))
            {
                result = text;
                return false;
            }

            result = text[..^trim.Length];
            return true;
        }

        public static ReadOnlySpan<char> TrimStartFirstOccurrence(this ReadOnlySpan<char> text, char trim)
        {
            text = text.TrimStart(); // remove leading whitespace

            if (text.Length < 1)
            {
                return text;
            }

            if (text[0] != trim)
            {
                return text;
            }

            return text[1..];
        }

        public static ReadOnlySpan<char> TrimEndFirstOccurrence(this ReadOnlySpan<char> text, char trim)
        {
            text = text.TrimEnd(); // remove trailing whitespace

            if (text.Length < 1)
            {
                return text;
            }

            if (text[^1] != trim)
            {
                return text;
            }

            return text[..^1];
        }

        public static ReadOnlySpan<char> TrimStartFirstOccurrence(this ReadOnlySpan<char> text, ReadOnlySpan<char> trim)
        {
            text = text.TrimStart(); // remove leading whitespace

            if (text.Length < trim.Length)
            {
                return text;
            }

            if (!text[..trim.Length].SequenceEqual(trim))
            {
                return text;
            }

            return text[trim.Length..];
        }

        public static ReadOnlySpan<char> TrimEndFirstOccurrence(this ReadOnlySpan<char> text, ReadOnlySpan<char> trim)
        {
            text = text.TrimEnd(); // remove trailing whitespace

            if (text.Length < trim.Length)
            {
                return text;
            }

            if (!text[^trim.Length..].SequenceEqual(trim))
            {
                return text;
            }

            return text[..^trim.Length];
        }
    }
}