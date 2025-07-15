namespace HexaGen.Patching
{
    using System.Collections.Generic;

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
}