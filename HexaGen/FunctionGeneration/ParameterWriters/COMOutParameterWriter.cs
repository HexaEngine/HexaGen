namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen.Core.CSharp;

    public struct COMOutParameterWriter : IParameterWriter
    {
        public readonly int Priority => 4 * IParameterWriter.PriorityMultiplier - 100;

        public readonly bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return (paramFlags & ParameterFlags.Out) != 0;
        }

        public readonly void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.Writer.WriteLine($"{cppParameter.Name} = default;");
            if (paramFlags.HasFlag(ParameterFlags.COMPtr))
            {
                context.AppendParam($"({cppParameter.Type.Name}){cppParameter.Name}.GetAddressOf()");
            }
            else
            {
                context.AppendParam($"out {cppParameter.Name}");
            }
        }
    }
}