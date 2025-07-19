namespace HexaGen.FunctionGeneration
{
    using HexaGen;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Types;

    public class FunctionGenRuleRef : FunctionGenRuleMatch<CppArrayType>
    {
        public override CsParameterInfo CreateParameter(CppParameter cppParameter, ParameterMapping? mapping, CppArrayType type, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings)
        {
            if (mapping != null && mapping.UseOut)
            {
                return new(csParamName, cppParameter.Type, new("out " + settings.GetCsTypeName(type.ElementType, false), kind), direction);
            }
            return new(csParamName, cppParameter.Type, new("ref " + settings.GetCsTypeName(type.ElementType, false), kind), direction);
        }

        public override bool IsMatch(CppParameter cppParameter, CppArrayType type)
        {
            return type.Size > 0;
        }
    }
}