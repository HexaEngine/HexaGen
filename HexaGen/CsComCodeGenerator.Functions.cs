namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using System.Collections.Generic;
    using System.Text;

    public partial class CsComCodeGenerator
    {
        protected override List<string> SetupFunctionUsings()
        {
            var usings = base.SetupFunctionUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }

        protected override void GenerateFunctions(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Functions.cs");
            DefinedVariationsFunctions.Clear();

            // Generate Functions
            using var writer = new CsCodeWriter(filePath, settings.Namespace, SetupFunctionUsings());
            GenContext context = new(compilation, filePath, writer);

            using (writer.PushBlock($"public unsafe partial class {settings.ApiName}"))
            {
                writer.WriteLine($"internal const string LibName = \"{settings.LibName}\";\n");
                List<CsFunction> functions = new();
                for (int i = 0; i < compilation.Functions.Count; i++)
                {
                    CppFunction? cppFunction = compilation.Functions[i];
                    if (FilterFunctionIgnored(context, cppFunction))
                    {
                        continue;
                    }

                    string? csName = settings.GetPrettyFunctionName(cppFunction.Name);
                    string returnCsName = settings.GetCsTypeName(cppFunction.ReturnType, false);
                    CppPrimitiveKind returnKind = cppFunction.ReturnType.GetPrimitiveKind();

                    bool boolReturn = returnCsName == "bool";
                    bool canUseOut = OutReturnFunctions.Contains(cppFunction.Name);
                    var argumentsString = settings.GetParameterSignature(cppFunction.Parameters, canUseOut, settings.GenerateMetadata);
                    var header = $"{returnCsName} {csName}Native({argumentsString})";
                    var headerId = $"{csName}({settings.GetParameterSignature(cppFunction.Parameters, canUseOut, false, false)})";

                    if (FilterNativeFunction(context, cppFunction, headerId))
                    {
                        continue;
                    }

                    settings.WriteCsSummary(cppFunction.Comment, writer);
                    if (settings.GenerateMetadata)
                    {
                        writer.WriteLine($"[NativeName(NativeNameType.Func, \"{cppFunction.Name}\")]");
                        writer.WriteLine($"[return: NativeName(NativeNameType.Type, \"{cppFunction.ReturnType.GetDisplayName()}\")]");
                    }

                    if (settings.UseLibraryImport)
                    {
                        writer.WriteLine($"[LibraryImport(LibName, EntryPoint = \"{cppFunction.Name}\")]");
                        writer.WriteLine($"[UnmanagedCallConv(CallConvs = new Type[] {{typeof({cppFunction.CallingConvention.GetCallingConventionLibrary()})}})]");
                    }
                    else
                    {
                        writer.WriteLine($"[DllImport(LibName, CallingConvention = CallingConvention.{cppFunction.CallingConvention.GetCallingConvention()}, EntryPoint = \"{cppFunction.Name}\")]");
                    }

                    if (boolReturn)
                    {
                        writer.WriteLine($"internal static extern {settings.GetBoolType()} {csName}Native({argumentsString});");
                        writer.WriteLine();
                    }
                    else
                    {
                        writer.WriteLine($"internal static extern {header};");
                        writer.WriteLine();
                    }

                    var function = CreateCsFunction(cppFunction, csName, functions, out var overload);
                    overload.Modifiers.Add("public");
                    overload.Modifiers.Add("static");
                    funcGen.GenerateCOMVariations(cppFunction.Parameters, overload, false);
                    WriteFunctions(context, DefinedVariationsFunctions, function, overload, WriteFunctionFlags.None, "public static");
                }
            }
        }

        protected override void WriteFunction(GenContext context, HashSet<string> definedFunctions, CsFunction function, CsFunctionOverload overload, CsFunctionVariation variation, WriteFunctionFlags flags, params string[] modifiers)
        {
            var writer = context.Writer;
            CsType csReturnType = variation.ReturnType;
            PrepareArgs(variation, csReturnType);

            string header = variation.BuildFullSignatureForCOM(settings.GenerateMetadata);// BuildFunctionHeader(variation, csReturnType, flags);
            string id = BuildFunctionHeaderId(variation, flags);

            if (FilterFunction(context, definedFunctions, id))
            {
                return;
            }

            ClassifyParameters(overload, variation, csReturnType, out bool firstParamReturn, out int offset, out bool hasManaged);

            LogInfo("defined function " + header);

            writer.WriteLines(overload.Comment);
            if (settings.GenerateMetadata)
            {
                writer.WriteLines(overload.Attributes);
            }

            using (writer.PushBlock($"{string.Join(" ", modifiers)} {header}"))
            {
                StringBuilder sb = new();

                if (!firstParamReturn && (!csReturnType.IsVoid || csReturnType.IsVoid && csReturnType.IsPointer))
                {
                    if (csReturnType.IsBool && !csReturnType.IsPointer && !hasManaged)
                    {
                        sb.Append($"{settings.GetBoolType()} ret = ");
                    }
                    else
                    {
                        sb.Append($"{csReturnType.Name} ret = ");
                    }
                }

                if (csReturnType.IsString)
                {
                    WriteStringConvertToManaged(sb, variation.ReturnType);
                }

                if (flags != WriteFunctionFlags.None)
                {
                    sb.Append($"{settings.ApiName}.");
                }

                if (hasManaged)
                {
                    sb.Append($"{overload.Name}(");
                }
                else if (firstParamReturn)
                {
                    sb.Append($"{overload.Name}Native(&ret" + (overload.Parameters.Count > 1 ? ", " : ""));
                }
                else
                {
                    sb.Append($"{overload.Name}Native(");
                }

                Stack<(string, CsParameterInfo, string)> strings = new();
                Stack<string> stringArrays = new();
                int stringCounter = 0;
                int blockCounter = 0;

                for (int i = 0; i < overload.Parameters.Count - offset; i++)
                {
                    var cppParameter = overload.Parameters[i + offset];
                    var paramFlags = ParameterFlags.None;

                    if (variation.TryGetParameter(cppParameter.Name, out var param))
                    {
                        paramFlags = param.Flags;
                        cppParameter = param;
                    }

                    if (flags.HasFlag(WriteFunctionFlags.UseHandle) && i == 0)
                    {
                        sb.Append("Handle");
                    }
                    else if (flags.HasFlag(WriteFunctionFlags.UseThis) && i == 0 && overload.Parameters[i].Type.IsPointer)
                    {
                        writer.BeginBlock($"fixed ({overload.Parameters[i].Type.Name} @this = &this)");
                        sb.Append("@this");
                        blockCounter++;
                    }
                    else if (flags.HasFlag(WriteFunctionFlags.UseThis) && i == 0)
                    {
                        sb.Append("this");
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Default))
                    {
                        var rootParam = overload.Parameters[i + offset];
                        var paramCsDefault = cppParameter.DefaultValue;
                        if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
                        {
                            sb.Append($"(string){paramCsDefault}");
                        }
                        else if (cppParameter.Type.IsBool && !cppParameter.Type.IsPointer && !cppParameter.Type.IsArray)
                        {
                            sb.Append($"({settings.GetBoolType()})({paramCsDefault})");
                        }
                        else if (rootParam.Type.IsEnum || cppParameter.Type.IsPrimitive || cppParameter.Type.IsPointer || cppParameter.Type.IsArray)
                        {
                            sb.Append($"({rootParam.Type.Name})({paramCsDefault})");
                        }
                        else
                        {
                            sb.Append($"{paramCsDefault}");
                        }
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.String))
                    {
                        if (paramFlags.HasFlag(ParameterFlags.Array))
                        {
                            WriteStringArrayConvertToUnmanaged(writer, cppParameter.Type, cppParameter.Name, stringArrays.Count);
                            sb.Append($"pStrArray{stringArrays.Count}");
                            stringArrays.Push(cppParameter.Name);
                        }
                        else
                        {
                            if (paramFlags.HasFlag(ParameterFlags.Ref))
                            {
                                strings.Push((cppParameter.Name, cppParameter, $"pStr{stringCounter}"));
                            }

                            WriteStringConvertToUnmanaged(writer, cppParameter.Type, cppParameter.Name, stringCounter);
                            sb.Append($"pStr{stringCounter}");
                            stringCounter++;
                        }
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Ref))
                    {
                        writer.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.Name} = &{cppParameter.Name})");
                        sb.Append($"({overload.Parameters[i + offset].Type.Name})p{cppParameter.Name}");
                        blockCounter++;
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Out))
                    {
                        writer.WriteLine($"{cppParameter.Name} = default;");
                        if (paramFlags.HasFlag(ParameterFlags.COMPtr))
                        {
                            sb.Append($"({overload.Parameters[i + 0].Type.Name}){cppParameter.Name}.GetAddressOf()");
                        }
                        else
                        {
                            sb.Append($"out {cppParameter.Name}");
                        }
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Array))
                    {
                        writer.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.Name} = {cppParameter.Name})");
                        sb.Append($"({overload.Parameters[i + offset].Type.Name})p{cppParameter.Name}");
                        blockCounter++;
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Bool) && !paramFlags.HasFlag(ParameterFlags.Ref) && !paramFlags.HasFlag(ParameterFlags.Pointer))
                    {
                        sb.Append($"{cppParameter.Name} ? ({settings.GetBoolType()})1 : ({settings.GetBoolType()})0");
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.COMPtr))
                    {
                        sb.Append($"({overload.Parameters[i + 0].Type.Name}){cppParameter.Name}.GetAddressOf()");
                    }
                    else
                    {
                        sb.Append(cppParameter.Name);
                    }

                    if (i != overload.Parameters.Count - 1 - offset)
                    {
                        sb.Append(", ");
                    }
                }

                if (csReturnType.IsString)
                {
                    sb.Append("));");
                }
                else
                {
                    sb.Append(");");
                }

                if (firstParamReturn)
                {
                    writer.WriteLine($"{csReturnType.Name} ret;");
                }
                writer.WriteLine(sb.ToString());

                while (strings.TryPop(out var stackItem))
                {
                    WriteStringConvertToManaged(writer, stackItem.Item2.Type, stackItem.Item1, stackItem.Item3);
                }

                while (stringArrays.TryPop(out var arrayName))
                {
                    WriteFreeUnmanagedStringArray(writer, arrayName, stringArrays.Count);
                }

                while (stringCounter > 0)
                {
                    stringCounter--;
                    WriteFreeString(writer, stringCounter);
                }

                if (firstParamReturn || !csReturnType.IsVoid || csReturnType.IsVoid && csReturnType.IsPointer)
                {
                    if (csReturnType.IsBool && !csReturnType.IsPointer && !hasManaged)
                    {
                        writer.WriteLine("return ret != 0;");
                    }
                    else
                    {
                        writer.WriteLine("return ret;");
                    }
                }

                while (blockCounter > 0)
                {
                    blockCounter--;
                    writer.EndBlock();
                }
            }

            writer.WriteLine();
        }
    }
}