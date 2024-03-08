namespace HexaGen
{
    using CppAst;
    using HexaGen.Core;

    public class GenContext
    {
        public GenContext(CppCompilation compilation, string filePath, ICodeWriter codeWriter)
        {
            Compilation = compilation;
            FilePath = filePath;
            Writer = codeWriter;
        }

        public CppCompilation Compilation { get; set; }

        public string FilePath { get; set; }

        public ICodeWriter Writer { get; set; }
    }
}