namespace HexaGen.FunctionGeneration
{
    using HexaGen;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Types;
    using System.Collections.Generic;

    public class FunctionGenRuleString : FunctionGenRule
    {
        public override CsParameterInfo CreateParameter(CppParameter cppParameter, ParameterMapping? mapping, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings, IList<CppParameter> cppParameters, CsParameterInfo[] csParameterList, int paramIndex, CsFunctionVariation variation)
        {
            if (cppParameter.Type is CppArrayType arrayType && arrayType.ElementType.IsString(settings, out var stringKind0))
            {
                return new(csParamName, cppParameter.Type, new("string[]", stringKind0), direction);
            }

            if (cppParameter.Type.IsString(settings, out var stringKind1))
            {
                CsType type = new(direction == Direction.InOut ? "ref string" : "ReadOnlySpan<char>", stringKind1);
                type.FlagAsString();
                return new(csParamName, cppParameter.Type, type, direction);
            }

            return CreateDefaultWrapperParameter(cppParameter, mapping, csParamName, kind, direction, settings);
        }
    }
}