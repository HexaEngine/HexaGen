﻿namespace HexaGen.GenerationSteps
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using HexaGen.Metadata;
    using System.Collections.Generic;
    using System.Text;

    public class ComExtensionGenerationStep : ExtensionGenerationStep
    {
        public readonly Dictionary<string, HashSet<string>> MemberFunctions = new();
        public readonly HashSet<string> DefinedTypes = new();
        protected readonly HashSet<string> LibDefinedTypes = new();
        private readonly CsComCodeGenerator comGenerator;

        public ComExtensionGenerationStep(CsComCodeGenerator generator, CsCodeGeneratorConfig config) : base(generator, config)
        {
            comGenerator = generator;
        }

        protected override List<string> SetupExtensionUsings()
        {
            var usings = base.SetupExtensionUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }

        protected virtual bool FilterCOMClassType(GenContext context, CppClass cppClass)
        {
            if (config.AllowedTypes.Count != 0 && !config.AllowedTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (config.IgnoredTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (LibDefinedTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (!comGenerator.HasGUID(cppClass.Name) && (cppClass.Fields.Count != 0 || cppClass.Functions.Count == 0 || !cppClass.IsAbstract))
            {
                return true;
            }

            return false;
        }

        protected virtual bool FilterCOMExtensionFunction(GenContext context, CppFunction cppFunction)
        {
            if (config.AllowedFunctions.Count != 0 && !config.AllowedFunctions.Contains(cppFunction.Name))
            {
                return true;
            }

            if (config.IgnoredFunctions.Contains(cppFunction.Name))
            {
                return true;
            }

            return false;
        }

        public override void Generate(CppCompilation compilation, string outputPath, CsCodeGeneratorConfig config, CsCodeGeneratorMetadata metadata)
        {
            string filePath = Path.Combine(outputPath, "Extensions.cs");

            // Generate Extensions
            using var writer = new CsSplitCodeWriter(filePath, config.Namespace, SetupExtensionUsings(), config.HeaderInjector);
            GenContext context = new(compilation, filePath, writer);

            using (writer.PushBlock($"public static unsafe partial class Extensions"))
            {
                for (int i = 0; i < compilation.Typedefs.Count; i++)
                {
                    WriteExtensionsForHandle(context, compilation.Typedefs[i]);
                }

                for (int i = 0; i < compilation.Classes.Count; i++)
                {
                    WriteExtensionsForCOMObject(context, compilation.Classes[i]);
                }
            }
        }

        protected virtual void WriteExtensionsForCOMObject(GenContext context, CppClass cppClass)
        {
            if (FilterCOMClassType(context, cppClass))
                return;

            string csName = config.GetCsCleanName(cppClass.Name);
            var mapping = config.GetTypeMapping(cppClass.Name);
            csName = mapping?.FriendlyName ?? csName;

            int vTableIndex = 0;
            WriteExtensionsForCOMObject(context, cppClass, cppClass, csName, ref vTableIndex);
        }

        protected virtual void WriteExtensionsForCOMObject(GenContext context, CppClass targetClass, CppClass cppClass, string className, ref int vTableIndex)
        {
            for (int i = 0; i < cppClass.BaseTypes.Count; i++)
            {
                var baseType = cppClass.BaseTypes[i];
                if (baseType.Type is CppClass baseClass)
                {
                    if (FilterCOMClassType(context, baseClass))
                    {
                        continue;
                    }

                    WriteExtensionsForCOMObject(context, targetClass, baseClass, className, ref vTableIndex);
                }
            }

            List<CsFunction> commands = new();
            for (int i = 0; i < cppClass.Functions.Count; i++, vTableIndex++)
            {
                var cppFunction = cppClass.Functions[i];

#if CPPAST_15_OR_GREATER
                if (cppFunction.IsFunctionTemplate)
                {
                    vTableIndex--;
                    continue;
                }
#endif

                if (FilterCOMExtensionFunction(context, cppFunction))
                {
                    continue;
                }

                var extensionPrefix = config.GetExtensionNamePrefix(cppClass.Name);

                var csFunctionName = config.GetCsFunctionName(cppFunction.Name);
                var csName = config.GetExtensionName(csFunctionName, extensionPrefix);

                generator.CreateCsFunction(cppFunction, CsFunctionKind.Extension, csName, commands, out var overload);
                funcGen.GenerateVariations(cppFunction.Parameters, overload);

                if (!MemberFunctions.TryGetValue(targetClass.Name, out var definedExtensions))
                {
                    definedExtensions = new();
                    MemberFunctions.Add(targetClass.Name, definedExtensions);
                }

                if (!WriteCOMExtensions(context, definedExtensions, overload, className, vTableIndex, "public static"))
                {
                    vTableIndex--;
                }
            }
        }

        protected virtual bool WriteCOMExtensions(GenContext context, HashSet<string> definedExtensions, CsFunctionOverload overload, string className, int index, params string[] modifiers)
        {
            bool hasWritten = false;
            for (int i = 0; i < overload.Variations.Count; i++)
            {
                hasWritten |= WriteCOMExtension(context, definedExtensions, overload, overload.Variations[i], className, index, modifiers);
            }
            return hasWritten;
        }

        protected virtual bool WriteCOMExtension(GenContext context, HashSet<string> definedExtensions, CsFunctionOverload overload, CsFunctionVariation variation, string className, int index, string[] modifiers)
        {
            var writer = context.Writer;
            CsType csReturnType = variation.ReturnType;

            generator.PrepareArgs(variation, csReturnType);

            string modifierString = string.Join(" ", modifiers);
            string header = variation.BuildFullExtensionSignatureForCOM(className, config.GenerateMetadata);
            string signatureNameless = overload.BuildSignatureNamelessForCOM(className, config);

            string id = variation.BuildExtensionSignatureIdentifierForCOM(className);
            if (FilterExtension(context, definedExtensions, id))
            {
                return false;
            }

            LogInfo("defined extension " + header);

            writer.WriteLines(overload.Comment);

            if (config.GenerateMetadata)
            {
                writer.WriteLines(overload.Attributes);
            }

            using (writer.PushBlock($"{modifierString} {header}"))
            {
                writer.WriteLine($"{className}* handle = comObj.Handle;");
                StringBuilder sb = new();

                bool hasManaged = false;
                for (int j = 0; j < overload.Parameters.Count - 0; j++)
                {
                    var cppParameter = overload.Parameters[j + 0];
                    if (variation.HasParameter(cppParameter))
                    {
                        continue;
                    }

                    if (!overload.DefaultValues.TryGetValue(cppParameter.Name, out string? paramCsDefault))
                    {
                        continue;
                    }

                    if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
                    {
                        hasManaged = true;
                    }
                }

                if (!csReturnType.IsVoid || csReturnType.IsVoid && csReturnType.IsPointer)
                {
                    if (csReturnType.IsBool && !csReturnType.IsPointer && !hasManaged)
                    {
                        sb.Append($"{config.GetBoolType()} ret = ");
                    }
                    else
                    {
                        sb.Append($"{csReturnType.Name} ret = ");
                    }
                }

                if (csReturnType.IsString)
                {
                    MarshalHelper.WriteStringConvertToManaged(sb, variation.ReturnType);
                }

                var retType = csReturnType.IsBool ? config.GetBoolType() : csReturnType.Name;
                var ptr = index == 0 ? "*handle->LpVtbl" : $"handle->LpVtbl[{index}]";
                var tail = variation.Parameters.Count > 0 ? ", " : string.Empty;

                sb.Append($"((delegate* unmanaged[Stdcall]<{signatureNameless}, {retType}>)({ptr}))(handle{tail}");

                Stack<(string, CsParameterInfo, string)> stack = new();
                int strings = 0;
                Stack<string> arrays = new();
                int stacks = 0;

                for (int i = 0; i < overload.Parameters.Count - 0; i++)
                {
                    var cppParameter = overload.Parameters[i + 0];
                    var paramFlags = ParameterFlags.None;

                    if (variation.TryGetParameter(cppParameter.Name, out var param))
                    {
                        paramFlags = param.Flags;
                        cppParameter = param;
                    }

                    if (paramFlags.HasFlag(ParameterFlags.Default))
                    {
                        var rootParam = overload.Parameters[i + 0];
                        var paramCsDefault = cppParameter.DefaultValue!;
                        if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
                        {
                            sb.Append($"(string){paramCsDefault}");
                        }
                        else if (cppParameter.Type.IsBool && !cppParameter.Type.IsPointer && !cppParameter.Type.IsArray)
                        {
                            sb.Append($"({config.GetBoolType()})({paramCsDefault})");
                        }
                        else if (rootParam.Type.IsEnum)
                        {
                            sb.Append($"({rootParam.Type.Name})({paramCsDefault})");
                        }
                        else if (cppParameter.Type.IsPrimitive || cppParameter.Type.IsPointer || cppParameter.Type.IsArray)
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
                            MarshalHelper.WriteStringArrayConvertToUnmanaged(writer, cppParameter.Type, cppParameter.Name, arrays.Count);
                            sb.Append($"pStrArray{arrays.Count}");
                            arrays.Push(cppParameter.Name);
                        }
                        else
                        {
                            if (paramFlags.HasFlag(ParameterFlags.Ref))
                            {
                                stack.Push((cppParameter.Name, cppParameter, $"pStr{strings}"));
                            }

                            MarshalHelper.WriteStringConvertToUnmanaged(writer, cppParameter.Type, cppParameter.Name, strings);
                            sb.Append($"pStr{strings}");
                            strings++;
                        }
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Ref))
                    {
                        writer.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.Name.Replace("@", string.Empty)} = &{cppParameter.Name})");
                        sb.Append($"({overload.Parameters[i + 0].Type.Name})p{cppParameter.Name}");
                        stacks++;
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
                        sb.Append($"({overload.Parameters[i + 0].Type.Name})p{cppParameter.Name}");
                        stacks++;
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.Bool) && !paramFlags.HasFlag(ParameterFlags.Ref) && !paramFlags.HasFlag(ParameterFlags.Pointer))
                    {
                        sb.Append($"{cppParameter.Name} ? ({config.GetBoolType()})1 : ({config.GetBoolType()})0");
                    }
                    else if (paramFlags.HasFlag(ParameterFlags.COMPtr))
                    {
                        if (paramFlags.HasFlag(ParameterFlags.Ref))
                        {
                            sb.Append($"({overload.Parameters[i + 0].Type.Name}){cppParameter.Name}.GetAddressOf()");
                        }
                        else
                        {
                            sb.Append($"({overload.Parameters[i + 0].Type.Name}){cppParameter.Name}.Handle");
                        }
                    }
                    else
                    {
                        sb.Append(cppParameter.Name);
                    }

                    if (i != overload.Parameters.Count - 1 - 0)
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

                writer.WriteLine(sb.ToString());

                while (stack.TryPop(out var stackItem))
                {
                    MarshalHelper.WriteStringConvertToManaged(writer, stackItem.Item2.Type, stackItem.Item1, stackItem.Item3);
                }

                while (arrays.TryPop(out var arrayName))
                {
                    MarshalHelper.WriteFreeUnmanagedStringArray(writer, arrayName, arrays.Count);
                }

                while (strings > 0)
                {
                    strings--;
                    MarshalHelper.WriteFreeString(writer, strings);
                }

                if (!csReturnType.IsVoid || csReturnType.IsVoid && csReturnType.IsPointer)
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

                while (stacks > 0)
                {
                    stacks--;
                    writer.EndBlock();
                }
            }

            writer.WriteLine();
            return true;
        }
    }
}