namespace HexaGen.Language
{
    using HexaGen;
    using System;

    public struct Token : IEquatable<Token>
    {
        public TokenType Type;
        public LiteralType LiteralType;
        public NumberType NumberType;
        public KeywordType KeywordType;
        public int Start;
        public int Length;
        public string Source;
        public SourceLocation Location;

        public Token(TokenType type, int start, int length, string source)
        {
            Type = type;
            Start = start;
            Length = length;
            Source = source;
            Location = new();
        }

        public Token(TokenType type, int start, int length, string source, SourceLocation location)
        {
            Type = type;
            Start = start;
            Length = length;
            Source = source;
            Location = location;
        }

        public Token(int start, int length, string source, SourceLocation location, LiteralType literalType)
        {
            Type = TokenType.Literal;
            Start = start;
            Length = length;
            Source = source;
            Location = location;
            LiteralType = literalType;
        }

        public Token(int start, int length, string source, SourceLocation location, NumberType numberType)
        {
            Type = TokenType.Literal;
            Start = start;
            Length = length;
            Source = source;
            Location = location;
            LiteralType = LiteralType.Number;
            NumberType = numberType;
        }

        public Token(int start, int length, string source, SourceLocation location, KeywordType keywordType)
        {
            Type = TokenType.Keyword;
            Start = start;
            Length = length;
            Source = source;
            Location = location;
            KeywordType = keywordType;
        }

        public readonly bool IsIdentifier => Type == TokenType.Identifier;

        public readonly bool IsKeyword => Type == TokenType.Keyword;

        public readonly bool IsPunctuation => Type == TokenType.Punctuation;

        public readonly bool IsOperator => Type == TokenType.Operator;

        public readonly bool IsLiteral => Type == TokenType.Literal;

        public readonly bool IsComment => Type == TokenType.Comment;

        public readonly ReadOnlySpan<char> Span => Source.AsSpan(Start, Length);

        public readonly string AsString()
        {
            return Span.ToString();
        }

        public readonly bool IsString(string other)
        {
            if (other.Length != Length) return false;
            for (int i = 0; i < Length; i++)
            {
                if (Span[i] != other[i])
                {
                    return false;
                }
            }
            return true;
        }

        public readonly bool IsChar(char other)
        {
            if (1 != Length) return false;

            return Span[0] == other;
        }

        public override readonly bool Equals(object? obj)
        {
            return obj is Token token && Equals(token);
        }

        public readonly bool Equals(Token other)
        {
            return Type == other.Type &&
                   Start == other.Start &&
                   Length == other.Length &&
                   Source == other.Source;
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Type, Start, Length, Source);
        }

        public override readonly string ToString()
        {
            return $"{Type} \t {(Span[0] == '\n' ? "<newline>" : new(Span))}";
        }

        public static bool operator ==(Token left, Token right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Token left, Token right)
        {
            return !(left == right);
        }

        public static bool operator ==(Token left, string right)
        {
            return left.IsString(right);
        }

        public static bool operator !=(Token left, string right)
        {
            return !(left == right);
        }

        public static bool operator ==(Token left, char right)
        {
            return left.IsChar(right);
        }

        public static bool operator !=(Token left, char right)
        {
            return !(left == right);
        }

        public static bool operator ==(Token left, KeywordType right)
        {
            return left.KeywordType == right;
        }

        public static bool operator !=(Token left, KeywordType right)
        {
            return !(left == right);
        }
    }
}