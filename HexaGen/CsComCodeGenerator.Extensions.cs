namespace HexaGen
{
    using ClangSharp;
    using CppAst;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public partial class CsComCodeGenerator
    {
        protected override List<string> SetupExtensionUsings()
        {
            var usings = base.SetupExtensionUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }

        protected virtual bool FilterCOMClassType(GenContext context, CppClass cppClass)
        {
            if (settings.AllowedTypes.Count != 0 && !settings.AllowedTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (settings.IgnoredTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (LibDefinedTypes.Contains(cppClass.Name))
            {
                return true;
            }

            if (!HasGUID(cppClass.Name) && (cppClass.Fields.Count != 0 || cppClass.Functions.Count == 0 || !cppClass.IsAbstract))
            {
                return true;
            }

            return false;
        }

        protected virtual bool FilterCOMExtensionFunction(GenContext context, CppFunction cppFunction)
        {
            if (settings.AllowedFunctions.Count != 0 && !settings.AllowedFunctions.Contains(cppFunction.Name))
            {
                return true;
            }

            if (settings.IgnoredFunctions.Contains(cppFunction.Name))
            {
                return true;
            }

            return false;
        }

        protected override void GenerateExtensions(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Extensions.cs");

            // Generate Extensions
            using var writer = new CodeWriter(filePath, settings.Namespace, SetupExtensionUsings());
            GenContext context = new(compilation, filePath, writer);

            using (writer.PushBlock($"public static unsafe class Extensions"))
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

            string csName = settings.GetCsCleanName(cppClass.Name);
            var mapping = settings.GetTypeMapping(cppClass.Name);
            csName = mapping?.FriendlyName ?? csName;

            int vTableIndex = 0;
            WriteExtensionsForCOMObject(context, cppClass, csName, ref vTableIndex);
        }

        protected virtual void WriteExtensionsForCOMObject(GenContext context, CppClass cppClass, string className, ref int vTableIndex)
        {
            for (int i = 0; i < cppClass.BaseTypes.Count; i++)
            {
                var baseType = cppClass.BaseTypes[i];
                if (baseType.Type is CppClass baseClass)
                {
                    // TODO: change to FilterCOMClassType, but first a bug needs to be fixed.
                    if (HasGUID(baseClass.Name))
                    {
                        WriteExtensionsForCOMObject(context, baseClass, className, ref vTableIndex);
                    }
                    // TODO: remove this dirty fix.
                    if (!FilterCOMClassType(context, baseClass))
                    {
                        for (int j = 0; j < baseClass.Functions.Count; j++)
                        {
                            if (!baseClass.Functions[j].IsFunctionTemplate)
                                vTableIndex++;
                        }
                    }
                }
            }

            List<CsFunction> commands = new();
            for (int i = 0; i < cppClass.Functions.Count; i++, vTableIndex++)
            {
                var cppFunction = cppClass.Functions[i];

                if (FilterCOMExtensionFunction(context, cppFunction))
                    continue;

                var extensionPrefix = settings.GetExtensionNamePrefix(cppClass.Name);

                var csFunctionName = settings.GetPrettyFunctionName(cppFunction.Name);
                var csName = settings.GetPrettyExtensionName(csFunctionName, extensionPrefix);

                CreateCsFunction(cppFunction, csName, commands, out var overload);
                funcGen.GenerateCOMVariations(cppFunction.Parameters, overload, false);
                WriteCOMExtensions(context, DefinedVariationsFunctions, overload, className, vTableIndex, "public static");
            }
        }

        protected virtual void WriteCOMExtensions(GenContext context, HashSet<string> definedExtensions, CsFunctionOverload overload, string className, int index, params string[] modifiers)
        {
            for (int j = 0; j < overload.Variations.Count; j++)
            {
                WriteCOMExtension(context, definedExtensions, overload, overload.Variations[j], className, index, modifiers);
            }
        }

        protected virtual void WriteCOMExtension(GenContext context, HashSet<string> definedExtensions, CsFunctionOverload overload, CsFunctionVariation variation, string className, int index, string[] modifiers)
        {
            var writer = context.Writer;
            CsType csReturnType = variation.ReturnType;
            if (WrappedPointers.TryGetValue(csReturnType.Name, out string? value))
            {
                csReturnType.Name = value;
            }

            for (int i = 0; i < variation.Parameters.Count; i++)
            {
                var cppParameter = variation.Parameters[i];
                if (WrappedPointers.TryGetValue(cppParameter.Type.Name, out string? v))
                {
                    cppParameter.Type.Name = v;
                    cppParameter.Type.Classify();
                }
            }

            string modifierString = string.Join(" ", modifiers);
            string signature;
            string genericSignature = string.Join(", ", variation.GenericParameters.Select(p => p.Name));
            string genericConstrain = string.Join(" ", variation.GenericParameters.Select(p => p.Constrain));
            string signatureNameless = $"{className}*{(overload.Parameters.Count > 0 ? ", " : string.Empty)}";

            signature = string.Join(", ", variation.Parameters.Select(x => $"{string.Join(" ", x.Attributes)} {x.Type} {x.Name}").Reverse().Append($"this ComPtr<{className}> comObj").Reverse());
            signatureNameless += string.Join(", ", overload.Parameters.Select(x => $"{(x.Type.IsBool ? settings.GetBoolType() : x.Type.Name)}"));

            string header = $"{csReturnType.Name} {variation.Name}{(variation.IsGeneric ? $"<{genericSignature}>" : string.Empty)}({signature}) {genericConstrain}";

            if (FilterExtension(context, definedExtensions, header))
                return;

            LogInfo("defined extension " + header);

            if (overload.Comment != null)
            {
                writer.WriteLines(overload.Comment);
            }

            for (int i = 0; i < overload.Attributes.Count; i++)
            {
                writer.WriteLine(overload.Attributes[i]);
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

                if ((!csReturnType.IsVoid || csReturnType.IsVoid && csReturnType.IsPointer))
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

                var retType = csReturnType.IsBool ? settings.GetBoolType() : csReturnType.Name;
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
                    var isOut = false;
                    var isRef = false;
                    var isPointer = false;
                    var isStr = false;
                    var isArray = false;
                    var isBool = false;
                    var isConst = true;
                    var isIID = false;
                    var isCOMPtr = false;

                    for (int j = 0; j < variation.Parameters.Count; j++)
                    {
                        var param = variation.Parameters[j];
                        if (param.Name == cppParameter.Name)
                        {
                            cppParameter = param;
                            isOut = param.Type.IsOut;
                            isRef = param.Type.IsRef;
                            isPointer = param.Type.IsPointer;
                            isStr = param.Type.IsString;
                            isArray = param.Type.IsArray;
                            isBool = param.Type.IsBool;
                            isIID = param.Type.Name.Contains("Guid*");
                            isCOMPtr = param.Type.Name.Contains("ComPtr<");
                            isConst = false;
                            break;
                        }
                    }

                    if (isConst)
                    {
                        var rootParam = overload.Parameters[i + 0];
                        if (!overload.DefaultValues.TryGetValue(cppParameter.Name, out string? paramCsDefault))
                        {
                            if (isIID)
                            {
                                sb.Append($"ComUtils.GuidPtrOf<T>()");
                            }
                            continue;
                        }
                        if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
                        {
                            sb.Append($"(string){paramCsDefault}");
                        }
                        else if (cppParameter.Type.IsBool && !cppParameter.Type.IsPointer && !cppParameter.Type.IsArray)
                        {
                            sb.Append($"({settings.GetBoolType()})({paramCsDefault})");
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
                    else if (isStr)
                    {
                        if (isArray)
                        {
                            WriteStringArrayConvertToUnmanaged(writer, cppParameter.Type, cppParameter.Name, arrays.Count);
                            sb.Append($"pStrArray{arrays.Count}");
                            arrays.Push(cppParameter.Name);
                        }
                        else
                        {
                            if (isRef)
                            {
                                stack.Push((cppParameter.Name, cppParameter, $"pStr{strings}"));
                            }

                            WriteStringConvertToUnmanaged(writer, cppParameter.Type, cppParameter.Name, strings);
                            sb.Append($"pStr{strings}");
                            strings++;
                        }
                    }
                    else if (isRef)
                    {
                        writer.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.Name} = &{cppParameter.Name})");
                        sb.Append($"({overload.Parameters[i + 0].Type.Name})p{cppParameter.Name}");
                        stacks++;
                    }
                    else if (isOut)
                    {
                        writer.WriteLine($"{cppParameter.Name} = default;");
                        if (isCOMPtr)
                        {
                            sb.Append($"({overload.Parameters[i + 0].Type.Name}){cppParameter.Name}.GetAddressOf()");
                        }
                        else
                        {
                            sb.Append($"out {cppParameter.Name}");
                        }
                    }
                    else if (isArray)
                    {
                        writer.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.Name} = {cppParameter.Name})");
                        sb.Append($"({overload.Parameters[i + 0].Type.Name})p{cppParameter.Name}");
                        stacks++;
                    }
                    else if (isBool && !isRef && !isPointer)
                    {
                        sb.Append($"{cppParameter.Name} ? ({settings.GetBoolType()})1 : ({settings.GetBoolType()})0");
                    }
                    else if (isCOMPtr)
                    {
                        sb.Append($"({overload.Parameters[i + 0].Type.Name}){cppParameter.Name}.GetAddressOf()");
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
                    WriteStringConvertToManaged(writer, stackItem.Item2.Type, stackItem.Item1, stackItem.Item3);
                }

                while (arrays.TryPop(out var arrayName))
                {
                    WriteFreeUnmanagedStringArray(writer, arrayName, arrays.Count);
                }

                while (strings > 0)
                {
                    strings--;
                    WriteFreeString(writer, strings);
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
        }
    }
}