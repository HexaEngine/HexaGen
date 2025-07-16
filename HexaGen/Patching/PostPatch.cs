namespace HexaGen.Patching
{
    using HexaGen.Metadata;
    using System.Collections.Generic;

    public abstract class PostPatch : IPostPatch
    {
        private readonly List<RegexPatch> regexPatches = [];

        public void AddRegexPatch(RegexPatch patch)
        {
            regexPatches.Add(patch);
        }

        public virtual void Apply(PatchContext context, CsCodeGeneratorMetadata metadata, List<string> files)
        {
            PatchFiles(context, files);
        }

        protected virtual void PatchFiles(PatchContext context, List<string> files)
        {
            foreach (var file in files)
            {
                PatchFile(context, file);
            }
        }

        protected virtual void PatchFile(PatchContext context, string file)
        {
            var text = context.ReadFile(file);

            foreach (var patch in regexPatches)
            {
                patch.PostPatch(file, ref text);
            }

            context.WriteFile(file, text);
        }
    }
}