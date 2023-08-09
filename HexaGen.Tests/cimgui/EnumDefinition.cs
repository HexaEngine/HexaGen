namespace Test
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    internal class EnumDefinition : IEquatable<EnumDefinition?>
    {
        private readonly Dictionary<string, string> _sanitizedNames;

        public string Name { get; }
        public string[] Names { get; }
        public string[] FriendlyNames { get; }
        public EnumMember[] Members { get; }
        public string? Comment { get; }

        public EnumDefinition(string name, EnumMember[] elements, string? comment)
        {
            Name = name;
            if (ImguiDefinitions.AlternateEnumPrefixes.TryGetValue(name, out string? altName))
            {
                Names = new[] { name, altName };
            }
            else
            {
                Names = new[] { name };
            }
            FriendlyNames = new string[Names.Length];
            for (int i = 0; i < Names.Length; i++)
            {
                string n = Names[i];
                if (n.EndsWith('_'))
                {
                    FriendlyNames[i] = n[..^1];
                }
                else
                {
                    FriendlyNames[i] = n;
                }
            }

            Members = elements;

            _sanitizedNames = new Dictionary<string, string>();
            foreach (EnumMember el in elements)
            {
                _sanitizedNames.Add(el.Name, SanitizeMemberName(el.Name));
            }
            Comment = comment;
        }

        public string SanitizeNames(string text)
        {
            foreach (KeyValuePair<string, string> kvp in _sanitizedNames)
            {
                text = text.Replace(kvp.Key, kvp.Value);
            }

            return text;
        }

        private string SanitizeMemberName(string memberName)
        {
            string ret = memberName;
            bool altSubstitution = false;

            // Try alternate substitution first
            foreach (KeyValuePair<string, string> substitutionPair in ImguiDefinitions.AlternateEnumPrefixSubstitutions)
            {
                if (memberName.StartsWith(substitutionPair.Key))
                {
                    ret = ret.Replace(substitutionPair.Key, substitutionPair.Value);
                    altSubstitution = true;
                    break;
                }
            }

            if (!altSubstitution)
            {
                foreach (string name in Names)
                {
                    if (memberName.StartsWith(name))
                    {
                        ret = memberName[name.Length..];
                        if (ret.StartsWith("_"))
                        {
                            ret = ret[1..];
                        }
                    }
                }
            }

            if (ret.EndsWith('_'))
            {
                ret = ret[..^1];
            }

            if (char.IsDigit(ret.First()))
                ret = "_" + ret;

            return ret;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as EnumDefinition);
        }

        public bool Equals(EnumDefinition? other)
        {
            return other is not null &&
                   EqualityComparer<Dictionary<string, string>>.Default.Equals(_sanitizedNames, other._sanitizedNames) &&
                   EqualityComparer<string[]>.Default.Equals(Names, other.Names) &&
                   EqualityComparer<string[]>.Default.Equals(FriendlyNames, other.FriendlyNames) &&
                   EqualityComparer<EnumMember[]>.Default.Equals(Members, other.Members);
        }

        public static bool operator ==(EnumDefinition? left, EnumDefinition? right)
        {
            return EqualityComparer<EnumDefinition>.Default.Equals(left, right);
        }

        public static bool operator !=(EnumDefinition? left, EnumDefinition? right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_sanitizedNames, Names, FriendlyNames, Members);
        }
    }
}