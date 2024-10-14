namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public readonly struct RefParameterWriter : IParameterWriter
    {
        public int Priority => 4 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.Ref);
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.CleanName} = &{cppParameter.Name})");
            context.AppendParam($"({rootParameter.Type.Name})p{cppParameter.CleanName}");
        }
    }
}