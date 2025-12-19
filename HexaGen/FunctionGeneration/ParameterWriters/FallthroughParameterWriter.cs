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
            if (rootParameter.Type.Name != cppParameter.Type.Name)
            {
                context.AppendParam($"({rootParameter.Type.Name}){cppParameter.Name}");
            }
            else
            {
                context.AppendParam(cppParameter.Name);
            }
        }
    }
}