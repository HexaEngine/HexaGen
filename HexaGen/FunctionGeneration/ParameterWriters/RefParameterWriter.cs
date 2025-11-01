namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public class RefParameterWriter : IParameterWriter
    {
        public virtual int Priority => 4 * IParameterWriter.PriorityMultiplier;

        public virtual bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return (paramFlags & (ParameterFlags.Ref | ParameterFlags.Out | ParameterFlags.In)) != 0;
        }

        public virtual void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            var varName = context.UniqueName($"p{cppParameter.CleanName}");
            context.BeginBlock($"fixed ({cppParameter.Type.CleanName}* {varName} = &{cppParameter.Name})");
            context.AppendParam($"({rootParameter.Type.Name}){varName}");
        }
    }
}