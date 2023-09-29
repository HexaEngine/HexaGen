namespace HexaGen.Core
{
    using System.Collections.Generic;
    using System.Text;

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
        public static void Reverse(this StringBuilder sb)
        {
            char t;
            int end = sb.Length - 1;
            int start = 0;

            while (end - start > 0)
            {
                t = sb[end];
                sb[end] = sb[start];
                sb[start] = t;
                start++;
                end--;
            }
        }

        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dictionary) where TKey : notnull
        {
            return new(dictionary);
        }

        public static List<T> Clone<T>(this List<T> list)
        {
            return new(list);
        }

        public static List<T> CloneValues<T>(this IList<T> list) where T : ICloneable<T>
        {
            return new(list.Select(x => x.Clone()));
        }
    }
}