namespace TestApp
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using HexaGen;
    using HexaGen.Runtime;
    using Newtonsoft.Json.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class BitfieldBenchmarks
    {
        private Foo raw;

        [GlobalSetup]
        public void Setup()
        {
            raw = Foo.C; // start with max value
        }

        [Benchmark]
        public Foo GetA()
        {
            return Bitfield.Get(raw, 0, 8);
        }

        [Benchmark]
        public void SetA()
        {
            var val = Foo.B;
            Bitfield.Set(ref raw, val, 0, 8);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = CsCodeGeneratorConfig.Load("config.json");
            config.GeneratePlaceholderComments = false;
            CsCodeGenerator gen = new(config);

            gen.Generate("main.h", "../../../Output");
            Console.WriteLine("Hello, World!");
        }
    }

    public enum Foo
    {
        None,
        A,
        B,
        C,
    }

    public struct Baa
    {
        Foo raw;

        public Foo A { readonly get => Bitfield.Get(raw, 0, 8); set => Bitfield.Set(ref raw, value, 0, 8); }
        public Foo B { readonly get => Bitfield.Get(raw, 8, 8); set => Bitfield.Set(ref raw, value, 8, 8); }
        public Foo C { readonly get => Bitfield.Get(raw, 16, 8); set => Bitfield.Set(ref raw, value, 16, 8); }
    }
}