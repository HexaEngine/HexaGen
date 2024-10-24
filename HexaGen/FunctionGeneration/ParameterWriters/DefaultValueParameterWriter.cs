namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public class DefaultValueParameterWriter : IParameterWriter
    {
        public virtual int Priority => 6 * IParameterWriter.PriorityMultiplier;

        public virtual bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.Default);
        }

        public virtual void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            var settings = context.Settings;
            var paramCsDefault = cppParameter.DefaultValue!;
            if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
            {
                context.AppendParam($"(string){paramCsDefault}");
            }
            else if (cppParameter.Type.IsBool && !cppParameter.Type.IsPointer && !cppParameter.Type.IsArray)
            {
                context.AppendParam($"({settings.GetBoolType()})({paramCsDefault})");
            }
            else if (rootParameter.Type.IsEnum || cppParameter.Type.IsPrimitive || cppParameter.Type.IsPointer || cppParameter.Type.IsArray)
            {
                context.AppendParam($"({rootParameter.Type.Name})({paramCsDefault})");
            }
            else
            {
                context.AppendParam($"{paramCsDefault}");
            }
        }
    }
}