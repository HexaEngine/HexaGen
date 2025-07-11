namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen.Core.CSharp;

    public class COMOutParameterWriter : IParameterWriter
    {
        public virtual int Priority => 4 * IParameterWriter.PriorityMultiplier - 100;

        public virtual bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return (paramFlags & ParameterFlags.Out) != 0;
        }

        public virtual void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.Writer.WriteLine($"{cppParameter.Name} = default;");
            if (paramFlags.HasFlag(ParameterFlags.COMPtr))
            {
                context.AppendParam($"({rootParameter.Type.Name}){cppParameter.Name}.GetAddressOf()");
            }
            else
            {
                context.AppendParam($"out {cppParameter.Name}");
            }
        }
    }
}