using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexaGen
{
    public partial class CsCodeGeneratorSettings
    {
        public string GetCsCleanName(string name)
        {
            if (TypeMappings.TryGetValue(name, out string? mappedName))
            {
                return mappedName;
            }

            StringBuilder sb = new();
            bool isCaps = name.IsCaps();
            bool wasLower = false;
            bool first = true;
            bool wasNumber = false;
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (c == '_')
                {
                    first = false;
                    wasLower = true;
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

                if (wasLower || wasNumber)
                {
                    c = char.ToUpper(c);
                    wasLower = false;
                }

                sb.Append(c);
                wasNumber = char.IsDigit(c);
            }

            if (sb[^1] == 'T')
            {
                sb.Remove(sb.Length - 1, 1);
            }

            string newName = sb.ToString();

            foreach (var item in NameMappings)
            {
                newName = newName.Replace(item.Key, item.Value, StringComparison.InvariantCultureIgnoreCase);
            }

            return newName;
        }
    }
}