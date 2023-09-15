namespace HexaGen
{
    using System.Text;

    public enum NamingConvention
    {
        Unknown,
        PascalCase,
        CamelCase,
        SnakeCase,
        ScreamingSnakeCase,
        Caps,
    }

    public static class NamingHelper
    {
        public static NamingConvention AnalyzeNamingConvention(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return NamingConvention.Unknown;
            }

            bool hasUnderscore = input.Contains('_');
            bool hasLowerCase = false;
            bool hasUpperCase = false;
            bool firstIsUpperCase = char.IsUpper(input[0]);

            hasUpperCase |= firstIsUpperCase;

            for (int i = 1; i < input.Length; i++)
            {
                var c = input[i];
                if (char.IsLower(c))
                {
                    hasLowerCase = true;
                }
                else if (char.IsUpper(c))
                {
                    hasUpperCase = true;
                }
                if (hasUpperCase && hasLowerCase)
                {
                    break;
                }
            }

            if (hasUnderscore)
            {
                if (!hasLowerCase && hasUpperCase)
                {
                    return NamingConvention.ScreamingSnakeCase;
                }

                return NamingConvention.SnakeCase;
            }
            else if (firstIsUpperCase && hasLowerCase && hasUpperCase)
            {
                return NamingConvention.PascalCase;
            }
            else if (hasLowerCase)
            {
                return NamingConvention.CamelCase;
            }
            else if (hasUpperCase)
            {
                return NamingConvention.Caps;
            }

            return NamingConvention.Unknown;
        }

        public static string[] GetParts(string input, NamingConvention convention)
        {
            if (convention == NamingConvention.Unknown || convention == NamingConvention.Caps)
            {
                return new string[] { input };
            }
            string[] parts = new string[] { input };
            switch (convention)
            {
                case NamingConvention.CamelCase:
                case NamingConvention.PascalCase:
                    parts = input.SplitByCase();
                    break;

                case NamingConvention.ScreamingSnakeCase:
                case NamingConvention.SnakeCase:
                    parts = input.Split("_");
                    break;
            }

            return parts;
        }

        public static string ConvertTo(string input, NamingConvention targetConvention)
        {
            // unknown as target is basically keep original convention.
            if (targetConvention == NamingConvention.Unknown)
            {
                return input;
            }

            var sourceConvention = AnalyzeNamingConvention(input);
            var parts = GetParts(input, sourceConvention);

            StringBuilder sb = new();

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                switch (targetConvention)
                {
                    case NamingConvention.CamelCase:
                        if (i == 0)
                        {
                            sb.Append(part.ToLower());
                        }
                        else
                        {
                            sb.Append(part.Capitalize());
                        }
                        break;

                    case NamingConvention.PascalCase:
                        sb.Append(part.Capitalize());
                        break;

                    case NamingConvention.SnakeCase:
                        if (i > 0)
                            sb.Append('_');
                        sb.Append(part.ToLower());
                        break;

                    case NamingConvention.ScreamingSnakeCase:
                        if (i > 0)
                            sb.Append('_');
                        sb.Append(part.ToUpper());
                        break;

                    case NamingConvention.Caps:
                        sb.Append(part.ToUpper());
                        break;
                }
            }

            return sb.ToString();
        }

        public static unsafe string Capitalize(this string input)
        {
            fixed (char* c = input)
            {
                c[0] = char.ToUpper(c[0]);
                for (int i = 1; i < input.Length; i++)
                {
                    c[i] = char.ToLower(c[i]);
                }
            }
            return input;
        }
    }
}