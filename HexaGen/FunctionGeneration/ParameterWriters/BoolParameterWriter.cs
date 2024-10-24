namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public class BoolParameterWriter : IParameterWriter
    {
        public virtual int Priority => 1 * IParameterWriter.PriorityMultiplier;

        public virtual bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.Bool) && !paramFlags.HasFlag(ParameterFlags.Ref) && !paramFlags.HasFlag(ParameterFlags.Pointer);
        }

        public virtual void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            var settings = context.Settings;
            context.AppendParam($"{cppParameter.Name} ? ({settings.GetBoolType()})1 : ({settings.GetBoolType()})0");
        }
    }
}