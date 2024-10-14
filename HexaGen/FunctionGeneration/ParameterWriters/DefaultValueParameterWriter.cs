namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen;
    using HexaGen.Core.CSharp;

    public readonly struct DefaultValueParameterWriter : IParameterWriter
    {
        public int Priority => 6 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.Default);
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
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