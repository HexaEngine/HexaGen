namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public class StringParameterWriter : IParameterWriter
    {
        public virtual int Priority => 5 * IParameterWriter.PriorityMultiplier;

        public virtual bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.String);
        }

        public virtual void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            if (paramFlags.HasFlag(ParameterFlags.Array))
            {
                context.WriteStringArrayConvertToUnmanaged(cppParameter);
            }
            else
            {
                context.WriteStringConvertToUnmanaged(cppParameter, paramFlags.HasFlag(ParameterFlags.Ref), GetConvertBackCondition(context, rootParameter, cppParameter, paramFlags));
            }
        }

        protected virtual string? GetConvertBackCondition(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags)
        {
            return null;
        }
    }
}