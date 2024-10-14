namespace HexaGen.FunctionGeneration
{
    using CppAst;
    using HexaGen;
    using HexaGen.Core.CSharp;
    using System.Collections.Generic;

    public class FunctionGenRuleString : FunctionGenRule
    {
        public override CsParameterInfo CreateParameter(CppParameter cppParameter, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings, IList<CppParameter> cppParameters, CsParameterInfo[] csParameterList, int paramIndex, CsFunctionVariation variation)
        {
            if (cppParameter.Type is CppArrayType arrayType && arrayType.ElementType.IsString())
            {
                return new(csParamName, cppParameter.Type, new("string[]", kind), direction);
            }

            if (cppParameter.Type.IsString())
            {
                return new(csParamName, cppParameter.Type, new(direction == Direction.InOut ? "ref string" : "string", kind), direction);
            }

            return CreateDefaultWrapperParameter(cppParameter, csParamName, kind, direction, settings);
        }
    }
}