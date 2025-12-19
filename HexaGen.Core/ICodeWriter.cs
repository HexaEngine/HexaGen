namespace HexaGen.Core
{
    using System;
    using System.Collections.Generic;

    public interface ICodeWriter
    {
        int IndentLevel { get; }

        void BeginBlock(string content);

        void Unindent(int count = 1);

        void Dispose();

        void EndBlock(string marker = "}");

        void Indent(int count = 1);

        IDisposable PushBlock(string marker = "{");

        void Write(char chr);

        void Write(string @string);

        void WriteLine();

        void WriteLine(string @string);

        void WriteLines(string? @string);

        void WriteLines(IEnumerable<string> lines);
    }
}