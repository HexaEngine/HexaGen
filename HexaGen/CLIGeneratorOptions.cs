namespace HexaGen
{
    using CommandLine;

    public class CLIGeneratorOptions
    {
        [Option('o', "output-dir", Required = false)]
        public string? OutputDirectory { get; set; } = null;

        [Option("targets", Default = "all", Required = false)]
        public string Targets { get; set; } = null!;
    }
}