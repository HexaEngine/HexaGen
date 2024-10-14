namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using HexaGen.FunctionGeneration;
    using HexaGen.FunctionGeneration.ParameterWriters;
    using System.Collections.Generic;
    using System.Text;

    public partial class CsComCodeGenerator
    {
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