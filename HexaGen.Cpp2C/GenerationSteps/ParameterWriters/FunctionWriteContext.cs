namespace HexaGen.Cpp2C.GenerationSteps.ParameterWriters
{
    using HexaGen.Core;
    using HexaGen.CppAst.Model.Declarations;
    using System;
    using System.Text;

    public class FunctionWriteContext : IDisposable
    {
        private readonly StringBuilder sb = new();
        private bool firstParam = true;
        private int blockLevel = 0;

        public FunctionWriteContext(CppFunction function, ICodeWriter writer)
        {
            Function = function;
            Writer = writer;
        }

        public CppFunction Function { get; }

        public ICodeWriter Writer { get; }

        public void AppendParam(string text)
        {
            if (!firstParam)
            {
                sb.Append(", ");
            }
            firstParam = false;
            sb.Append(text);
        }

        public void BeginBlock(string line)
        {
            Writer.BeginBlock(line);
            blockLevel++;
        }

        public void WriteLine(string line)
        {
            Writer.WriteLine(line);
        }

        public void WriteLine()
        {
            Writer.WriteLine();
        }

        public void Write(string line)
        {
            Writer.Write(line);
        }

        public void EndBlock(string line)
        {
            Writer.EndBlock(line);
            blockLevel--;
        }

        public string GetParamString()
        {
            return sb.ToString();
        }

        public void Dispose()
        {
            while (blockLevel-- > 0)
            {
                Writer.EndBlock();
            }
            GC.SuppressFinalize(this);
        }
    }
}