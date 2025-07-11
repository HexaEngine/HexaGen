namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen.Core.CSharp;

    public class COMParameterWriter : IParameterWriter
    {
        public virtual int Priority => 1 * IParameterWriter.PriorityMultiplier - 100;

        public virtual bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return (paramFlags & ParameterFlags.COMPtr) != 0;
        }

        public virtual void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            int count = rootParameter.Type.Name.AsSpan().Count('*');
            switch (count)
            {
                case 1:
                    context.AppendParam($"({rootParameter.Type.Name}){cppParameter.Name}.Handle");
                    break;

                default:
                    context.AppendParam($"({rootParameter.Type.Name}){cppParameter.Name}.GetAddressOf()");
                    break;
            }
        }
    }
}