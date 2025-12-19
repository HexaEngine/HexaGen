namespace HexaGen.FunctionGeneration.ParameterWriters
{
    using HexaGen.Core.CSharp;

    public class DelegateParameterWriter : IParameterWriter
    {
        public int Priority { get; } = 3 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return rootParameter.CppType.IsDelegate() && !cppParameter.Type.IsPointer && !cppParameter.Type.IsRef && !cppParameter.Type.IsIn;
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            rootParameter.CppType.IsDelegate(out var cppFunction);
            var config = context.Config;
            var ptrType = config.MakeDelegatePointer(cppFunction!);
            context.AppendParam($"({ptrType})Utils.GetFunctionPointerForDelegate({cppParameter.Name})");
        }
    }
}