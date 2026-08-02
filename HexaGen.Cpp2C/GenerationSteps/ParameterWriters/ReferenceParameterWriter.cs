using HexaGen.CppAst.Model.Types;

namespace HexaGen.Cpp2C.GenerationSteps.ParameterWriters
{
    using HexaGen.CppAst.Model.Declarations;

    public class ReferenceParameterWriter : ParameterWriter
    {
        public override int Priority { get; } = 100;

        public override bool CanWriteParameter(FunctionWriteContext context, CppParameter parameter, CppType cppType, CType cType)
        {
            return cppType.IsReferenceType();
        }

        public override void WriteParameter(FunctionWriteContext context, CppParameter parameter, CppType cppType, CType cType)
        {
            string cast = string.Empty;
            cppType.IsReferenceType(out var referenceType);
            cast = $"{referenceType!.ElementType.GetFullName()}*";
            if (cppType is CppQualifiedType qualifiedType)
            {
                if (qualifiedType.Qualifier == CppTypeQualifier.Const)
                {
                    cast += "const ";
                }
                else if (qualifiedType.Qualifier == CppTypeQualifier.Volatile)
                {
                    cast += "volatile ";
                }
            }
            context.AppendParam($"*reinterpret_cast<{cast}>({parameter.Name})");
        }
    }
}