namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public class HandleParameterWriter : IParameterWriter
    {
        public virtual int Priority => 8 * IParameterWriter.PriorityMultiplier;

        public virtual bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            var writeFunctionFlags = context.Flags;
            return writeFunctionFlags.HasFlag(WriteFunctionFlags.UseHandle) && index == 0;
        }

        public virtual void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.AppendParam("Handle");
        }
    }
}