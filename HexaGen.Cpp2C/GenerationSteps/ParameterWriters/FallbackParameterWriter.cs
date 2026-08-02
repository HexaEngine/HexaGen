using HexaGen.CppAst.Model.Types;

namespace HexaGen.Cpp2C.GenerationSteps.ParameterWriters
{
    using HexaGen.CppAst.Model.Declarations;

    public class FallbackParameterWriter : ParameterWriter
    {
        public override int Priority { get; } = int.MinValue;

        public override bool CanWriteParameter(FunctionWriteContext context, CppParameter parameter, CppType cppType, CType cType)
        {
            return true;
        }

        public override void WriteParameter(FunctionWriteContext context, CppParameter parameter, CppType cppType, CType cType)
        {
            context.AppendParam(parameter.Name);
        }
    }
}