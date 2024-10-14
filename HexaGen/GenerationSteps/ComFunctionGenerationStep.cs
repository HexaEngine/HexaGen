namespace HexaGen.GenerationSteps
{
    using HexaGen.FunctionGeneration.ParameterWriters;
    using System.Collections.Generic;

    public class ComFunctionGenerationStep : FunctionGenerationStep
    {
        public ComFunctionGenerationStep(CsCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
        }

        protected override List<string> SetupFunctionUsings()
        {
            var usings = base.SetupFunctionUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }

        public override List<IParameterWriter> ParameterWriters { get; } =
        [
            new HandleParameterWriter(),
            new UseThisParameterWriter(),
            new DefaultValueParameterWriter(),
            new StringParameterWriter(),
            new RefParameterWriter(),
            new COMOutParameterWriter(),
            new SpanParameterWriter(),
            new ArrayParameterWriter(),
            new BoolParameterWriter(),
            new COMParameterWriter(),
            new FallthroughParameterWriter(),
        ];
    }
}