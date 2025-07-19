namespace HexaGen
{
    using HexaGen.CppAst.Model.Metadata;
    using HexaGen.CppAst.Parsing;
    using HexaGen.FunctionGeneration;
    using HexaGen.GenerationSteps;
    using HexaGen.PreProcessSteps;
    using System;
    using System.Collections.Generic;

    public partial class CsComCodeGenerator : CsCodeGenerator
    {
        public CsComCodeGenerator(CsCodeGeneratorConfig settings) : base(settings)
        {
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

        protected override void ConfigureGeneratorCore(List<PreProcessStep> preProcessSteps, List<GenerationStep> generationSteps, out FunctionGenerator funcGen)
        {
            funcGen = FunctionGenerator.CreateForCOM(config);
            preProcessSteps.Add(new ConstantPreProcessStep(this, config));
            generationSteps.Add(new ComEnumGenerationStep(this, config));
            generationSteps.Add(new ComConstantGenerationStep(this, config));
            generationSteps.Add(new ComHandleGenerationStep(this, config));
            generationSteps.Add(new ComTypeGenerationStep(this, config));
            generationSteps.Add(new ComFunctionGenerationStep(this, config));
            generationSteps.Add(new ComExtensionGenerationStep(this, config));
            generationSteps.Add(new ComDelegateGenerationStep(this, config));
        }

        protected override CppCompilation ParseFiles(CppParserOptions parserOptions, List<string> headerFiles)
        {
            comGUIDExtractor.ExtractGuidsFromFiles(headerFiles, this, guids, guidMap);
            return base.ParseFiles(parserOptions, headerFiles);
        }
    }
}