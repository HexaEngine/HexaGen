namespace HexaGen
{
    using HexaGen.Core;
    using System.Text;

    public delegate void HeaderInjectionDelegate(ICodeWriter writer, StringBuilder headerBuilder);

    public sealed class CodeWriter : ICodeWriter, IDisposable
    {
        private readonly string[] _indentStrings;

        private readonly StreamWriter _writer;

        private int lines;
        private int blocks = 0;
        private int indentLevel;

        private string _indentString = "";
        private bool _shouldIndent = true;

        public int IndentLevel { get => indentLevel; }

        public string NewLine { get => _writer.NewLine; }

        public string FileName { get; }

        public CodeWriter(string fileName, string template, HeaderInjectionDelegate? headerInjector)
        {
            FileName = fileName;

            _indentStrings = new string[10];
            for (int i = 0; i < _indentStrings.Length; i++)
            {
                _indentStrings[i] = new string('\t', i);
            }

            _writer = File.CreateText(fileName);
            _writer.NewLine = Environment.NewLine;

            StringBuilder stringBuilder = new(template);

            headerInjector?.Invoke(this, stringBuilder);

            _writer.Write(stringBuilder.ToString());
        }

        public long Length => _writer.BaseStream.Length;

        public int Lines => lines;

        public void Dispose()
        {
            EndBlock();
            _writer.Dispose();
        }

        public void Write(char chr)
        {
            WriteIndented(chr);
        }

        public void Write(string @string)
        {
            WriteIndented(@string);
        }

        public void WriteLine()
        {
            _writer.WriteLine();
            _shouldIndent = true;
        }

        public void WriteLine(string @string)
        {
            WriteIndented(@string);
            _writer.WriteLine();
            _shouldIndent = true;
            lines++;
        }

        public void WriteLines(string? @string)
        {
            if (@string == null)
                return;

            if (@string.Contains('\n'))
            {
                var lines = @string.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    WriteIndented(lines[i]);
                    _shouldIndent = true;
                    this.lines++;
                }
            }
            _shouldIndent = true;
        }

        public void WriteLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                WriteLine(line);
            }
        }

        public void BeginBlock(string content)
        {
            WriteLine(content);
            WriteLine("{");
            Indent(1);
            blocks++;
        }

        public void EndBlock()
        {
            if (blocks <= 0)
                return;
            blocks--;
            Unindent(1);
            WriteLine("}");
        }

        public IDisposable PushBlock(string marker = "{") => new CodeBlock(this, marker);

        public void Indent(int count = 1)
        {
            indentLevel += count;

            if (IndentLevel < _indentStrings.Length)
            {
                _indentString = _indentStrings[IndentLevel];
            }
            else
            {
                _indentString = new string('\t', IndentLevel);
            }
        }

        public void Unindent(int count = 1)
        {
            if (count > indentLevel)
                throw new ArgumentException("count out of range.", nameof(count));

            indentLevel -= count;
            if (indentLevel < _indentStrings.Length)
            {
                _indentString = _indentStrings[indentLevel];
            }
            else
            {
                _indentString = new string('\t', indentLevel);
            }
        }

        private void WriteIndented(char chr)
        {
            if (_shouldIndent)
            {
                _writer.Write(_indentString);
                _shouldIndent = false;
            }

            _writer.Write(chr);
        }

        private void WriteIndented(string @string)
        {
            if (_shouldIndent)
            {
                _writer.Write(_indentString);
                _shouldIndent = false;
            }

            _writer.Write(@string);
        }

        public readonly struct CodeBlock : IDisposable
        {
            private readonly CodeWriter _writer;

            public CodeBlock(CodeWriter writer, string content)
            {
                _writer = writer;
                _writer.BeginBlock(content);
            }

            public void Dispose()
            {
                _writer.EndBlock();
            }
        }
    }
}