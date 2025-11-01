namespace HexaGen.FunctionGeneration
{
    using HexaGen;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Types;
    using System.Collections.Generic;

    public class FunctionGenRuleSpan : FunctionGenRule
    {
        public override CsParameterInfo CreateParameter(CppParameter cppParameter, ParameterMapping? mapping, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings, IList<CppParameter> cppParameters, CsParameterInfo[] csParameterList, int paramIndex, CsFunctionVariation variation)
        {
            if (cppParameter.Type is CppArrayType arrayType)
            {
                if (arrayType.Size > 0)
                {
                    return new(csParamName, cppParameter.Type, new($"ReadOnlySpan<{settings.GetCsTypeName(arrayType.ElementType)}>", kind), direction);
                }
            }
            else if (cppParameter.Type.IsString(settings, out var stringKind))
            {
                switch (stringKind)
                {
                    case CppPrimitiveKind.Char:
                        if (direction == Direction.InOut || direction == Direction.Out) break;
                        return new(csParamName, cppParameter.Type, new("ReadOnlySpan<byte>", stringKind), direction);

                    case CppPrimitiveKind.WChar:
                        if (direction == Direction.InOut || direction == Direction.Out) break;
                        return new(csParamName, cppParameter.Type, new("ReadOnlySpan<char>", stringKind), direction);
                }
            }

            return CreateDefaultWrapperParameter(cppParameter, mapping, csParamName, kind, direction, settings);
        }
    }
}