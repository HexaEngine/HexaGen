namespace HexaGen.Metadata
{
    using HexaGen.Core;
    using HexaGen.Core.Collections;
    using System.Collections;

    public class MetadataListEntry<T> : GeneratorMetadataEntry, IList<T>
    {
        private readonly List<T> values = [];

        public MetadataListEntry()
        {
        }

        public MetadataListEntry(IEnumerable<T> values)
        {
            this.values.AddRange(values);
        }

        public T this[int index] { get => ((IList<T>)values)[index]; set => ((IList<T>)values)[index] = value; }

        public int Count => ((ICollection<T>)values).Count;

        public bool IsReadOnly => ((ICollection<T>)values).IsReadOnly;

        public List<T> Values => values;

        public void Add(T item)
        {
            ((ICollection<T>)values).Add(item);
        }

        public void Clear()
        {
            ((ICollection<T>)values).Clear();
        }

        public override GeneratorMetadataEntry Clone()
        {
            if (typeof(T).IsAssignableTo(typeof(ICloneable<T>)))
            {
                return new MetadataListEntry<T>(values.Select(static x => ((ICloneable<T>)x!).Clone()));
            }
            return new MetadataListEntry<T>(values);
        }

        public bool Contains(T item)
        {
            return ((ICollection<T>)values).Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)values).CopyTo(array, arrayIndex);
        }

        public void CopyTo(List<T> other)
        {
            other.AddRange(values);
        }

        public void CopyTo(HashSet<T> other)
        {
            other.AddRange(values);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)values).GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return ((IList<T>)values).IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            ((IList<T>)values).Insert(index, item);
        }

        public override void Merge(GeneratorMetadataEntry from, in MergeOptions options)
        {
            if (from is MetadataListEntry<T> list)
            {
                values.AddRange(list);
            }
        }

        public bool Remove(T item)
        {
            return ((ICollection<T>)values).Remove(item);
        }

        public void RemoveAt(int index)
        {
            ((IList<T>)values).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)values).GetEnumerator();
        }
    }
}