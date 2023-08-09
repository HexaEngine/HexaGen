namespace HexaGen
{
    using CppAst;

    public class GenContext
    {
        public GenContext(CppCompilation compilation, string filePath, CodeWriter codeWriter)
        {
            Compilation = compilation;
            FilePath = filePath;
            Writer = codeWriter;
        }

        public CppCompilation Compilation { get; set; }

        public string FilePath { get; set; }

        public CodeWriter Writer { get; set; }
    }
}