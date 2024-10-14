namespace HexaGen
{
    using System.Collections.Generic;

    public static class CollectionHelper
    {
        public static void AddRange<T>(this HashSet<T> destination, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                destination.Add(value);
            }
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> destination, Dictionary<TKey, TValue> values) where TKey : notnull
        {
            foreach (var pair in values)
            {
                destination[pair.Key] = pair.Value;
            }
        }
    }
}