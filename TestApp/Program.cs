namespace TestApp
{
    using HexaGen;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = CsCodeGeneratorConfig.Load("config.json");
            CsCodeGenerator generator = new(config);
            generator.LogToConsole();
            generator.Generate("test.h", "Output");
        }
    }
}