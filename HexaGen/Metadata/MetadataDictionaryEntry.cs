namespace HexaGen.Metadata
{
    using HexaGen.Core.Collections;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;

    public class MetadataDictionaryEntry<TKey, TValue> : GeneratorMetadataEntry, IDictionary<TKey, TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, TValue> dictionary = [];

        public MetadataDictionaryEntry()
        {
        }

        public MetadataDictionaryEntry(Dictionary<TKey, TValue> other)
        {
            dictionary.AddRange(other);
        }

        public TValue this[TKey key] { get => ((IDictionary<TKey, TValue>)dictionary)[key]; set => ((IDictionary<TKey, TValue>)dictionary)[key] = value; }

        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)dictionary).Keys;

        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)dictionary).Values;

        public Dictionary<TKey, TValue> Dictionary => dictionary;

        public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Count;

        public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).IsReadOnly;

        public void Add(TKey key, TValue value)
        {
            ((IDictionary<TKey, TValue>)dictionary).Add(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Add(item);
        }

        public void Clear()
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Clear();
        }

        public override GeneratorMetadataEntry Clone()
        {
            return new MetadataDictionaryEntry<TKey, TValue>(dictionary);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Contains(item);
        }

        public bool ContainsKey(TKey key)
        {
            return ((IDictionary<TKey, TValue>)dictionary).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).CopyTo(array, arrayIndex);
        }

        public void CopyTo(Dictionary<TKey, TValue> other)
        {
            other.AddRange(dictionary);
        }

        public void CopyFrom(Dictionary<TKey, TValue> other)
        {
            dictionary.AddRange(other);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)dictionary).GetEnumerator();
        }

        public override void Merge(GeneratorMetadataEntry from, in MergeOptions options)
        {
            if (from is MetadataDictionaryEntry<TKey, TValue> dict)
            {
                dictionary.AddRange(dict.dictionary);
            }
        }

        public bool Remove(TKey key)
        {
            return ((IDictionary<TKey, TValue>)dictionary).Remove(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)dictionary).Remove(item);
        }

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return ((IDictionary<TKey, TValue>)dictionary).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)dictionary).GetEnumerator();
        }
    }
}