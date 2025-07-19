namespace HexaGen
{
    using HexaGen.Core;
    using HexaGen.Core.CSharp;
    using System.Collections.Generic;
    using System.Text;

    public class FunctionWriterContext
    {
        public ICodeWriter Writer { get; }

        public IGeneratorConfig Settings { get; }

        public StringBuilder StringBuilder { get; }

        public CsFunctionOverload Overload { get; }

        public CsFunctionVariation Variation { get; }

        public WriteFunctionFlags Flags { get; }

        private Stack<(string paramName, CsParameterInfo param, string varName, string? convertBackCondition)> strings = new();
        private Stack<(string paramName, CsParameterInfo param, string varName)> stringArrays = new();
        private int stringCounter = 0;
        private int blockCounter = 0;

        public FunctionWriterContext(ICodeWriter writer, IGeneratorConfig settings, StringBuilder stringBuilder, CsFunctionOverload overload, CsFunctionVariation variation, WriteFunctionFlags flags)
        {
            Writer = writer;
            Settings = settings;
            StringBuilder = stringBuilder;
            Overload = overload;
            Variation = variation;
            Flags = flags;
        }

        public void AppendParam(string param)
        {
            StringBuilder.Append(param);
        }

        public int BeginBlock(string text)
        {
            Writer.BeginBlock(text);
            return IncrementBlockCounter();
        }

        public int EndBlock()
        {
            Writer.EndBlock();
            return DecrementBlockCounter();
        }

        private int IncrementBlockCounter()
        {
            return blockCounter++;
        }

        public void WriteStringArrayConvertToUnmanaged(CsParameterInfo parameter)
        {
            MarshalHelper.WriteStringArrayConvertToUnmanaged(Writer, parameter.Type, parameter.Name, $"pStrArray{stringArrays.Count}");
            AppendParam($"pStrArray{stringArrays.Count}");
            PushStringArray(parameter.Name, parameter, $"pStrArray{stringArrays.Count}");
        }

        public void PushStringArray(string paramName, CsParameterInfo parameter, string varName)
        {
            stringArrays.Push((paramName, parameter, varName));
        }

        public void WriteStringConvertToUnmanaged(CsParameterInfo parameter, bool isRef, string? convertBackCondition = null)
        {
            // Lightweight branch for readonly strings.
            if (!isRef && parameter.Type.StringType == CsStringType.StringUTF16)
            {
                var varName = UniqueName($"p{parameter.CleanName}");
                BeginBlock($"fixed (char* {varName} = {parameter.Name})");
                AppendParam(varName);
                return;
            }

            int stringCounter = IncrementStringCounter();
            if (isRef)
            {
                PushString(parameter.Name, parameter, $"pStr{stringCounter}", convertBackCondition);
            }

            MarshalHelper.WriteStringConvertToUnmanaged(Writer, parameter.Type, parameter.Name, stringCounter);
            AppendParam($"pStr{stringCounter}");
        }

        public void PushString(string paramName, CsParameterInfo parameter, string varName, string? convertBackCondition)
        {
            strings.Push((paramName, parameter, varName, convertBackCondition));
        }

        public int IncrementStringCounter()
        {
            return stringCounter++;
        }

        public bool TryPopString(out (string paramName, CsParameterInfo param, string varName, string? convertBackCondition) stackItem)
        {
            return strings.TryPop(out stackItem);
        }

        public bool TryPopStringArray(out (string paramName, CsParameterInfo param, string varName) stackItem)
        {
            return stringArrays.TryPop(out stackItem);
        }

        public int StringCounter => stringCounter;

        public int BlockCounter => blockCounter;

        public int DecrementBlockCounter()
        {
            return blockCounter--;
        }

        public int DecrementStringCounter()
        {
            return stringCounter--;
        }

        public void ConvertStrings()
        {
            while (strings.TryPop(out var stackItem))
            {
                MarshalHelper.WriteStringConvertToManaged(Writer, stackItem.param.Type, stackItem.paramName, stackItem.varName, stackItem.convertBackCondition);
            }
        }

        public void FreeStringArrays()
        {
            while (stringArrays.TryPop(out var stackItem))
            {
                MarshalHelper.WriteFreeUnmanagedStringArray(Writer, stackItem.paramName, stackItem.varName);
            }
        }

        public void FreeStrings()
        {
            while (stringCounter > 0)
            {
                stringCounter--;
                MarshalHelper.WriteFreeString(Writer, stringCounter);
            }
        }

        public void EndBlocks()
        {
            while (blockCounter > 0)
            {
                blockCounter--;
                Writer.EndBlock();
            }
        }

        public string UniqueName(string name)
        {
            var nname = name;
            int i = 0;
            while (true)
            {
                bool found = false;
                foreach (var param in Variation.Parameters)
                {
                    if (param.DefaultValue == null && param.CleanName == nname)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    break;
                }

                nname = $"{name}{i}";
                i++;
            }

            return nname;
        }
    }
}