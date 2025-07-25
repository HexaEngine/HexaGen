﻿namespace HexaGen
{
    using HexaGen.Core;
    using HexaGen.CppAst.Model.Metadata;

    public class GenContext
    {
        public GenContext(ParseResult result, string filePath, ICodeWriter codeWriter)
        {
            ParseResult = result;
            Compilation = result.Compilation;
            FilePath = filePath;
            Writer = codeWriter;
        }

        public ParseResult ParseResult { get; set; }

        public CppCompilation Compilation { get; set; }

        public string FilePath { get; set; }

        public ICodeWriter Writer { get; set; }
    }
}