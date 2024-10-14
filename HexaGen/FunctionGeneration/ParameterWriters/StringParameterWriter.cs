namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public readonly struct StringParameterWriter : IParameterWriter
    {
        public int Priority => 5 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.String);
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            if (paramFlags.HasFlag(ParameterFlags.Array))
            {
                context.WriteStringArrayConvertToUnmanaged(cppParameter);
            }
            else
            {
                context.WriteStringConvertToUnmanaged(cppParameter, paramFlags.HasFlag(ParameterFlags.Ref));
            }
        }
    }
}