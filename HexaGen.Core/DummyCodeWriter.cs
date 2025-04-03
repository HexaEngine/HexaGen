namespace HexaGen
{
    using HexaGen.Core;

    public sealed class DummyCodeWriter : ICodeWriter, IDisposable
    {
        private int indentLevel;

        public int IndentLevel { get => indentLevel; }

        public void BeginBlock(string content)
        {
        }

        public void Dispose()
        {
        }

        public void EndBlock()
        {
        }

        public void Indent(int count = 1)
        {
            indentLevel += count;
        }

        public IDisposable PushBlock(string marker = "{")
        {
            return new DummyBlock(this);
        }

        private struct DummyBlock : IDisposable
        {
            private DummyCodeWriter dummyCodeWriter;

            public DummyBlock(DummyCodeWriter dummyCodeWriter)
            {
                this.dummyCodeWriter = dummyCodeWriter;
            }

            public void Dispose()
            {
                dummyCodeWriter.EndBlock();
            }
        }

        public void Unindent(int count = 1)
        {
            indentLevel -= count;
        }

        public void Write(char chr)
        {
        }

        public void Write(string @string)
        {
        }

        public void WriteLine()
        {
        }

        public void WriteLine(string @string)
        {
        }

        public void WriteLines(string? @string)
        {
        }

        public void WriteLines(IEnumerable<string> lines)
        {
        }
    }
}