namespace HexaGen
{
    using HexaGen.Core.CSharp;

    public readonly struct ParameterPriorityComparer : IComparer<IParameterWriter>
    {
        public int Compare(IParameterWriter? x, IParameterWriter? y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            return y.Priority.CompareTo(x.Priority);
        }
    }

    public interface IParameterWriter
    {
        int Priority { get; }

        bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset);

        void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset);

        public const int PriorityMultiplier = 10000;
    }

    public readonly struct HandleParameterWriter : IParameterWriter
    {
        public int Priority => 8 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            var writeFunctionFlags = context.Flags;
            return writeFunctionFlags.HasFlag(WriteFunctionFlags.UseHandle) && index == 0;
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.AppendParam("Handle");
        }
    }

    public readonly struct UseThisParameterWriter : IParameterWriter
    {
        public int Priority => 7 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            var writeFunctionFlags = context.Flags;
            return writeFunctionFlags.HasFlag(WriteFunctionFlags.UseThis) && index == 0;
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            var overload = context.Overload;
            if (overload.Parameters[index].Type.IsPointer)
            {
                context.BeginBlock($"fixed ({overload.Parameters[index].Type.Name} @this = &this)");
                context.AppendParam("@this");
            }
            else
            {
                context.AppendParam("this");
            }
        }
    }

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
            var paramCsDefault = cppParameter.DefaultValue;
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

    public readonly struct StringParameterWriter : IParameterWriter
    {
        public int Priority => 5 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.String);
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            if (paramFlags.HasFlag(ParameterFlags.Array))
            {
                context.WriteStringArrayConvertToUnmanaged(cppParameter);
            }
            else
            {
                context.WriteStringConvertToUnmanaged(cppParameter, paramFlags.HasFlag(ParameterFlags.Ref));
            }
        }
    }

    public readonly struct RefParameterWriter : IParameterWriter
    {
        public int Priority => 4 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.Ref);
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.CleanName} = &{cppParameter.Name})");
            context.AppendParam($"({rootParameter.Type.Name})p{cppParameter.CleanName}");
        }
    }

    public readonly struct SpanParameterWriter : IParameterWriter
    {
        public int Priority => 3 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.Span);
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.CleanName} = {cppParameter.Name})");
            context.AppendParam($"({rootParameter.Type.Name})p{cppParameter.CleanName}");
        }
    }

    public readonly struct ArrayParameterWriter : IParameterWriter
    {
        public int Priority => 2 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.Array);
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.CleanName} = {cppParameter.Name})");
            context.AppendParam($"({rootParameter.Type.Name})p{cppParameter.CleanName}");
        }
    }

    public readonly struct BoolParameterWriter : IParameterWriter
    {
        public int Priority => 1 * IParameterWriter.PriorityMultiplier;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return paramFlags.HasFlag(ParameterFlags.Bool) && !paramFlags.HasFlag(ParameterFlags.Ref) && !paramFlags.HasFlag(ParameterFlags.Pointer);
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            var settings = context.Settings;
            context.AppendParam($"{cppParameter.Name} ? ({settings.GetBoolType()})1 : ({settings.GetBoolType()})0");
        }
    }

    public readonly struct FallthroughParameterWriter : IParameterWriter
    {
        public int Priority => int.MinValue;

        public bool CanWrite(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            return true;
        }

        public void Write(FunctionWriterContext context, CsParameterInfo rootParameter, CsParameterInfo cppParameter, ParameterFlags paramFlags, int index, int offset)
        {
            context.AppendParam(cppParameter.Name);
        }
    }
}