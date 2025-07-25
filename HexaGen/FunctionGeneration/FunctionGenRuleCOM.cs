﻿namespace HexaGen.FunctionGeneration
{
    using HexaGen;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Types;
    using System.Collections.Generic;

    public class FunctionGenRuleCOM : FunctionGenRule
    {
        public override CsParameterInfo CreateParameter(CppParameter cppParameter, ParameterMapping? mapping, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings, IList<CppParameter> cppParameters, CsParameterInfo[] csParameterList, int paramIndex, CsFunctionVariation variation)
        {
            int pointerDepth = 0;
            var name = settings.GetCsTypeName(cppParameter.Type, false).Replace("*", string.Empty);
            if (cppParameter.Type.IsPointer(ref pointerDepth, out var pointerType) && (cppParameter.Type.IsClass(out var cppClass) && cppClass.IsCOMObject() || pointerType.IsVoid()))
            {
                if (pointerDepth != 0 && name == "void")
                {
                    name = "T";
                    if (variation.GenericParameters.Count > 0)
                    {
                        name += $"{variation.GenericParameters.Count}";
                    }
                    variation.GenericParameters.Add(new(name, $"where {name} : unmanaged, IComObject, IComObject<{name}>"));

                    if (paramIndex > 0 && csParameterList[paramIndex - 1]?.Type?.Name == "Guid*")
                    {
                        /*if (!function.DefaultValues.ContainsKey(comPtrParameterList[j - 1].Name))
                        {
                            function.DefaultValues.Add(comPtrParameterList[j - 1].Name, "ComUtils.GuidPtrOf<T>()");
                        }*/

                        csParameterList[paramIndex - 1].DefaultValue = $"ComUtils.GuidPtrOf<{name}>()";
                    }
                }
                if (pointerDepth == 1)
                {
                    return new(csParamName, cppParameter.Type, new($"ComPtr<{name}>", kind), direction);
                }
                else if (pointerDepth == 2)
                {
                    if (paramIndex == cppParameters.Count - 1)
                    {
                        return new(csParamName, cppParameter.Type, new($"out ComPtr<{name}>", kind), direction);
                    }
                    else
                    {
                        return new(csParamName, cppParameter.Type, new($"ref ComPtr<{name}>", kind), direction);
                    }
                }
                else
                {
                    return new(csParamName, cppParameter.Type, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                }
            }
            else
            {
                return CreateDefaultWrapperParameter(cppParameter, mapping, csParamName, kind, direction, settings);
            }
            /*
            if (name == "Guid"
                && cppParameter.Name == "riid"
                && cppParameter.Type is CppReferenceType refer
                && refer.ElementType is CppQualifiedType qual
                && qual.ElementType is CppTypedef def
                && def.Name == "IID")
            {
                if (j + 1 < parameters.Count)
                {
                    var next = parameters[j + 1];
                    if (next.Type.IsPointer(ref pointerDepth, out var pointerType) && pointerType.IsVoid() && pointerDepth != 0)
                    {
                        name = "T";
                        comVariation.GenericParameters.Add(new("T", "where T : unmanaged, IComObject, IComObject<T>"));
                        if (!function.DefaultValues.ContainsKey(paramCsName))
                        {
                            function.DefaultValues.Add(paramCsName, "ComUtils.GuidPtrOf<T>()");
                        }
                        if (pointerDepth == 1)
                        {
                            comPtrParameterList[j + 1] = new(paramCsName, new($"ComPtr<{name}>", kind), direction);
                            comSkipNext = true;
                        }
                        else if (pointerDepth == 2)
                        {
                            if (j == parameters.Count - 1)
                            {
                                comPtrParameterList[j + 1] = new(paramCsName, new($"out ComPtr<{name}>", kind), direction);
                            }
                            else
                            {
                                comPtrParameterList[j + 1] = new(paramCsName, new($"ref ComPtr<{name}>", kind), direction);
                            }
                            comSkipNext = true;
                        }
                    }
                }
            }
            */
        }
    }
}