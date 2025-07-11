namespace HexaGen
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Frozen;
    using System;

    public class FileSet : ISet<string>, IReadOnlySet<string>, IReadOnlyCollection<string>, ICollection
    {
        private readonly FrozenSet<string> set;

        public FileSet(FrozenSet<string> set)
        {
            this.set = set;
        }

        public FileSet(IEnumerable<string> set)
        {
            this.set = set.ToFrozenSet();
        }

        public int Count => ((ICollection)set).Count;

        public bool IsSynchronized => ((ICollection)set).IsSynchronized;

        public object SyncRoot => ((ICollection)set).SyncRoot;

        public bool IsReadOnly => ((ICollection<string>)set).IsReadOnly;

        public bool Add(string item)
        {
            return ((ISet<string>)set).Add(item);
        }

        public void Clear()
        {
            ((ICollection<string>)set).Clear();
        }

        public bool Contains(string path)
        {
            if (path == null) return true;
            path = PathHelper.GetPath(path);
            return set.Contains(path);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)set).CopyTo(array, index);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            ((ICollection<string>)set).CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<string> other)
        {
            ((ISet<string>)set).ExceptWith(other);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)set).GetEnumerator();
        }

        public void IntersectWith(IEnumerable<string> other)
        {
            ((ISet<string>)set).IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<string> other)
        {
            return ((ISet<string>)set).IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<string> other)
        {
            return ((ISet<string>)set).IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<string> other)
        {
            return ((ISet<string>)set).IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<string> other)
        {
            return ((ISet<string>)set).IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<string> other)
        {
            return ((ISet<string>)set).Overlaps(other);
        }

        public bool Remove(string item)
        {
            return ((ICollection<string>)set).Remove(item);
        }

        public bool SetEquals(IEnumerable<string> other)
        {
            return ((ISet<string>)set).SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<string> other)
        {
            ((ISet<string>)set).SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<string> other)
        {
            ((ISet<string>)set).UnionWith(other);
        }

        void ICollection<string>.Add(string item)
        {
            ((ICollection<string>)set).Add(item);
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return ((IEnumerable<string>)set).GetEnumerator();
        }
    }
}
