namespace HexaGen.Patching
{
    using System.Text.RegularExpressions;

    public abstract class RegexPatch
    {
        protected Regex Regex;
        private readonly string? targetFile;

        public RegexPatch(string pattern, RegexOptions options, string? targetFile = null)
        {
            if ((options & RegexOptions.Compiled) == 0)
            {
                options |= RegexOptions.Compiled;
            }

            Regex = new Regex(pattern, options);
            this.targetFile = targetFile;
        }

        public RegexPatch(string pattern, string? targetFile = null)
        {
            Regex = new Regex(pattern, RegexOptions.Compiled);
            this.targetFile = targetFile;
        }

        public RegexPatch(Regex regex, string? targetFile = null)
        {
            Regex = regex;
            this.targetFile = targetFile;
        }

        public virtual void PrePatch(CsCodeGeneratorConfig settings, ParseResult result, string file, ref string text)
        {
            if (targetFile != null && file != targetFile)
            {
                return;
            }

            var matches = Regex.Matches(text);

            foreach (Match match in matches)
            {
                PrePatchMatch(settings, result, ref text, match);
            }
        }

        protected virtual void PrePatchMatch(CsCodeGeneratorConfig settings, ParseResult result, ref string text, Match match)
        {
        }

        public virtual void PostPatch(string file, ref string text)
        {
            if (targetFile != null && file != targetFile)
            {
                return;
            }

            var matches = Regex.Matches(text);

            foreach (Match match in matches)
            {
                PostPatchMatch(file, ref text, match);
            }
        }

        protected virtual void PostPatchMatch(string file, ref string text, Match match)
        {
        }
    }
}