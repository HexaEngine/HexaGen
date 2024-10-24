namespace HexaGen.Core
{
    using System.Diagnostics.CodeAnalysis;

    public class IdentifierComparer<T> : IEqualityComparer<T> where T : class, IHasIdentifier
    {
        public static readonly IdentifierComparer<T> Default = new();

        public bool Equals(T? x, T? y)
        {
            return x?.Identifier == y?.Identifier;
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return obj.Identifier.GetHashCode();
        }
    }
}