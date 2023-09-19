namespace HexaGen.Core
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;

    public class TrieSet<T> : ICollection<IEnumerable<T>> where T : notnull
    {
        private readonly IEqualityComparer<T> _comparer;
        private readonly TrieNode root;
        private int count;

        public TrieSet()
        {
            _comparer = EqualityComparer<T>.Default;
            root = new(default, _comparer);
        }

        public TrieSet(IEqualityComparer<T> comparer)
        {
            _comparer = comparer;
            root = new(default, _comparer);
        }

        public class TrieNode
        {
            public readonly T Key;
            public IEnumerable<T>? Value;
            public TrieNode? Parent;
            public Dictionary<T, TrieNode> Children;
            public bool IsTerminal;

            public TrieNode(T key, IEqualityComparer<T> comparer)
            {
                Key = key;
                Children = new(comparer);
            }

            public TrieNode(T key, TrieNode parent, IEqualityComparer<T> comparer)
            {
                Key = key;
                Parent = parent;
                Children = new(comparer);
            }
        }

        public int Count => count;

        bool ICollection<IEnumerable<T>>.IsReadOnly => false;

        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            return GetAllNodes(root).Select(GetFullKey).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IEnumerable<T> value)
        {
            TrieNode node = root;
            foreach (var key in value)
            {
                node = AddItem(node, key);
            }

            if (node.IsTerminal)
            {
                throw new InvalidOperationException("Key is already in the list");
            }

            node.IsTerminal = true;
            node.Value = value;
            count++;
        }

        public void AddRange(IEnumerable<IEnumerable<T>> values)
        {
            foreach (IEnumerable<T> key in values)
            {
                Add(key);
            }
        }

        public TrieNode AddItem(TrieNode node, T key)
        {
            if (!node.Children.TryGetValue(key, out var child))
            {
                child = new(key, node, _comparer);

                node.Children.Add(key, child);
            }

            return child;
        }

        public void Clear()
        {
            root.Children.Clear();
            count = 0;
        }

        public bool Contains(IEnumerable<T> key)
        {
            var node = GetNode(key);

            return node != null && node.IsTerminal;
        }

        public void CopyTo(IEnumerable<T>[] array, int arrayIndex) => Array.Copy(GetAllNodes(root).Select(GetFullKey).ToArray(), 0, array, arrayIndex, Count);

        public bool Remove(IEnumerable<T> key)
        {
            TrieNode? node = GetNode(key);

            if (node == null)
            {
                return false;
            }

            if (!node.IsTerminal)
            {
                return false;
            }

            RemoveNode(node);

            return true;
        }

        public TrieNode? GetNode(IEnumerable<T> key)
        {
            var node = root;

            foreach (T item in key)
            {
                if (!node.Children.TryGetValue(item, out node))
                {
                    return null;
                }
            }

            return node;
        }

        public bool TryGetNode(IEnumerable<T> key, [NotNullWhen(true)] out TrieNode? node)
        {
            node = GetNode(key);

            return node != null && node.IsTerminal;
        }

        public void RemoveNode(TrieNode node)
        {
            Remove(node);
            count--;
        }

        public void Remove(TrieNode node)
        {
            while (true)
            {
                node.IsTerminal = false;

                if (node.Children.Count == 0 && node.Parent != null)
                {
                    Remove(node.Parent, node.Key);

                    if (!node.Parent.IsTerminal)
                    {
                        node = node.Parent;
                        continue;
                    }
                }

                break;
            }
        }

        public void Remove(TrieNode node, T key)
        {
            foreach (var trieNode in node.Children)
            {
                if (_comparer.Equals(key, trieNode.Key))
                {
                    node.Children.Remove(trieNode.Key);
                    return;
                }
            }
        }

        public ReadOnlySpan<T> FindLargestMatch(ReadOnlySpan<T> match)
        {
            if (match.Length == 0)
            {
                return match;
            }

            int lastMatch = 0;
            TrieNode last = root;
            for (int i = 0; i < match.Length; i++)
            {
                var c = match[i];

                if (last.Children.TryGetValue(c, out var child))
                {
                    if (child.IsTerminal)
                    {
                        lastMatch = i;
                    }
                }
                else
                {
                    break;
                }
            }

            return match[..(lastMatch + 1)];
        }

        public ReadOnlySpan<T> FindSmallestMatch(ReadOnlySpan<T> match)
        {
            if (match.Length == 0)
            {
                return match;
            }

            TrieNode last = root;
            for (int i = 0; i < match.Length; i++)
            {
                var c = match[i];

                if (last.Children.TryGetValue(c, out var child))
                {
                    if (child.IsTerminal)
                    {
                        return match[..(i + 1)];
                    }
                }
                else
                {
                    break;
                }
            }

            return match[..0];
        }

        /// <summary>
        /// Gets items by key prefix.
        /// </summary>
        /// <param name="prefix">Key prefix.</param>
        /// <returns>Collection of <see cref="T"/> items.</returns>
        public IEnumerable<IEnumerable<T>> GetByPrefix(IEnumerable<T> prefix)
        {
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            var node = root;

            foreach (var item in prefix)
            {
                if (!node.Children.TryGetValue(item, out node))
                {
                    return Enumerable.Empty<IEnumerable<T>>();
                }
            }

            return GetByPrefix(node);
        }

        private static IEnumerable<T> GetFullKey(TrieNode node)
        {
            return node.Value;
        }

        private static IEnumerable<TrieNode> GetAllNodes(TrieNode node)
        {
            foreach (var child in node.Children)
            {
                if (child.Value.IsTerminal)
                {
                    yield return child.Value;
                }

                foreach (var item in GetAllNodes(child.Value))
                {
                    if (item.IsTerminal)
                    {
                        yield return item;
                    }
                }
            }
        }

        private static IEnumerable<IEnumerable<T>> GetByPrefix(TrieNode node)
        {
            var stack = new Stack<TrieNode>();
            var current = node;

            while (stack.Count > 0 || current != null)
            {
                if (current != null)
                {
                    if (current.IsTerminal)
                    {
                        yield return GetFullKey(current);
                    }

                    using (var enumerator = current.Children.GetEnumerator())
                    {
                        current = enumerator.MoveNext() ? enumerator.Current.Value : null;

                        while (enumerator.MoveNext())
                        {
                            stack.Push(enumerator.Current.Value);
                        }
                    }
                }
                else
                {
                    current = stack.Pop();
                }
            }
        }
    }
}