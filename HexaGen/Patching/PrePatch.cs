namespace HexaGen.Patching
{
    using CppAst;
    using System.Collections.Generic;

    public abstract class PrePatch : IPrePatch
    {
        private readonly List<RegexPatch> regexPatches = [];

        public void AddRegexPatch(RegexPatch patch)
        {
            regexPatches.Add(patch);
        }

        public virtual void Apply(PatchContext context, CsCodeGeneratorConfig settings, List<string> files, CppCompilation compilation)
        {
            PatchFiles(context, settings, compilation, files);

            PatchCompilation(settings, compilation);
        }

        protected virtual void PatchFiles(PatchContext context, CsCodeGeneratorConfig settings, CppCompilation compilation, List<string> files)
        {
            foreach (var file in files)
            {
                PatchFile(context, settings, compilation, file);
            }
        }

        protected virtual void PatchFile(PatchContext context, CsCodeGeneratorConfig settings, CppCompilation compilation, string file)
        {
            var text = File.ReadAllText(file);

            foreach (var patch in regexPatches)
            {
                patch.PrePatch(settings, compilation, file, ref text);
            }

            File.WriteAllText(file, text);
        }

        protected virtual void PatchCompilation(CsCodeGeneratorConfig settings, CppCompilation compilation)
        {
            foreach (var type in compilation.Classes)
            {
                PatchClass(settings, type);
            }

            foreach (var type in compilation.Typedefs)
            {
                PatchTypedef(settings, type);
            }

            foreach (var type in compilation.Functions)
            {
                PatchFunction(settings, type);
            }

            foreach (var type in compilation.Enums)
            {
                PatchEnum(settings, type);
            }
        }

        protected virtual void PatchClass(CsCodeGeneratorConfig settings, CppClass cppClass)
        {
        }

        protected virtual void PatchTypedef(CsCodeGeneratorConfig settings, CppTypedef cppTypedef)
        {
        }

        protected virtual void PatchFunction(CsCodeGeneratorConfig settings, CppFunction cppFunction)
        {
        }

        protected virtual void PatchEnum(CsCodeGeneratorConfig settings, CppEnum cppEnum)
        {
        }
    }
}