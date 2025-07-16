namespace HexaGen
{
    using CppAst;
    using HexaGen.GenerationSteps;
    using HexaGen.FunctionGeneration;
    using HexaGen.GenerationSteps;
    using HexaGen.PreProcessSteps;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    public partial class CsComCodeGenerator : CsCodeGenerator
    {
        public CsComCodeGenerator(CsCodeGeneratorConfig settings) : base(settings)
        {
            PreProcessSteps.Clear();
            PreProcessSteps.Add(new ConstantPreProcessStep(this, config));

            GenerationSteps.Clear();
            GenerationSteps.Add(new ComEnumGenerationStep(this, config));
            GenerationSteps.Add(new ComConstantGenerationStep(this, config));
            GenerationSteps.Add(new ComHandleGenerationStep(this, config));
            GenerationSteps.Add(new ComTypeGenerationStep(this, config));
            GenerationSteps.Add(new ComFunctionGenerationStep(this, config));
            GenerationSteps.Add(new ComExtensionGenerationStep(this, config));
            GenerationSteps.Add(new ComDelegateGenerationStep(this, config));
        }

        private ComGUIDExtractor comGUIDExtractor = new();
        private readonly List<(string, Guid)> guids = [];
        private readonly Dictionary<string, Guid> guidMap = [];

        public ComGUIDExtractor ComGUIDExtractor { get => comGUIDExtractor; set => comGUIDExtractor = value; }

        public Guid? GetGUID(string name)
        {
            if (guidMap.TryGetValue(name, out Guid guid))
            {
                return guid;
            }
            return null;
        }

        public bool TryGetGUID(string name, out Guid guid)
        {
            return guidMap.TryGetValue(name, out guid);
        }

        public bool HasGUID(string name)
        {
            return guidMap.ContainsKey(name);
        }

        public override bool Generate(List<string> headerFiles, string outputPath, List<string>? allowedHeaders = null)
        {
            LogInfo($"Generating: {config.ApiName}");
            var options = PrepareSettings();

            comGUIDExtractor.ExtractGuidsFromFiles(headerFiles, this, guids, guidMap);

            LogInfo("Parsing Headers...");
            CppCompilation compilation = CppParser.ParseFiles(headerFiles, options);

            return Generate(compilation, headerFiles, outputPath, allowedHeaders);
        }

        public override bool Generate(string headerFile, string outputPath, List<string>? allowedHeaders = null)
        {
            LogInfo($"Generating: {config.ApiName}");
            var options = PrepareSettings();

            comGUIDExtractor.ExtractGuidsFromFile(headerFile, this, guids, guidMap);

            LogInfo("Parsing Headers...");
            CppCompilation compilation = CppParser.ParseFile(headerFile, options);

            return Generate(compilation, [headerFile], outputPath, allowedHeaders);
        }
    }
}