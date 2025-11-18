namespace HexaGen.PreProcessSteps
{
    using HexaGen.Core;
    using HexaGen.Core.CSharp;
    using HexaGen.CppAst.Model;
    using HexaGen.CppAst.Model.Metadata;
    using HexaGen.Metadata;
    using System.Collections.Frozen;

    public class ConstantPreProcessStep : PreProcessStep
    {
        public ConstantPreProcessStep(CsCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
        }

        public override void Configure(CsCodeGeneratorConfig config)
        {
        }

        public override void PreProcess(FileSet files, CppCompilation compilation, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata, ParseResult result)
        {
            FrozenSet<string> functionNames = compilation.Functions.Select(f => f.Name).ToFrozenSet();
            foreach (CppMacro macro in compilation.Macros)
            {
                if (!files.Contains(macro.SourceFile))
                    continue;

                ProcessConstant(macro, functionNames, result);
            }
        }

        private void ProcessConstant(CppMacro macro, FrozenSet<string> functionNames, ParseResult result)
        {
            var value = macro.Value.NormalizeConstantValue();
            if (functionNames.Contains(value))
            {
                string name = config.GetCsFunctionName(macro.Name);
                FunctionAlias alias = new(value, macro.Name, name, null);
                result.AddFunctionAlias(alias);
            }
        }
    }
}