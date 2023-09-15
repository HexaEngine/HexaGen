namespace HexaGen.Core
{
    using System.Collections.Generic;

    //
    // Summary:
    //     Supports cloning, which creates a new instance of a class with the same value
    //     as an existing instance.
    public interface ICloneable<T>
    {
        //
        // Summary:
        //     Creates a new object that is a copy of the current instance.
        //
        // Returns:
        //     A new object that is a copy of this instance.
        T Clone();
    }

    public static class Extensions
    {
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dictionary) where TKey : notnull
        {
            return new(dictionary);
        }

        public static List<T> Clone<T>(this List<T> list)
        {
            return new(list);
        }

        public static List<T> CloneValues<T>(this List<T> list) where T : ICloneable<T>
        {
            return new(list.Select(x => x.Clone()));
        }
    }
}