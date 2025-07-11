namespace HexaGen.Patching
{
    using CppAst;
    using HexaGen.Metadata;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.RegularExpressions;

    public class PatchContext
    {
        private readonly string stage;
        private readonly List<string> files = [];

        public PatchContext(string stage)
        {
            this.stage = stage;
        }

        public string Stage => stage;

        public void CopyFromInput(string root, List<string> input)
        {
            foreach (var file in input)
            {
                var relativePath = Path.GetRelativePath(root, file);
                var fullPath = Path.Combine(stage, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                File.Copy(file, fullPath, true);
                if (!files.Contains(relativePath))
                {
                    files.Add(relativePath);
                }
            }
        }

        public string GetFullPath(string path)
        {
            return Path.Combine(stage, path);
        }

        public void CopyToOutput(string root)
        {
            foreach (var file in files)
            {
                var relativePath = file;
                var fullPath = Path.Combine(root, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                File.Copy(GetFullPath(file), fullPath, true);
            }
        }

        public void CopyFromStage(PatchContext context)
        {
            foreach (var file in context.files)
            {
                var relativePath = file;
                var fullPath = Path.Combine(stage, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                File.Copy(context.GetFullPath(file), fullPath, true);
                if (!files.Contains(fullPath))
                {
                    files.Add(fullPath);
                }
            }
        }

        public string ReadFile(string path)
        {
            var fullPath = Path.Combine(stage, path);
            return File.ReadAllText(fullPath);
        }

        public void WriteFile(string path, string content)
        {
            var fullPath = Path.Combine(stage, path);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllText(fullPath, content);
            if (!files.Contains(path))
            {
                files.Add(path);
            }
        }
    }

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

        private JsonSerializerOptions options = new() { WriteIndented = true };

        public void ApplyPrePatches(CsCodeGeneratorConfig settings, string outputDir, List<string> files, ParseResult result)
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

                patch.Apply(context, settings, files, result);
                context.WriteFile("settings.json", JsonSerializer.Serialize(settings, options));
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
        void Apply(PatchContext context, CsCodeGeneratorConfig settings, List<string> files, ParseResult compilation);
    }

    public interface IPostPatch : IPatch
    {
        void Apply(PatchContext context, CsCodeGeneratorMetadata metadata, List<string> files);
    }

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