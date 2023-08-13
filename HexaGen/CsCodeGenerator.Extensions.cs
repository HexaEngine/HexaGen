namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public partial class CsCodeGenerator
    {
        protected readonly HashSet<string> LibDefinedExtensions = new();
        public readonly HashSet<string> DefinedExtensions = new();

        protected virtual List<string> SetupExtensionUsings()
        {
            List<string> usings = new() { "System", "System.Runtime.CompilerServices", "System.Runtime.InteropServices", "HexaGen.Runtime" };
            usings.AddRange(settings.Usings);
            return usings;
        }

        protected virtual bool FilterExtensionType(GenContext context, CppTypedef typedef)
        {
            if (settings.IgnoredTypedefs.Contains(typedef.Name))
                return true;

            if (LibDefinedExtensions.Contains(typedef.Name))
                return true;

            if (typedef.ElementType is not CppPointerType)
            {
                return true;
            }

            if (typedef.IsDelegate())
            {
                return true;
            }

            return false;
        }

        protected virtual bool FilterExtensionFunction(GenContext context, CppFunction cppFunction, CppTypedef typedef, bool isCustomHandle)
        {
            if (settings.AllowedFunctions.Count != 0 && !settings.AllowedFunctions.Contains(cppFunction.Name))
                return true;
            if (settings.IgnoredFunctions.Contains(cppFunction.Name))
                return true;
            if (cppFunction.Parameters.Count == 0 || cppFunction.Parameters[0].Type.TypeKind == CppTypeKind.Pointer && !isCustomHandle)
                return true;

            if (cppFunction.Parameters[0].Type.GetDisplayName() == typedef.GetDisplayName())
            {
                return false;
            }

            return true;
        }

        protected virtual bool FilterExtension(GenContext context, HashSet<string> definedExtensions, string header)
        {
            if (definedExtensions.Contains(header))
            {
                LogWarn($"{context.FilePath}: {header} extension is already defined!");
                return true;
            }

            definedExtensions.Add(header);

            return false;
        }

        protected virtual void GenerateExtensions(CppCompilation compilation, string outputPath)
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
            }
        }

        protected virtual void WriteExtensionsForHandle(GenContext context, CppTypedef typedef, bool isCustomHandle = false)
        {
            if (FilterExtensionType(context, typedef))
                return;

            string handleName = typedef.Name;
            var compilation = context.Compilation;
            List<CsFunction> functions = new();
            for (int i = 0; i < compilation.Functions.Count; i++)
            {
                var cppFunction = compilation.Functions[i];

                if (FilterExtensionFunction(context, cppFunction, typedef, isCustomHandle))
                    continue;

                var extensionPrefix = settings.GetExtensionNamePrefix(handleName);

                var csFunctionName = settings.GetPrettyFunctionName(cppFunction.Name);
                var csName = settings.GetPrettyExtensionName(csFunctionName, extensionPrefix);

                CreateCsFunction(cppFunction, csName, functions, out var overload);
                funcGen.GenerateVariations(cppFunction.Parameters, overload, false);
                WriteExtensions(context, DefinedVariationsFunctions, csFunctionName, overload, "public static");
            }
        }

        protected virtual void WriteExtensions(GenContext context, HashSet<string> definedExtensions, string originalFunction, CsFunctionOverload overload, params string[] modifiers)
        {
            for (int j = 0; j < overload.Variations.Count; j++)
            {
                WriteExtension(context, definedExtensions, originalFunction, overload, overload.Variations[j], modifiers);
            }
        }

        protected virtual string BuildExtensionSignature(CsFunctionVariation variation)
        {
            var first = variation.Parameters[0];
            return string.Join(", ", variation.Parameters.Skip(1).Select(x => $"{string.Join(" ", x.Attributes)} {x.Type} {x.Name}").Reverse().Append($"this {first.Type} {first.Name}").Reverse());
        }

        protected virtual void WriteExtension(GenContext context, HashSet<string> definedExtensions, string originalFunction, CsFunctionOverload overload, CsFunctionVariation variation, string[] modifiers)
        {
            var writer = context.Writer;
            CsType csReturnType = variation.ReturnType;
            PrepareArgs(variation, csReturnType);

            string modifierString = string.Join(" ", modifiers);
            string signature = BuildExtensionSignature(variation);
            string header = $"{csReturnType.Name} {variation.Name}({signature})";

            string identifier = variation.BuildSignatureIdentifier();
            if (FilterExtension(context, definedExtensions, identifier))
                return;

            LogInfo("defined extension " + header);

            if (overload.Comment != null)
                writer.WriteLines(overload.Comment);

            writer.WriteLines(overload.Attributes);

            using (writer.PushBlock($"{modifierString} {header}"))
            {
                StringBuilder sb = new();
                bool firstParamReturn = false;
                if (!csReturnType.IsString && csReturnType.Name != overload.ReturnType.Name)
                {
                    firstParamReturn = true;
                }

                int offset = firstParamReturn ? 1 : 0;

                bool hasManaged = false;
                for (int j = 0; j < overload.Parameters.Count - offset; j++)
                {
                    var cppParameter = overload.Parameters[j + offset];
                    if (variation.HasParameter(cppParameter))
                        continue;
                    var paramCsDefault = overload.DefaultValues[cppParameter.Name];
                    if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
                        hasManaged = true;
                }

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

                sb.Append($"{settings.ApiName}.");

                if (hasManaged)
                    sb.Append($"{originalFunction}(");
                else if (firstParamReturn)
                    sb.Append($"{originalFunction}Native(&ret" + (overload.Parameters.Count > 1 ? ", " : ""));
                else
                    sb.Append($"{originalFunction}Native(");
                Stack<(string, CsParameterInfo, string)> stack = new();
                int strings = 0;
                Stack<string> arrays = new();
                int stacks = 0;

                for (int i = 0; i < overload.Parameters.Count - offset; i++)
                {
                    var cppParameter = overload.Parameters[i + offset];
                    var isRef = false;
                    var isPointer = false;
                    var isStr = false;
                    var isArray = false;
                    var isBool = false;
                    var isConst = true;

                    for (int j = 0; j < variation.Parameters.Count; j++)
                    {
                        var param = variation.Parameters[j];
                        if (param.Name == cppParameter.Name)
                        {
                            cppParameter = param;
                            isRef = param.Type.IsRef;
                            isPointer = param.Type.IsPointer;
                            isStr = param.Type.IsString;
                            isArray = param.Type.IsArray;
                            isBool = param.Type.IsBool;
                            isConst = false;
                        }
                    }

                    if (isConst)
                    {
                        var rootParam = overload.Parameters[i + offset];
                        var paramCsDefault = overload.DefaultValues[cppParameter.Name];
                        if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
                            sb.Append($"(string){paramCsDefault}");
                        else if (cppParameter.Type.IsBool && !cppParameter.Type.IsPointer && !cppParameter.Type.IsArray)
                            sb.Append($"({settings.GetBoolType()})({paramCsDefault})");
                        else if (rootParam.Type.IsEnum)
                            sb.Append($"({rootParam.Type.Name})({paramCsDefault})");
                        else if (cppParameter.Type.IsPrimitive || cppParameter.Type.IsPointer || cppParameter.Type.IsArray)
                            sb.Append($"({rootParam.Type.Name})({paramCsDefault})");
                        else
                            sb.Append($"{paramCsDefault}");
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
                        sb.Append($"({overload.Parameters[i + offset].Type.Name})p{cppParameter.Name}");
                        stacks++;
                    }
                    else if (isArray)
                    {
                        writer.BeginBlock($"fixed ({cppParameter.Type.CleanName}* p{cppParameter.Name} = {cppParameter.Name})");
                        sb.Append($"({overload.Parameters[i + offset].Type.Name})p{cppParameter.Name}");
                        stacks++;
                    }
                    else if (isBool && !isRef && !isPointer)
                    {
                        sb.Append($"{cppParameter.Name} ? ({settings.GetBoolType()})1 : ({settings.GetBoolType()})0");
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