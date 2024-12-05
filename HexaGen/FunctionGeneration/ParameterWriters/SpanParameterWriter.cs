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
            var varName = context.UniqueName($"p{cppParameter.CleanName}");
            context.BeginBlock($"fixed ({cppParameter.Type.CleanName}* {varName} = {cppParameter.Name})");
            context.AppendParam($"({rootParameter.Type.Name}){varName}");
        }
    }
}