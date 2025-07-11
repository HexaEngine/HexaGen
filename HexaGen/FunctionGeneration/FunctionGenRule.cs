namespace HexaGen.FunctionGeneration
{
    using CppAst;
    using HexaGen;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using System.Collections.Generic;

    public abstract class FunctionGenRule
    {
        public abstract CsParameterInfo CreateParameter(CppParameter cppParameter, ParameterMapping? mapping, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings, IList<CppParameter> cppParameters, CsParameterInfo[] csParameters, int paramIndex, CsFunctionVariation variation);

        public virtual CsParameterInfo CreateDefaultParameter(CppParameter cppParameter, ParameterMapping? mapping, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings)
        {
            return new(csParamName, cppParameter.Type, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
        }

        public virtual CsParameterInfo CreateDefaultWrapperParameter(CppParameter cppParameter, ParameterMapping? mapping, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings)
        {
            if (mapping?.UseOut ?? false)
            {
                return new(csParamName, cppParameter.Type, new(settings.GetCsWrapperTypeName(cppParameter.Type, false).Replace("ref", "out"), kind), direction);
            }
            return new(csParamName, cppParameter.Type, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
        }
    }
}