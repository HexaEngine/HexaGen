namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public class FallthroughParameterWriter : IParameterWriter
    {
        public virtual int Priority => int.MinValue;

        public virtual bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return true;
        }

        public virtual void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.AppendParam(cppParameter.Name);
        }
    }
}