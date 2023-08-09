namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public partial class CsComCodeGenerator
    {
        protected override List<string> SetupTypeUsings()
        {
            var usings = base.SetupTypeUsings();
            usings.Add("HexaGen.Runtime.COM");
            return usings;
        }

        protected override void GenerateTypes(CppCompilation compilation, string outputPath)
        {
            // Print All classes, structs
            string filePath = Path.Combine(outputPath, "Structures.cs");

            // Generate Structures
            using var writer = new CodeWriter(filePath, settings.Namespace, SetupTypeUsings());

            GenContext context = new(compilation, filePath, writer);

            // Print All classes, structs
            for (int i = 0; i < compilation.Classes.Count; i++)
            {
                CppClass? cppClass = compilation.Classes[i];
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

                if (DefinedTypes.Contains(cppClass.Name))
                {
                    LogWarn($"{filePath}: {cppClass} is already defined!");
                    continue;
                }

                DefinedTypes.Add(cppClass.Name);

                string csName = settings.GetCsCleanName(cppClass.Name);

                var mapping = settings.GetTypeMapping(cppClass.Name);

                csName = mapping?.FriendlyName ?? csName;

                if (TryGetGUID(cppClass.Name, out var guid))
                {
                    WriteCOMObject(writer, compilation, cppClass, mapping, csName, guid);
                }
                else
                {
                    if (cppClass.Fields.Count == 0 && cppClass.Functions.Count > 0 && cppClass.IsAbstract)
                    {
                        WriteCOMObject(writer, compilation, cppClass, mapping, csName, null);
                        continue;
                    }

                    WriteClass(context, cppClass, mapping, csName);
                }
            }
        }

        private void WriteCOMObject(CodeWriter writer, CppCompilation compilation, CppClass cppClass, TypeMapping? mapping, string csName, Guid? guid)
        {
            if (cppClass.ClassKind == CppClassKind.Class || cppClass.Name.EndsWith("_T") || csName == "void")
            {
                return;
            }

            int vTableIndex = 0;
            bool isReadOnly = false;
            string modifier = "partial";

            LogInfo("defined struct " + csName);
            var commentWritten = cppClass.Comment.WriteCsSummary(writer);
            if (!commentWritten)
            {
                commentWritten = mapping?.Comment.WriteCsSummary(writer) ?? false;
            }
            if (guid != null)
            {
                writer.WriteLine($"[Guid(\"{guid}\")]");
            }
            writer.WriteLine($"[NativeName(NativeNameType.StructOrClass, \"{cppClass.Name}\")]");

            StringBuilder sb = new($"IComObject, IComObject<{csName}>");
            {
                Queue<CppBaseType> baseTypes = new();
                for (int i = 0; i < cppClass.BaseTypes.Count; i++)
                {
                    baseTypes.Enqueue(cppClass.BaseTypes[i]);
                }
                while (baseTypes.Count > 0)
                {
                    var current = baseTypes.Dequeue();
                    if (current.Type is CppClass baseClass)
                    {
                        string csNameBaseClass = settings.GetCsCleanName(baseClass.Name);
                        sb.Append($", IComObject<{csNameBaseClass}>");

                        for (int i = 0; i < baseClass.BaseTypes.Count; i++)
                        {
                            baseTypes.Enqueue(baseClass.BaseTypes[i]);
                        }
                    }
                }
            }

            using (writer.PushBlock($"public {modifier} struct {csName} : {sb}"))
            {
                writer.WriteLine("public unsafe void** LpVtbl;");
                writer.WriteLine();

                if (guid != null)
                {
                    writer.WriteLine($"public static readonly Guid Guid = new(\"{guid}\");");
                    writer.WriteLine();
                }

                WriteCOMConstructor(writer, csName);

                for (int i = 0; i < cppClass.BaseTypes.Count; i++)
                {
                    var baseType = cppClass.BaseTypes[i];
                    if (baseType.Type is CppClass baseClass)
                    {
                        if (HasGUID(baseClass.Name))
                        {
                            WriteFunctionsForCOMObject(writer, baseClass, csName, ref vTableIndex);
                        }
                    }
                }

                WriteFunctionsForCOMObject(writer, cppClass, csName, ref vTableIndex);

                using (writer.PushBlock($"unsafe void*** IComObject.AsVtblPtr()"))
                {
                    writer.WriteLine($"return (void***)Unsafe.AsPointer(ref Unsafe.AsRef(in this));");
                }
                writer.WriteLine();

                for (int i = 0; i < cppClass.BaseTypes.Count; i++)
                {
                    var baseType = cppClass.BaseTypes[i];
                    if (baseType.Type is CppClass baseClass)
                    {
                        WriteCOMBaseTypeCast(writer, csName, baseClass);
                    }
                }
            }

            writer.WriteLine();
        }

        private void WriteFunctionsForCOMObject(CodeWriter writer, CppClass cppClass, string csName, ref int vTableIndex)
        {
            List<CsFunction> commands = new();
            for (int i = 0; i < cppClass.Functions.Count; i++, vTableIndex++)
            {
                var cppFunction = cppClass.Functions[i];

                string? csFunctionName = settings.GetPrettyFunctionName(cppFunction.Name);
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

                CsFunctionOverload overload = new(cppFunction.Name, csFunctionName, function.Comment, "", false, false, false, new(returnCsName, returnKind));
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
                WriteCOMFunctions(writer, DefinedVariationsFunctions, function, overload, csName, vTableIndex, "public readonly unsafe");
            }
        }

        private void WriteCOMConstructor(CodeWriter writer, string csName)
        {
            using (writer.PushBlock($"public unsafe {csName} (void** lpVtbl = null)"))
            {
                writer.WriteLine("LpVtbl = lpVtbl;");
            }
            writer.WriteLine();
        }

        private void WriteCOMBaseTypeCast(CodeWriter writer, string csName, CppClass baseClass)
        {
            string csNameBaseClass = settings.GetCsCleanName(baseClass.Name);
            using (writer.PushBlock($"public unsafe static implicit operator {csNameBaseClass} ({csName} value)"))
            {
                writer.WriteLine($"return Unsafe.As<{csName}, {csNameBaseClass}>(ref value);");
            }
            writer.WriteLine();

            for (int i = 0; i < baseClass.BaseTypes.Count; i++)
            {
                var baseType = baseClass.BaseTypes[i];
                if (baseType.Type is CppClass basebaseClass)
                {
                    WriteCOMBaseTypeCast(writer, csName, basebaseClass);
                }
            }
        }

        private void WriteCOMFunctions(CodeWriter writer, HashSet<string> definedFunctions, CsFunction csFunction, CsFunctionOverload overload, string className, int index, params string[] modifiers)
        {
            for (int j = 0; j < overload.Variations.Count; j++)
            {
                WriteCOMFunction(writer, definedFunctions, csFunction, overload, overload.Variations[j], className, index, modifiers);
            }
        }

        private void WriteCOMFunction(CodeWriter writer, HashSet<string> definedFunctions, CsFunction function, CsFunctionOverload overload, CsFunctionVariation variation, string className, int index, params string[] modifiers)
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
            string signatureNameless = $"{className}*{(overload.Parameters.Count > 0 ? ", " : string.Empty)}";

            signature = string.Join(", ", variation.Parameters.Select(x => $"{string.Join(" ", x.Attributes)} {x.Type} {x.Name}"));
            signatureNameless += string.Join(", ", overload.Parameters.Select(x => $"{(x.Type.IsBool ? settings.GetBoolType() : x.Type.Name)}"));

            string header = $"{csReturnType.Name} {variation.Name}({signature})";

            if (definedFunctions.Contains(header))
            {
                return;
            }
            definedFunctions.Add(header);

            LogInfo("defined function " + header);

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
                writer.WriteLine($"{className}* ptr = ({className}*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));");
                StringBuilder sb = new();

                int offset = 0;

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

                if (!csReturnType.IsVoid || csReturnType.IsVoid && csReturnType.IsPointer)
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
                var ptr = index == 0 ? "*LpVtbl" : $"LpVtbl[{index}]";
                var tail = variation.Parameters.Count > 0 ? ", " : string.Empty;

                sb.Append($"((delegate* unmanaged[Stdcall]<{signatureNameless}, {retType}>)({ptr}))(ptr{tail}");

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