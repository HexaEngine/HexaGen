namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public readonly struct HandleParameterWriter : IParameterWriter
    {
        public int Priority => 8 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            var writeFunctionFlags = context.Flags;
            return writeFunctionFlags.HasFlag(WriteFunctionFlags.UseHandle) && index == 0;
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.AppendParam("Handle");
        }
    }
}