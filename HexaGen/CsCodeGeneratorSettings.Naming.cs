using System.Collections.Concurrent;
using System.Text;

namespace HexaGen
{
    public partial class CsCodeGeneratorSettings
    {
        private readonly ConcurrentDictionary<string, string> nameCache = new();

        public string GetCsCleanName(string name)
        {
            if (TypeMappings.TryGetValue(name, out string? mappedName))
            {
                return mappedName;
            }

            if (nameCache.TryGetValue(name, out string? cacheEntry))
            {
                return cacheEntry;
            }

            StringBuilder sb = new();
            bool isCaps = name.IsCaps();
            bool wasLowerCase = false;
            bool wasNumber = false;

            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (c == '_')
                {
                    wasLowerCase = true;
                    continue;
                }

                if (isCaps)
                {
                    c = char.ToLower(c);
                }

                if (i == 0)
                {
                    c = char.ToUpper(c);
                }

                if (wasLowerCase || wasNumber)
                {
                    c = char.ToUpper(c);
                    wasLowerCase = false;
                }

                sb.Append(c);
                wasNumber = char.IsDigit(c);
            }

            if (sb.Length > 0 && sb[^1] == 'T')
            {
                sb.Remove(sb.Length - 1, 1);
            }

            string newName = sb.ToString();

            foreach (var item in NameMappings)
            {
                newName = newName.Replace(item.Key, item.Value, StringComparison.InvariantCultureIgnoreCase);
            }

            nameCache.TryAdd(name, newName);

            return newName;
        }

        public string GetCsCleanNameWithConvention(string name, NamingConvention convention, bool removeTailingT = true)
        {
            if (TypeMappings.TryGetValue(name, out string? mappedName))
            {
                return mappedName;
            }

            if (nameCache.TryGetValue(name, out string? cacheEntry))
            {
                return cacheEntry;
            }

            string newName = NamingHelper.ConvertTo(name, convention);

            if (removeTailingT)
            {
                if (newName.Length > 0 && newName[^1] == 'T')
                {
                    newName = newName.Remove(newName.Length - 1, 1);
                }
            }

            foreach (var item in NameMappings)
            {
                newName = newName.Replace(item.Key, item.Value, StringComparison.InvariantCultureIgnoreCase);
            }

            nameCache.TryAdd(name, newName);

            return newName;
        }
    }
}