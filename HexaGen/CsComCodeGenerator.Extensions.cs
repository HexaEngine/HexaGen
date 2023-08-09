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
        private readonly HashSet<string> LibDefinedExtensions = new();
        public readonly HashSet<string> DefinedExtensions = new();

        private void GenerateExtensions(CppCompilation compilation, string outputPath)
        {
            string filePath = Path.Combine(outputPath, "Extensions.cs");
            string[] usings = { "System", "System.Runtime.CompilerServices", "System.Runtime.InteropServices", "HexaGen.Runtime", "HexaGen.Runtime.COM" };

            // Generate Extensions
            using var writer = new CodeWriter(filePath, settings.Namespace, usings.Concat(settings.Usings).ToArray());
            using (writer.PushBlock($"public static unsafe class Extensions"))
            {
                for (int i = 0; i < compilation.Typedefs.Count; i++)
                {
                    CppTypedef typedef = compilation.Typedefs[i];
                    if (settings.IgnoredTypedefs.Contains(typedef.Name))
                    {
                        continue;
                    }

                    if (LibDefinedExtensions.Contains(typedef.Name))
                    {
                        continue;
                    }

                    if (typedef.ElementType is not CppPointerType)
                    {
                        continue;
                    }

                    if (typedef.IsDelegate())
                    {
                        continue;
                    }

                    WriteExtensionsForHandle(compilation, writer, typedef, typedef.Name);
                }

                for (int i = 0; i < compilation.Classes.Count; i++)
                {
                    CppClass cppClass = compilation.Classes[i];
                    if (settings.AllowedTypes.Count != 0 && !settings.AllowedTypes.Contains(cppClass.Name))
                    {
                        continue;
                    }

                    if (settings.IgnoredTypes.Contains(cppClass.Name))
                    {
                        continue;
                    }

                    if (LibDefinedTypes.Contains(cppClass.Name))
                    {
                        continue;
                    }

                    if (!HasGUID(cppClass.Name) && (cppClass.Fields.Count != 0 || cppClass.Functions.Count == 0 || !cppClass.IsAbstract))
                    {
                        continue;
                    }

                    string csName = settings.GetCsCleanName(cppClass.Name);

                    var mapping = settings.GetTypeMapping(cppClass.Name);

                    csName = mapping?.FriendlyName ?? csName;

                    int vTableIndex = 0;
                    WriteExtensionsForCOMObject(writer, DefinedExtensions, cppClass, csName, ref vTableIndex);
                }
            }
        }

        private void WriteExtensionsForHandle(CppCompilation compilation, CodeWriter writer, CppType typedef, string handleName, bool isCustomHandle = false)
        {
            List<CsFunction> commands = new();
            for (int i = 0; i < compilation.Functions.Count; i++)
            {
                var cppFunction = compilation.Functions[i];
                if (settings.AllowedFunctions.Count != 0 && !settings.AllowedFunctions.Contains(cppFunction.Name))
                {
                    continue;
                }

                if (settings.IgnoredFunctions.Contains(cppFunction.Name))
                {
                    continue;
                }

                if (cppFunction.Parameters.Count == 0 || cppFunction.Parameters[0].Type.TypeKind == CppTypeKind.Pointer && !isCustomHandle)
                {
                    continue;
                }

                if (cppFunction.Parameters[0].Type.GetDisplayName() == typedef.GetDisplayName())
                {
                    var extensionPrefix = settings.GetExtensionNamePrefix(handleName);

                    var csFunctionName = settings.GetPrettyFunctionName(cppFunction.Name);
                    var csName = settings.GetPrettyExtensionName(csFunctionName, extensionPrefix);
                    string returnCsName = settings.GetCsTypeName(cppFunction.ReturnType, false);
                    CppPrimitiveKind returnKind = cppFunction.ReturnType.GetPrimitiveKind();

                    CsFunction? function = null;
                    for (int j = 0; j < commands.Count; j++)
                    {
                        if (commands[j].Name == csName)
                        {
                            function = commands[j];
                            break;
                        }
                    }

                    if (function == null)
                    {
                        cppFunction.Comment.WriteCsSummary(out string? comment);
                        function = new(csName, comment);
                        commands.Add(function);
                    }

                    CsFunctionOverload overload = new(cppFunction.Name, csName, function.Comment, "", false, false, false, new(returnCsName, returnKind));
                    overload.Attributes.Add($"[NativeName(NativeNameType.Func, \"{cppFunction.Name}\")]");
                    overload.Attributes.Add($"[return: NativeName(NativeNameType.Type, \"{cppFunction.ReturnType.GetDisplayName()}\")]");
                    for (int j = 0; j < cppFunction.Parameters.Count; j++)
                    {
                        var cppParameter = cppFunction.Parameters[j];
                        var paramCsTypeName = settings.GetCsTypeName(cppParameter.Type, false);
                        var paramCsName = settings.GetParameterName(cppParameter.Type, cppParameter.Name);
                        var direction = cppParameter.Type.GetDirection();
                        var kind = cppParameter.Type.GetPrimitiveKind();

                        CsType csType = new(paramCsTypeName, kind);

                        CsParameterInfo csParameter = new(paramCsName, csType, direction);
                        csParameter.Attributes.Add($"[NativeName(NativeNameType.Param, \"{cppParameter.Name}\")]");
                        csParameter.Attributes.Add($"[NativeName(NativeNameType.Type, \"{cppParameter.Type.GetDisplayName()}\")]");
                        overload.Parameters.Add(csParameter);
                        if (settings.TryGetDefaultValue(cppFunction.Name, cppParameter, false, out var defaultValue))
                        {
                            overload.DefaultValues.Add(paramCsName, defaultValue);
                        }
                    }

                    function.Overloads.Add(overload);
                    funcGen.GenerateVariations(cppFunction.Parameters, overload, false);
                    WriteExtensions(writer, DefinedVariationsFunctions, csFunctionName, overload, "public static");
                }
            }
        }

        private void WriteExtensions(CodeWriter writer, HashSet<string> definedExtensions, string originalFunction, CsFunctionOverload overload, params string[] modifiers)
        {
            for (int j = 0; j < overload.Variations.Count; j++)
            {
                WriteExtension(writer, definedExtensions, originalFunction, overload, overload.Variations[j], modifiers);
            }
        }

        private void WriteExtension(CodeWriter writer, HashSet<string> definedExtensions, string originalFunction, CsFunctionOverload overload, CsFunctionVariation variation, string[] modifiers)
        {
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

            var first = variation.Parameters[0];
            signature = string.Join(", ", variation.Parameters.Skip(1).Select(x => $"{string.Join(" ", x.Attributes)} {x.Type} {x.Name}").Reverse().Append($"this {first.Type} {first.Name}").Reverse());

            string header = $"{csReturnType.Name} {variation.Name}({signature})";

            if (definedExtensions.Contains(header))
            {
                LogWarn($"{writer.FileName}: {header} extension is already defined!");
                return;
            }
            definedExtensions.Add(header);

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
                    {
                        continue;
                    }

                    var paramCsDefault = overload.DefaultValues[cppParameter.Name];
                    if (cppParameter.Type.IsString || paramCsDefault.StartsWith("\"") && paramCsDefault.EndsWith("\""))
                    {
                        hasManaged = true;
                    }
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
                {
                    sb.Append($"{originalFunction}(");
                }
                else if (firstParamReturn)
                {
                    sb.Append($"{originalFunction}Native(&ret" + (overload.Parameters.Count > 1 ? ", " : ""));
                }
                else
                {
                    sb.Append($"{originalFunction}Native(");
                }

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

        private void WriteExtensionsForCOMObject(CodeWriter writer, HashSet<string> definedExtensions, CppClass cppClass, string className, ref int vTableIndex)
        {
            for (int i = 0; i < cppClass.BaseTypes.Count; i++)
            {
                var baseType = cppClass.BaseTypes[i];
                if (baseType.Type is CppClass baseClass)
                {
                    if (HasGUID(baseClass.Name))
                    {
                        WriteExtensionsForCOMObject(writer, definedExtensions, baseClass, className, ref vTableIndex);
                    }
                }
            }

            List<CsFunction> commands = new();
            for (int i = 0; i < cppClass.Functions.Count; i++, vTableIndex++)
            {
                var cppFunction = cppClass.Functions[i];
                if (settings.AllowedFunctions.Count != 0 && !settings.AllowedFunctions.Contains(cppFunction.Name))
                {
                    continue;
                }

                if (settings.IgnoredFunctions.Contains(cppFunction.Name))
                {
                    continue;
                }

                var extensionPrefix = settings.GetExtensionNamePrefix(cppClass.Name);

                var csFunctionName = settings.GetPrettyFunctionName(cppFunction.Name);
                var csName = settings.GetPrettyExtensionName(csFunctionName, extensionPrefix);
                string returnCsName = settings.GetCsTypeName(cppFunction.ReturnType, false);
                CppPrimitiveKind returnKind = cppFunction.ReturnType.GetPrimitiveKind();

                CsFunction? function = null;
                for (int j = 0; j < commands.Count; j++)
                {
                    if (commands[j].Name == csName)
                    {
                        function = commands[j];
                        break;
                    }
                }

                if (function == null)
                {
                    cppFunction.Comment.WriteCsSummary(out string? comment);
                    function = new(csName, comment);
                    commands.Add(function);
                }

                CsFunctionOverload overload = new(cppFunction.Name, csName, function.Comment, "", false, false, false, new(returnCsName, returnKind));
                overload.Attributes.Add($"[NativeName(NativeNameType.Func, \"{cppFunction.Name}\")]");
                overload.Attributes.Add($"[return: NativeName(NativeNameType.Type, \"{cppFunction.ReturnType.GetDisplayName()}\")]");
                for (int j = 0; j < cppFunction.Parameters.Count; j++)
                {
                    var cppParameter = cppFunction.Parameters[j];
                    var paramCsTypeName = settings.GetCsTypeName(cppParameter.Type, false);
                    var paramCsName = settings.GetParameterName(cppParameter.Type, cppParameter.Name);
                    var direction = cppParameter.Type.GetDirection();
                    var kind = cppParameter.Type.GetPrimitiveKind();

                    CsType csType = new(paramCsTypeName, kind);

                    CsParameterInfo csParameter = new(paramCsName, csType, direction);
                    csParameter.Attributes.Add($"[NativeName(NativeNameType.Param, \"{cppParameter.Name}\")]");
                    csParameter.Attributes.Add($"[NativeName(NativeNameType.Type, \"{cppParameter.Type.GetDisplayName()}\")]");
                    overload.Parameters.Add(csParameter);
                    if (settings.TryGetDefaultValue(cppFunction.Name, cppParameter, false, out var defaultValue))
                    {
                        overload.DefaultValues.Add(paramCsName, defaultValue);
                    }
                }

                function.Overloads.Add(overload);
                funcGen.GenerateCOMVariations(cppFunction.Parameters, overload, false);
                WriteCOMExtensions(writer, DefinedVariationsFunctions, overload, className, vTableIndex, "public static");
            }
        }

        public void WriteCOMExtensions(CodeWriter writer, HashSet<string> definedExtensions, CsFunctionOverload overload, string className, int index, params string[] modifiers)
        {
            for (int j = 0; j < overload.Variations.Count; j++)
            {
                WriteCOMExtension(writer, definedExtensions, overload, overload.Variations[j], className, index, modifiers);
            }
        }

        private void WriteCOMExtension(CodeWriter writer, HashSet<string> definedExtensions, CsFunctionOverload overload, CsFunctionVariation variation, string className, int index, string[] modifiers)
        {
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

            if (definedExtensions.Contains(header))
            {
                LogWarn($"{writer.FileName}: {header} extension is already defined!");
                return;
            }
            definedExtensions.Add(header);

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