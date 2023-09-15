namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FunctionGenerator
    {
        private CsCodeGeneratorSettings settings;

        public FunctionGenerator(CsCodeGeneratorSettings settings)
        {
            this.settings = settings;
        }

        public void GenerateVariations(IList<CppParameter> parameters, CsFunctionOverload function, bool isMember)
        {
            settings.TryGetFunctionMapping(function.ExportedName, out var mapping);

            if (mapping != null)
            {
                function.Comment = mapping.Comment;
            }

            for (long ix = 0; ix < Math.Pow(2, parameters.Count); ix++)
            {
                {
                    CsParameterInfo[] refParameterList = new CsParameterInfo[parameters.Count];
                    CsParameterInfo[] stringParameterList = new CsParameterInfo[parameters.Count];
                    CsParameterInfo[][] customParameterList = new CsParameterInfo[mapping?.CustomVariations.Count ?? 0][];
                    for (int i = 0; i < (mapping?.CustomVariations.Count ?? 0); i++)
                    {
                        customParameterList[i] = new CsParameterInfo[parameters.Count];
                    }
                    for (int j = 0; j < parameters.Count; j++)
                    {
                        var bit = (ix & (1 << j - 64)) != 0;
                        CppParameter cppParameter = parameters[j];
                        CppPrimitiveKind kind = cppParameter.Type.GetPrimitiveKind();

                        var paramCsName = settings.GetParameterName(j, cppParameter.Name);
                        var direction = cppParameter.Type.GetDirection();

                        if (bit)
                        {
                            if (cppParameter.Type is CppArrayType arrayType)
                            {
                                if (arrayType.Size > 0)
                                {
                                    refParameterList[j] = new(paramCsName, new("ref " + settings.GetCsTypeName(arrayType.ElementType, false), kind), direction);
                                }
                                else
                                {
                                    refParameterList[j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }

                                if (arrayType.ElementType.IsString())
                                {
                                    stringParameterList[j] = new(paramCsName, new("string[]", kind), direction);
                                }
                                else
                                {
                                    stringParameterList[j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }
                            }
                            else
                            {
                                refParameterList[j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);

                                if (cppParameter.Type.IsString())
                                {
                                    stringParameterList[j] = new(paramCsName, new(direction == Direction.InOut ? "ref string" : "string", kind), direction);
                                }
                                else
                                {
                                    stringParameterList[j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }
                            }

                            if (mapping != null)
                            {
                                for (int i = 0; i < mapping.CustomVariations.Count; i++)
                                {
                                    if (mapping.CustomVariations[i].TryGetValue(paramCsName, out var paramType))
                                    {
                                        customParameterList[i][j] = new(paramCsName, new(paramType, kind), direction);
                                    }
                                    else
                                    {
                                        customParameterList[i][j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                    }
                                }
                            }
                        }
                        else
                        {
                            refParameterList[j] = new(paramCsName, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
                            stringParameterList[j] = new(paramCsName, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
                            if (mapping != null)
                            {
                                for (int i = 0; i < mapping.CustomVariations.Count; i++)
                                {
                                    customParameterList[i][j] = new(paramCsName, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
                                }
                            }
                        }

                        string paramAttr = $"[NativeName(NativeNameType.Param, \"{cppParameter.Name}\")]";
                        string typeAttr = $"[NativeName(NativeNameType.Type, \"{cppParameter.Type.GetDisplayName()}\")]";
                        refParameterList[j].Attributes.Add(paramAttr);
                        refParameterList[j].Attributes.Add(typeAttr);
                        stringParameterList[j].Attributes.Add(paramAttr);
                        stringParameterList[j].Attributes.Add(typeAttr);

                        if (mapping != null)
                        {
                            for (int i = 0; i < mapping.CustomVariations.Count; i++)
                            {
                                customParameterList[i][j].Attributes.Add(paramAttr);
                                customParameterList[i][j].Attributes.Add(typeAttr);
                            }
                        }
                    }

                    CsFunctionVariation refVariation = function.CreateVariationWith();
                    refVariation.Parameters.AddRange(refParameterList);
                    CsFunctionVariation stringVariation = function.CreateVariationWith();
                    stringVariation.Parameters.AddRange(stringParameterList);

                    if (!function.HasVariation(refVariation))
                    {
                        function.Variations.Add(refVariation);
                        GenerateDefaultValueVariations(parameters, function, refVariation, isMember);
                        GenerateReturnVariations(function, refVariation, isMember);
                    }
                    if (!function.HasVariation(stringVariation))
                    {
                        function.Variations.Add(stringVariation);
                        GenerateDefaultValueVariations(parameters, function, stringVariation, isMember);
                        GenerateReturnVariations(function, stringVariation, isMember);
                    }
                    for (int i = 0; i < (mapping?.CustomVariations.Count ?? 0); i++)
                    {
                        CsFunctionVariation customVariation = function.CreateVariationWith();
                        customVariation.Parameters.AddRange(customParameterList[i]);
                        if (!function.HasVariation(customVariation))
                        {
                            function.Variations.Add(customVariation);
                            GenerateDefaultValueVariations(parameters, function, customVariation, isMember);
                            GenerateReturnVariations(function, customVariation, isMember);
                        }
                    }
                }
            }
        }

        public unsafe void GenerateCOMVariations(IList<CppParameter> parameters, CsFunctionOverload function, bool isMember)
        {
            settings.TryGetFunctionMapping(function.ExportedName, out var mapping);

            if (mapping != null)
            {
                function.Comment = mapping.Comment;
            }

            for (long ix = 0; ix < Math.Pow(2, parameters.Count); ix++)
            {
                {
                    CsFunctionVariation refVariation = function.CreateVariationWith();
                    CsFunctionVariation stringVariation = function.CreateVariationWith();
                    CsFunctionVariation comVariation = function.CreateVariationWith();

                    CsParameterInfo[] refParameterList = new CsParameterInfo[parameters.Count];
                    CsParameterInfo[] stringParameterList = new CsParameterInfo[parameters.Count];
                    CsParameterInfo[] comPtrParameterList = new CsParameterInfo[parameters.Count];
                    CsParameterInfo[][] customParameterList = new CsParameterInfo[mapping?.CustomVariations.Count ?? 0][];
                    for (int i = 0; i < (mapping?.CustomVariations.Count ?? 0); i++)
                    {
                        customParameterList[i] = new CsParameterInfo[parameters.Count];
                    }
                    for (int j = 0; j < parameters.Count; j++)
                    {
                        var bit = (ix & (1 << j - 64)) != 0;
                        CppParameter cppParameter = parameters[j];
                        CppPrimitiveKind kind = cppParameter.Type.GetPrimitiveKind();

                        var paramCsName = settings.GetParameterName(j, cppParameter.Name);
                        var direction = cppParameter.Type.GetDirection();

                        if (bit)
                        {
                            if (cppParameter.Type is CppArrayType arrayType)
                            {
                                if (arrayType.Size > 0)
                                {
                                    refParameterList[j] = new(paramCsName, new("ref " + settings.GetCsTypeName(arrayType.ElementType, false), kind), direction);
                                }
                                else
                                {
                                    refParameterList[j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }

                                if (arrayType.Size > 0)
                                {
                                    comPtrParameterList[j] = new(paramCsName, new("ref " + settings.GetCsTypeName(arrayType.ElementType, false), kind), direction);
                                }
                                else
                                {
                                    comPtrParameterList[j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }

                                if (arrayType.ElementType.IsString())
                                {
                                    stringParameterList[j] = new(paramCsName, new("string[]", kind), direction);
                                }
                                else
                                {
                                    stringParameterList[j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }
                            }
                            else
                            {
                                refParameterList[j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);

                                int pointerDepth = 0;
                                var name = settings.GetCsTypeName(cppParameter.Type, false).Replace("*", string.Empty);
                                if (cppParameter.Type.IsPointer(ref pointerDepth) && (cppParameter.Type.IsClass(out var cppClass) && cppClass.IsCOMObject() || cppParameter.Name == "ppvInterface" || cppParameter.Name == "ppvObject"))
                                {
                                    if (pointerDepth != 0 && name == "void")
                                    {
                                        name = "T";
                                        comVariation.GenericParameters.Add(new("T", "where T : unmanaged, IComObject, IComObject<T>"));

                                        if (j > 0 && comPtrParameterList[j - 1].Type.Name == "Guid*")
                                        {
                                            if (!function.DefaultValues.ContainsKey(comPtrParameterList[j - 1].Name))
                                            {
                                                function.DefaultValues.Add(comPtrParameterList[j - 1].Name, "ComUtils.GuidPtrOf<T>()");
                                            }

                                            comPtrParameterList[j - 1] = null;
                                        }
                                    }
                                    if (pointerDepth == 1)
                                    {
                                        comPtrParameterList[j] = new(paramCsName, new($"ComPtr<{name}>", kind), direction);
                                    }
                                    else if (pointerDepth == 2)
                                    {
                                        if (j == parameters.Count - 1)
                                        {
                                            comPtrParameterList[j] = new(paramCsName, new($"out ComPtr<{name}>", kind), direction);
                                        }
                                        else
                                        {
                                            comPtrParameterList[j] = new(paramCsName, new($"ref ComPtr<{name}>", kind), direction);
                                        }
                                    }
                                    else
                                    {
                                        comPtrParameterList[j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                    }
                                }
                                else
                                {
                                    comPtrParameterList[j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }

                                if (name == "Guid"
                                    && cppParameter.Name == "riid"
                                    && cppParameter.Type is CppReferenceType refer
                                    && refer.ElementType is CppQualifiedType qual
                                    && qual.ElementType is CppTypedef def
                                    && def.Name == "IID")
                                {
                                }

                                if (cppParameter.Type.IsString())
                                {
                                    stringParameterList[j] = new(paramCsName, new(direction == Direction.InOut ? "ref string" : "string", kind), direction);
                                }
                                else
                                {
                                    stringParameterList[j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }
                            }

                            if (mapping != null)
                            {
                                for (int i = 0; i < mapping.CustomVariations.Count; i++)
                                {
                                    if (mapping.CustomVariations[i].TryGetValue(paramCsName, out var paramType))
                                    {
                                        customParameterList[i][j] = new(paramCsName, new(paramType, kind), direction);
                                    }
                                    else
                                    {
                                        customParameterList[i][j] = new(paramCsName, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                    }
                                }
                            }
                        }
                        else
                        {
                            refParameterList[j] = new(paramCsName, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
                            stringParameterList[j] = new(paramCsName, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
                            comPtrParameterList[j] = new(paramCsName, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
                            if (mapping != null)
                            {
                                for (int i = 0; i < mapping.CustomVariations.Count; i++)
                                {
                                    customParameterList[i][j] = new(paramCsName, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
                                }
                            }
                        }

                        string paramAttr = $"[NativeName(NativeNameType.Param, \"{cppParameter.Name}\")]";
                        string typeAttr = $"[NativeName(NativeNameType.Type, \"{cppParameter.Type.GetDisplayName()}\")]";
                        refParameterList[j].Attributes.Add(paramAttr);
                        refParameterList[j].Attributes.Add(typeAttr);
                        stringParameterList[j].Attributes.Add(paramAttr);
                        stringParameterList[j].Attributes.Add(typeAttr);
                        comPtrParameterList[j].Attributes.Add(paramAttr);
                        comPtrParameterList[j].Attributes.Add(typeAttr);

                        if (mapping != null)
                        {
                            for (int i = 0; i < mapping.CustomVariations.Count; i++)
                            {
                                customParameterList[i][j].Attributes.Add(paramAttr);
                                customParameterList[i][j].Attributes.Add(typeAttr);
                            }
                        }
                    }

                    refVariation.Parameters.AddRange(refParameterList);

                    stringVariation.Parameters.AddRange(stringParameterList);

                    comVariation.Parameters.AddRange(comPtrParameterList.Where(x => x != null));

                    if (!function.HasVariation(refVariation))
                    {
                        function.Variations.Add(refVariation);
                        GenerateDefaultValueVariations(parameters, function, refVariation, isMember);
                        GenerateReturnVariations(function, refVariation, isMember);
                    }
                    if (!function.HasVariation(stringVariation))
                    {
                        function.Variations.Add(stringVariation);
                        GenerateDefaultValueVariations(parameters, function, stringVariation, isMember);
                        GenerateReturnVariations(function, stringVariation, isMember);
                    }
                    if (!function.HasVariation(comVariation))
                    {
                        function.Variations.Add(comVariation);
                        GenerateDefaultValueVariations(parameters, function, comVariation, isMember);
                        GenerateReturnVariations(function, comVariation, isMember);
                    }
                    for (int i = 0; i < (mapping?.CustomVariations.Count ?? 0); i++)
                    {
                        CsFunctionVariation customVariation = function.CreateVariationWith();
                        customVariation.Parameters.AddRange(customParameterList[i]);
                        if (!function.HasVariation(customVariation))
                        {
                            function.Variations.Add(customVariation);
                            GenerateDefaultValueVariations(parameters, function, customVariation, isMember);
                            GenerateReturnVariations(function, customVariation, isMember);
                        }
                    }
                }
            }
        }

        public static unsafe void GenerateDefaultValueVariations(IList<CppParameter> parameters, CsFunctionOverload function, CsFunctionVariation variation, bool isMember)
        {
            if (function.DefaultValues.Count == 0)
                return;

            for (int i = variation.Parameters.Count - 1; i >= 0; i--)
            {
                var param = variation.Parameters[i];

                if (!function.DefaultValues.TryGetValue(param.Name, out var defaultValue))
                {
                    continue;
                }

                CsFunctionVariation defaultVariation = new(variation.ExportedName, variation.Name, variation.StructName, variation.IsMember, variation.IsConstructor, variation.IsDestructor, variation.ReturnType);
                defaultVariation.GenericParameters.AddRange(variation.GenericParameters);
                for (int j = 0; j < variation.Parameters.Count; j++)
                {
                    var variationParameter = variation.Parameters[j];
                    if (param != variationParameter)
                    {
                        defaultVariation.Parameters.Add(variationParameter);
                    }
                    else
                    {
                        var cloned = variationParameter.Clone();
                        cloned.DefaultValue = defaultValue;
                        defaultVariation.Parameters.Add(cloned);
                    }
                }

                if (function.HasVariation(defaultVariation))
                    continue;

                function.Variations.Add(defaultVariation);

                GenerateDefaultValueVariations(parameters, function, defaultVariation, isMember);
                GenerateReturnVariations(function, defaultVariation, isMember);
            }
        }

        public static void GenerateReturnVariations(CsFunctionOverload function, CsFunctionVariation variation, bool isMember)
        {
            if (!isMember && variation.ReturnType.IsVoid && !variation.ReturnType.IsPointer && variation.Parameters.Count > 0)
            {
                if (variation.Parameters[0].Name == "output" && variation.Parameters[0].Type.IsPointer)
                {
                    CsFunctionVariation returnVariation = new(variation.ExportedName, variation.Name, variation.StructName, variation.IsMember, variation.IsConstructor, variation.IsDestructor, variation.ReturnType);
                    returnVariation.GenericParameters.AddRange(variation.GenericParameters);
                    returnVariation.Parameters = variation.Parameters.Skip(1).ToList();
                    if (!function.HasVariation(returnVariation))
                    {
                        function.Variations.Add(returnVariation);
                        function.Variations.Remove(variation);
                        returnVariation.ReturnType = new(variation.Parameters[0].Type.Name[..^1], variation.Parameters[0].Type.PrimitiveType);
                    }
                }
            }
            if (variation.ReturnType.IsPointer && variation.ReturnType.PrimitiveType == CsPrimitiveType.Byte || variation.ReturnType.PrimitiveType == CsPrimitiveType.Char)
            {
                CsFunctionVariation returnVariation = new(variation.ExportedName, variation.Name + "S", variation.StructName, variation.IsMember, variation.IsConstructor, variation.IsDestructor, variation.ReturnType);
                returnVariation.GenericParameters.AddRange(variation.GenericParameters);
                returnVariation.Parameters = variation.Parameters.ToList();
                function.Variations.Add(returnVariation);
                returnVariation.ReturnType = new("string", variation.ReturnType.PrimitiveType);
            }
        }
    }
}