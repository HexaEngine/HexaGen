namespace HexaGen.Core
{
    using System;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;

    public class TrieStringSet : ICollection<string>
    {
        private readonly IEqualityComparer<char> _comparer;
        private readonly TrieSetNode root;
        private int count;

        public TrieStringSet()
        {
            _comparer = EqualityComparer<char>.Default;
            root = new('\0', _comparer);
        }

        public TrieStringSet(IEqualityComparer<char> comparer)
        {
            _comparer = comparer;
            root = new('\0', _comparer);
        }

        public class TrieSetNode
        {
            public readonly char Key;
            public TrieSetNode? Parent;
            public Dictionary<char, TrieSetNode> Children;
            public bool IsTerminal;

            public TrieSetNode(char key, IEqualityComparer<char> comparer)
            {
                Key = key;
                Children = new(comparer);
            }

            public TrieSetNode(char key, TrieSetNode parent, IEqualityComparer<char> comparer)
            {
                Key = key;
                Parent = parent;
                Children = new(comparer);
            }
        }

        public int Count => count;

        bool ICollection<string>.IsReadOnly => false;

        public IEnumerator<string> GetEnumerator()
        {
            return GetAllNodes(root).Select(GetFullKey).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string key)
        {
            TrieSetNode node = root;
            for (int i = 0; i < key.Length; i++)
            {
                node = AddItem(node, key[i]);
            }

            if (node.IsTerminal)
            {
                throw new InvalidOperationException("Key is already in the list");
            }

            node.IsTerminal = true;
            count++;
        }

        public void AddRange(IEnumerable<string> keys)
        {
            foreach (string key in keys)
            {
                Add(key);
            }
        }

        public TrieSetNode AddItem(TrieSetNode node, char key)
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

        public bool Contains(string key)
        {
            var node = GetNode(key);

            return node != null && node.IsTerminal;
        }

        public void CopyTo(string[] array, int arrayIndex) => Array.Copy(GetAllNodes(root).Select(GetFullKey).ToArray(), 0, array, arrayIndex, Count);

        public bool Remove(string key)
        {
            TrieSetNode? node = GetNode(key);

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

        public TrieSetNode? GetNode(string key)
        {
            var node = root;

            for (int i = 0; i < key.Length; i++)
            {
                char item = key[i];
                if (!node.Children.TryGetValue(item, out node))
                {
                    return null;
                }
            }

            return node;
        }

        public bool TryGetNode(string key, [NotNullWhen(true)] out TrieSetNode? node)
        {
            node = GetNode(key);

            return node != null && node.IsTerminal;
        }

        public void RemoveNode(TrieSetNode node)
        {
            Remove(node);
            count--;
        }

        public void Remove(TrieSetNode node)
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

        public void Remove(TrieSetNode node, char key)
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

        public ReadOnlySpan<char> FindLargestMatch(ReadOnlySpan<char> match)
        {
            if (match.Length == 0)
            {
                return match;
            }

            int lastMatch = 0;
            TrieSetNode last = root;
            for (int i = 0; i < match.Length; i++)
            {
                if (last.Children.TryGetValue(match[i], out var child))
                {
                    last = child;
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

        public ReadOnlySpan<char> FindSmallestMatch(ReadOnlySpan<char> match)
        {
            if (match.Length == 0)
            {
                return match;
            }

            TrieSetNode last = root;
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

        private static string GetFullKey(TrieSetNode node)
        {
            StringBuilder sb = new();

            var n = node;

            while (n != null && n.Parent != null)
            {
                sb.Append(n.Key);
                n = n.Parent;
            }

            sb.Reverse();

            return sb.ToString();
        }

        private static IEnumerable<TrieSetNode> GetAllNodes(TrieSetNode node)
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
    }
}