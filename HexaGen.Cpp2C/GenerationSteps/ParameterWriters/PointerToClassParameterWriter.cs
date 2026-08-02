using HexaGen.CppAst.Model.Types;

namespace HexaGen.Cpp2C.GenerationSteps.ParameterWriters
{
    using HexaGen.CppAst.Model.Declarations;

    public class PointerToClassParameterWriter : ParameterWriter
    {
        public override int Priority { get; } = 80;

        public override bool CanWriteParameter(FunctionWriteContext context, CppParameter parameter, CppType cppType, CType cType)
        {
            return cppType.IsPointerToClassType(out _, out _);
        }

        public override void WriteParameter(FunctionWriteContext context, CppParameter parameter, CppType cppType, CType cType)
        {
            cppType.IsPointerToClassType(out int depth, out CppClass? cppClass);
            string cast = string.Concat(Enumerable.Repeat("*", depth));
            context.AppendParam($"reinterpret_cast<{cppType.GetFullName()}>({parameter.Name})");
        }
    }
}