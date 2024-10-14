namespace HexaGen.FunctionGeneration
{
    using CppAst;
    using HexaGen;
    using HexaGen.Core.CSharp;
    using System.Collections.Generic;

    public abstract class FunctionGenRuleMatch<T> : FunctionGenRule where T : ICppElement
    {
        public abstract bool IsMatch(CppParameter cppParameter, T type);

        public override CsParameterInfo CreateParameter(CppParameter cppParameter, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings, IList<CppParameter> cppParameters, CsParameterInfo[] csParameterList, int paramIndex, CsFunctionVariation variation)
        {
            if (cppParameter.Type is T t && IsMatch(cppParameter, t))
            {
                return CreateParameter(cppParameter, t, csParamName, kind, direction, settings);
            }

            return CreateDefaultWrapperParameter(cppParameter, csParamName, kind, direction, settings);
        }

        public abstract CsParameterInfo CreateParameter(CppParameter cppParameter, T type, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings);
    }
}