namespace HexaGen.FunctionGeneration
{
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Types;

    public class FunctionGenRuleArray : FunctionGenRuleMatch<CppArrayType>
    {
        private readonly CsCodeGeneratorConfig config;

        public FunctionGenRuleArray(CsCodeGeneratorConfig config)
        {
            this.config = config;
        }

        public override CsParameterInfo CreateParameter(CppParameter cppParameter, ParameterMapping? mapping, CppArrayType type, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings)
        {
            if (settings.TryGetArrayMapping(type, out var arrayMapping))
            {
                return new(csParamName, cppParameter.Type, new($"ref {arrayMapping}", kind), direction);
            }

            return CreateDefaultWrapperParameter(cppParameter, mapping, csParamName, kind, direction, settings);
        }

        public override bool IsMatch(CppParameter cppParameter, CppArrayType type)
        {
            return type.Size > 0 && config.TryGetArrayMapping(type, out _);
        }
    }
}