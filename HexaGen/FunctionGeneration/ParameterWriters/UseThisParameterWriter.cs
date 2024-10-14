namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public readonly struct UseThisParameterWriter : IParameterWriter
    {
        public int Priority => 7 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            var writeFunctionFlags = context.Flags;
            return writeFunctionFlags.HasFlag(WriteFunctionFlags.UseThis) && index == 0;
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            var overload = context.Overload;
            if (overload.Parameters[index].Type.IsPointer)
            {
                context.BeginBlock($"fixed ({overload.Parameters[index].Type.Name} @this = &this)");
                context.AppendParam("@this");
            }
            else
            {
                context.AppendParam("this");
            }
        }
    }
}