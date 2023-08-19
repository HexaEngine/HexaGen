namespace MyBenchmarks
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using HexaGen.Language;

    public class LexerTest
    {
        private readonly Lexer lexer = new();
        private readonly string input;

        public LexerTest()
        {
            input = File.ReadAllText("C:\\Users\\juna\\source\\repos\\HexaGen\\HexaGen.Tests\\LeakTracer.cs");
        }

        [Benchmark]
        public void Test()
        {
            lexer.Tokenize(input, "");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<LexerTest>();
        }
    }
}