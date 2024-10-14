namespace HexaGen.FunctionGeneration
{
    using CppAst;
    using HexaGen;
    using HexaGen.Core.CSharp;

    public class FunctionGenRuleRef : FunctionGenRuleMatch<CppArrayType>
    {
        public override CsParameterInfo CreateParameter(CppParameter cppParameter, CppArrayType type, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings)
        {
            return new(csParamName, cppParameter.Type, new("ref " + settings.GetCsTypeName(type.ElementType, false), kind), direction);
        }

        public override bool IsMatch(CppParameter cppParameter, CppArrayType type)
        {
            return type.Size > 0;
        }
    }
}