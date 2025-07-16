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

        public virtual void Apply(PatchContext context, CsCodeGeneratorConfig settings, List<string> files, ParseResult result)
        {
            PatchFiles(context, settings, result, files);

            PatchCompilation(settings, result);
        }

        protected virtual void PatchFiles(PatchContext context, CsCodeGeneratorConfig settings, ParseResult result, List<string> files)
        {
            foreach (var file in files)
            {
                PatchFile(context, settings, result, file);
            }
        }

        protected virtual void PatchFile(PatchContext context, CsCodeGeneratorConfig settings, ParseResult result, string file)
        {
            var text = File.ReadAllText(file);

            foreach (var patch in regexPatches)
            {
                patch.PrePatch(settings, result, file, ref text);
            }

            File.WriteAllText(file, text);
        }

        protected virtual void PatchCompilation(CsCodeGeneratorConfig settings, ParseResult result)
        {
            var compilation = result.Compilation;
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