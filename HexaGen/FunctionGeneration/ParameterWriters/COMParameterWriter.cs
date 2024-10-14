namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen.Core.CSharp;

    public struct COMParameterWriter : IParameterWriter
    {
        public readonly int Priority => 1 * IParameterWriter.PriorityMultiplier - 100;

        public readonly bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return (paramFlags & ParameterFlags.COMPtr) != 0;
        }

        public readonly void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.AppendParam($"({cppParameter.Type.Name}){cppParameter.Name}.GetAddressOf()");
        }
    }
}