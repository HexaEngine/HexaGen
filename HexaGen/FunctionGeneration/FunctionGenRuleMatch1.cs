namespace HexaGen.FunctionGeneration
{
    using CppAst;
    using HexaGen;
    using HexaGen.Core.CSharp;
    using System.Collections.Generic;

    public abstract class FunctionGenRuleMatch : FunctionGenRule
    {
        public abstract bool IsMatch(CppParameter cppParameter, CppType type);

        public override CsParameterInfo CreateParameter(CppParameter cppParameter, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings, IList<CppParameter> cppParameters, CsParameterInfo[] csParameterList, int paramIndex, CsFunctionVariation variation)
        {
            if (IsMatch(cppParameter, cppParameter.Type))
            {
                return CreateParameter(cppParameter, cppParameter.Type, csParamName, kind, direction, settings);
            }

            return CreateDefaultWrapperParameter(cppParameter, csParamName, kind, direction, settings);
        }

        public abstract CsParameterInfo CreateParameter(CppParameter cppParameter, CppType type, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings);
    }
}