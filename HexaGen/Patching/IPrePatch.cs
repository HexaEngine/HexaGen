namespace HexaGen.Patching
{
    using System.Collections.Generic;

    public interface IPrePatch : IPatch
    {
        void Apply(PatchContext context, CsCodeGeneratorConfig settings, List<string> files, ParseResult compilation);
    }
}