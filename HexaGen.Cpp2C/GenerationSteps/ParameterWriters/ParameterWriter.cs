using HexaGen.CppAst.Model.Types;

namespace HexaGen.Cpp2C.GenerationSteps.ParameterWriters
{
    using HexaGen.CppAst.Model.Declarations;

    public abstract class ParameterWriter
    {
        public abstract int Priority { get; }

        public abstract bool CanWriteParameter(FunctionWriteContext context, CppParameter parameter, CppType cppType, CType cType);

        public abstract void WriteParameter(FunctionWriteContext context, CppParameter parameter, CppType cppType, CType cType);
    }

}