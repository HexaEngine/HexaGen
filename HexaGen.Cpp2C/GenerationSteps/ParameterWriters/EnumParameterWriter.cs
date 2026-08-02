using HexaGen.CppAst.Model.Types;

namespace HexaGen.Cpp2C.GenerationSteps.ParameterWriters
{
    using HexaGen.CppAst.Model.Declarations;

    public class EnumParameterWriter : ParameterWriter
    {
        public override int Priority { get; } = 75;

        public override bool CanWriteParameter(FunctionWriteContext context, CppParameter parameter, CppType cppType, CType cType)
        {
            return cppType.IsEnumType();
        }

        public override void WriteParameter(FunctionWriteContext context, CppParameter parameter, CppType cppType, CType cType)
        {
            context.AppendParam($"static_cast<{cppType.FullName}>({parameter.Name})");
        }
    }
}