namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public class ArrayParameterWriter : IParameterWriter
    {
        public virtual int Priority => 2 * IParameterWriter.PriorityMultiplier;

        public virtual bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.Array);
        }

        public virtual void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.CleanName} = {cppParameter.Name})");
            context.AppendParam($"({rootParameter.Type.Name})p{cppParameter.CleanName}");
        }
    }
}