namespace HexaGen
{
    using HexaGen.Core;
    using System.Text;

    public enum NamingConvention
    {
        Unknown,
        PascalCase,
        CamelCase,
        SnakeCase,
        ScreamingSnakeCase,
        UpperFlatCase,
        LowerFlatCase,
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
                return NamingConvention.UpperFlatCase;
            }
            else if (hasLowerCase)
            {
                return NamingConvention.LowerFlatCase;
            }

            return NamingConvention.Unknown;
        }

        public static string[] GetParts(string input, NamingConvention convention)
        {
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

                case NamingConvention.Unknown:
                case NamingConvention.LowerFlatCase:
                case NamingConvention.UpperFlatCase:
                    parts = WordList.EN_EN.SplitWords(input);
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

            if (sourceConvention == targetConvention)
            {
                return input;
            }

            var parts = GetParts(input, sourceConvention);

            StringBuilder sb = new();

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                if (string.IsNullOrWhiteSpace(part))
                {
                    continue;
                }
                if (char.IsDigit(part[0]) || char.IsDigit(part[^1]))
                {
                    sb.Append(part);
                    continue;
                }
                switch (targetConvention)
                {
                    case NamingConvention.CamelCase:
                        if (i == 0)
                        {
                            bool wasNumber = false;
                            for (int j = 0; j < part.Length; j++)
                            {
                                var c = part[j];
                                if (wasNumber)
                                {
                                    c = char.ToUpper(c);
                                }
                                else
                                {
                                    c = char.ToLower(c);
                                }
                                sb.Append(c);
                                wasNumber = char.IsDigit(c);
                            }
                        }
                        else
                        {
                            sb.Append(part.Capitalize());
                        }
                        break;

                    case NamingConvention.PascalCase:
                        {
                            bool wasNumber = false;
                            for (int j = 0; j < part.Length; j++)
                            {
                                var c = part[j];
                                if (wasNumber || j == 0)
                                {
                                    c = char.ToUpper(c);
                                }
                                else
                                {
                                    c = char.ToLower(c);
                                }
                                sb.Append(c);
                                wasNumber = char.IsDigit(c);
                            }
                        }
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

                    case NamingConvention.UpperFlatCase:
                        sb.Append(part.ToUpper());
                        break;

                    case NamingConvention.LowerFlatCase:
                        sb.Append(part.ToLower());
                        break;
                }
            }

            return sb.ToString();
        }

        public static unsafe string Capitalize(this string input)
        {
            string copy = input.ToString();
            fixed (char* c = copy)
            {
                c[0] = char.ToUpper(c[0]);
                for (int i = 1; i < copy.Length; i++)
                {
                    c[i] = char.ToLower(c[i]);
                }
            }
            return copy;
        }
    }
}