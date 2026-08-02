using HexaGen.CppAst.Model.Types;

namespace HexaGen.Cpp2C.GenerationSteps.ParameterWriters
{
    using HexaGen.Core;
    using HexaGen.CppAst.Model.Declarations;

    public class OpaquePointerParameterWriter : ParameterWriter
    {
        public override int Priority { get; } = 50;

        public override bool CanWriteParameter(FunctionWriteContext context, CppParameter parameter, CppType cppType, CType cType)
        {
            return cppType is CppPointerType cppPointer && cppPointer.ElementType is CppClass cppClass && cppClass.HasVirtualMembers();
        }

        public override void WriteParameter(FunctionWriteContext context, CppParameter parameter, CppType cppType, CType cType)
        {
            CppPointerType cppPointer = (CppPointerType)cppType;
            CppClass cppClass = (CppClass)cppPointer.ElementType;
            context.AppendParam($"reinterpret_cast<{cppClass.FullName}*>({parameter.Name})");
        }
    }
}