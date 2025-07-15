namespace HexaGen.Patching
{
    using CppAst;
    using HexaGen.Metadata;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class PatchEngine
    {
        private readonly List<IPrePatch> preGenerationPatches = new();
        private readonly List<IPostPatch> postGenerationPatches = new();
        private readonly string baseStagePath;

        public PatchEngine(string baseStagePath)
        {
            this.baseStagePath = baseStagePath;
        }

        public PatchEngine() : this($"patches/{Guid.NewGuid()}")
        {
        }

        public void RegisterPrePatch(IPrePatch patch)
        {
            preGenerationPatches.Add(patch);
        }

        public void RegisterPostPatch(IPostPatch patch)
        {
            postGenerationPatches.Add(patch);
        }

        internal void Build()
        {
        }

        private readonly JsonSerializerSettings options = new() { Formatting = Formatting.Indented };

        public void ApplyPrePatches(CsCodeGeneratorConfig settings, string outputDir, List<string> files, CppCompilation compilation)
        {
            PatchContext? last = null;
            for (int i = 0; i < preGenerationPatches.Count; i++)
            {
                IPrePatch? patch = preGenerationPatches[i];
                PatchContext context = new(Path.Combine(baseStagePath, "pre", $"stage{i}"));
                if (last != null)
                {
                    context.CopyFromStage(last);
                }
                else
                {
                    context.CopyFromInput(outputDir, files);
                }

                patch.Apply(context, settings, files, compilation);
                context.WriteFile("settings.json", JsonConvert.SerializeObject(settings, options));
            }

            last?.CopyToOutput(outputDir);
        }

        public void ApplyPostPatches(CsCodeGeneratorMetadata metadata, string outputDir, List<string> files)
        {
            PatchContext? last = null;
            for (int i = 0; i < postGenerationPatches.Count; i++)
            {
                IPostPatch? patch = postGenerationPatches[i];
                PatchContext context = new(Path.Combine(baseStagePath, "post", $"stage{i}"));
                if (last != null)
                {
                    context.CopyFromStage(last);
                }
                else
                {
                    context.CopyFromInput(outputDir, files);
                }
                patch.Apply(context, metadata, files);
            }

            last?.CopyToOutput(outputDir);
        }
    }

    public interface IPatch
    {
    }

    public interface IPrePatch : IPatch
    {
        void Apply(PatchContext context, CsCodeGeneratorConfig settings, List<string> files, CppCompilation compilation);
    }

    public interface IPostPatch : IPatch
    {
        void Apply(PatchContext context, CsCodeGeneratorMetadata metadata, List<string> files);
    }
}