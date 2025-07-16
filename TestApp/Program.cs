namespace TestApp
{
    using HexaGen;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = CsCodeGeneratorConfig.Load("config.json");

            Console.WriteLine("Hello, World!");
        }
    }
}