namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public class SpanParameterWriter : IParameterWriter
    {
        public virtual int Priority => 3 * IParameterWriter.PriorityMultiplier;

        public virtual bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.Span);
        }

        public virtual void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            if (context.Variation.ExportedName == "glLoadTransposeMatrixd")
            {
                var name = cppParameter.Type.Classify();
            }
            context.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.CleanName} = {cppParameter.Name})");
            context.AppendParam($"({rootParameter.Type.Name})p{cppParameter.CleanName}");
        }
    }
}