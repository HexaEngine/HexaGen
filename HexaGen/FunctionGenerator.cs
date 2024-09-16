namespace HexaGen
{
    using CppAst;
    using HexaGen.Core.CSharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CsSubClass
    {
        public CppType CppType { get; set; }

        public CsType Type { get; set; }

        public string Name { get; set; }

        public string CppFieldName { get; set; }

        public string FieldName { get; set; }

        public CsSubClass(CppType type, string name, string cppFieldName, string fieldName)
        {
            Type = new(name, name, false, false, false, false, false, false, false, false, false, false, CsStringType.None, CsPrimitiveType.Unknown);
            CppType = type;
            Name = name;
            CppFieldName = cppFieldName;
            FieldName = fieldName;
        }
    }

    public abstract class FunctionGenRule
    {
        public abstract CsParameterInfo CreateParameter(CppParameter cppParameter, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings);

        public virtual CsParameterInfo CreateDefaultParameter(CppParameter cppParameter, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings)
        {
            return new(csParamName, cppParameter.Type, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
        }

        public virtual CsParameterInfo CreateDefaultWrapperParameter(CppParameter cppParameter, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings)
        {
            return new(csParamName, cppParameter.Type, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
        }
    }

    public abstract class FunctionGenRuleMatch<T> : FunctionGenRule where T : ICppElement
    {
        public abstract bool IsMatch(CppParameter cppParameter, T type);

        public override CsParameterInfo CreateParameter(CppParameter cppParameter, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings)
        {
            if (cppParameter.Type is T t && IsMatch(cppParameter, t))
            {
                return CreateParameter(cppParameter, t, csParamName, kind, direction, settings);
            }

            return CreateDefaultWrapperParameter(cppParameter, csParamName, kind, direction, settings);
        }

        public abstract CsParameterInfo CreateParameter(CppParameter cppParameter, T type, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings);
    }

    public abstract class FunctionGenRuleMatch : FunctionGenRule
    {
        public abstract bool IsMatch(CppParameter cppParameter, CppType type);

        public override CsParameterInfo CreateParameter(CppParameter cppParameter, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings)
        {
            if (IsMatch(cppParameter, cppParameter.Type))
            {
                return CreateParameter(cppParameter, cppParameter.Type, csParamName, kind, direction, settings);
            }

            return CreateDefaultWrapperParameter(cppParameter, csParamName, kind, direction, settings);
        }

        public abstract CsParameterInfo CreateParameter(CppParameter cppParameter, CppType type, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings);
    }

    public class FunctionGenRuleRef : FunctionGenRuleMatch<CppArrayType>
    {
        public override CsParameterInfo CreateParameter(CppParameter cppParameter, CppArrayType type, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings)
        {
            return new(csParamName, cppParameter.Type, new("ref " + settings.GetCsTypeName(type.ElementType, false), kind), direction);
        }

        public override bool IsMatch(CppParameter cppParameter, CppArrayType type)
        {
            return type.Size > 0;
        }
    }

    public class FunctionGenRuleSpan : FunctionGenRule
    {
        public override CsParameterInfo CreateParameter(CppParameter cppParameter, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings)
        {
            if (cppParameter.Type is CppArrayType arrayType)
            {
                if (arrayType.Size > 0)
                {
                    return new(csParamName, cppParameter.Type, new($"ReadOnlySpan<{settings.GetCsTypeName(arrayType.ElementType, false)}>", kind), direction);
                }
            }
            else if (cppParameter.Type.IsString())
            {
                switch (kind)
                {
                    case CppPrimitiveKind.Char:
                        if (direction == Direction.InOut || direction == Direction.Out) break;
                        return new(csParamName, cppParameter.Type, new("ReadOnlySpan<byte>", kind), direction);

                    case CppPrimitiveKind.WChar:
                        if (direction == Direction.InOut || direction == Direction.Out) break;
                        return new(csParamName, cppParameter.Type, new("ReadOnlySpan<char>", kind), direction);
                }
            }

            return CreateDefaultWrapperParameter(cppParameter, csParamName, kind, direction, settings);
        }
    }

    public class FunctionGenRuleString : FunctionGenRule
    {
        public override CsParameterInfo CreateParameter(CppParameter cppParameter, string csParamName, CppPrimitiveKind kind, Direction direction, CsCodeGeneratorConfig settings)
        {
            if (cppParameter.Type is CppArrayType arrayType && arrayType.ElementType.IsString())
            {
                return new(csParamName, cppParameter.Type, new("string[]", kind), direction);
            }

            if (cppParameter.Type.IsString())
            {
                return new(csParamName, cppParameter.Type, new(direction == Direction.InOut ? "ref string" : "string", kind), direction);
            }

            return CreateDefaultWrapperParameter(cppParameter, csParamName, kind, direction, settings);
        }
    }

    /// <summary>
    /// Represents an abstract base class for function generation steps,
    /// providing methods to generate variations of functions and
    /// execute sub-steps.
    /// </summary>
    public abstract class FunctionGenStep
    {
        /// <summary>
        /// Gets or sets the list of function generation steps.
        /// </summary>
        public List<FunctionGenStep> GenSteps = []; // default to empty list, to avoid problems later with null checks

        /// <summary>
        /// Determines whether to skip the given function and variation.
        /// </summary>
        /// <param name="function">The function to check.</param>
        /// <param name="variation">The variation to check.</param>
        /// <returns>
        /// True if the function should be skipped; otherwise, false.
        /// </returns>
        public virtual bool ShouldSkip(CsFunctionOverload function, CsFunctionVariation variation)
        {
            return false;
        }

        /// <summary>
        /// Generates variations of the specified function and variation.
        /// Must be implemented by derived classes.
        /// </summary>
        /// <param name="function">The function to generate variations for.</param>
        /// <param name="variation">The variation to generate variations for.</param>
        public abstract void GenerateVariations(CsFunctionOverload function, CsFunctionVariation variation);

        /// <summary>
        /// Executes sub-steps of the specified type for the given function and variation.
        /// </summary>
        /// <typeparam name="T">The type of sub-step to execute.</typeparam>
        /// <param name="function">The function to process with sub-steps.</param>
        /// <param name="variation">The variation to process with sub-steps.</param>
        public void DoSubStep<T>(CsFunctionOverload function, CsFunctionVariation variation) where T : FunctionGenStep
        {
            foreach (var step in GenSteps)
            {
                if (step.ShouldSkip(function, variation)) continue;
                if (step is T t)
                {
                    t.GenerateVariations(function, variation);
                }
            }
        }

        /// <summary>
        /// Executes all sub-steps for the given function and variation.
        /// </summary>
        /// <param name="function">The function to process with sub-steps.</param>
        /// <param name="variation">The variation to process with sub-steps.</param>
        public void DoSubStep(CsFunctionOverload function, CsFunctionVariation variation)
        {
            foreach (var step in GenSteps)
            {
                if (step.ShouldSkip(function, variation)) continue;
                step.GenerateVariations(function, variation);
            }
        }
    }

    /// <summary>
    /// Represents a generator step that produces variations of functions
    /// by applying default values to parameters.
    /// </summary>
    public class DefaultValueGenStep : FunctionGenStep
    {
        /// <summary>
        /// Generates variations of the specified function and variation
        /// by applying default values to parameters.
        /// </summary>
        /// <param name="function">The function to generate variations for.</param>
        /// <param name="variation">The variation to generate variations for.</param>
        public override void GenerateVariations(CsFunctionOverload function, CsFunctionVariation variation)
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

                TransformDefaultValue(function, variation, param, ref defaultValue);

                CsFunctionVariation defaultVariation = variation.ShallowClone();
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

                if (!function.TryAddVariation(defaultVariation))
                    continue;

                if (function.Kind != CsFunctionKind.Constructor)
                {
                    DoSubStep(function, defaultVariation);
                }
                else
                {
                    DoSubStep<DefaultValueGenStep>(function, defaultVariation);
                }
            }
        }

        /// <summary>
        /// Transforms the default value of a parameter before applying it to a variation.
        /// </summary>
        /// <param name="function">The function being processed.</param>
        /// <param name="variation">The variation being processed.</param>
        /// <param name="parameter">The parameter whose default value is being transformed.</param>
        /// <param name="defaultValue">The default value to transform.</param>
        protected virtual void TransformDefaultValue(CsFunctionOverload function, CsFunctionVariation variation, CsParameterInfo parameter, ref string defaultValue)
        {
        }
    }

    /// <summary>
    /// Represents a generator step that produces variations of functions
    /// based on return type and parameter conditions.
    /// </summary>
    public class ReturnVariationGenStep : FunctionGenStep
    {
        /// <summary>
        /// Determines whether to skip the given function and variation.
        /// </summary>
        /// <param name="function">The function to check.</param>
        /// <param name="variation">The variation to check.</param>
        /// <returns>
        /// True if the function should be skipped; otherwise, false.
        /// </returns>
        public override bool ShouldSkip(CsFunctionOverload function, CsFunctionVariation variation)
        {
            return function.Kind == CsFunctionKind.Member;
        }

        /// <summary>
        /// Gets the set of allowed parameter names.
        /// </summary>
        public virtual HashSet<string> AllowedParameterNames { get; } = ["pOut"];

        /// <summary>
        /// Determines whether the specified parameter is allowed.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <returns>
        /// True if the parameter is allowed; otherwise, false.
        /// </returns>
        protected virtual bool IsParameterAllowed(CsParameterInfo parameter)
        {
            return AllowedParameterNames.Contains(parameter.Name) && parameter.Type.IsPointer;
        }

        /// <summary>
        /// Generates variations of the specified function and variation.
        /// </summary>
        /// <param name="function">The function to generate variations for.</param>
        /// <param name="variation">The variation to generate variations for.</param>
        public override void GenerateVariations(CsFunctionOverload function, CsFunctionVariation variation)
        {
            if (variation.ReturnType.IsVoid && !variation.ReturnType.IsPointer && variation.Parameters.Count > 0)
            {
                var param = variation.Parameters[0];
                if (IsParameterAllowed(param))
                {
                    CsFunctionVariation returnVariation = variation.ShallowClone();
                    CreateVariation(function, variation, param, returnVariation);
                }
            }
        }

        /// <summary>
        /// Creates a new variation of the function with updated return type
        /// based on the specified parameter.
        /// </summary>
        /// <param name="function">The original function.</param>
        /// <param name="variation">The original variation.</param>
        /// <param name="parameter">The parameter to base the new variation on.</param>
        /// <param name="returnVariation">The new function variation to be updated.</param>
        protected virtual void CreateVariation(CsFunctionOverload function, CsFunctionVariation variation, CsParameterInfo parameter, CsFunctionVariation returnVariation)
        {
            returnVariation.GenericParameters.AddRange(variation.GenericParameters);
            returnVariation.Parameters = variation.Parameters.Skip(1).ToList();
            if (function.TryUpdateVariation(variation, returnVariation))
            {
                returnVariation.ReturnType = new(parameter.Type.Name[..^1], parameter.Type.PrimitiveType);
            }
        }
    }

    /// <summary>
    /// Represents a generator step that produces variations of functions
    /// with string pointer return types.
    /// </summary>
    public class StringReturnGenStep : FunctionGenStep
    {
        /// <summary>
        /// Determines whether the specified return type is allowed.
        /// </summary>
        /// <param name="function">The function to check.</param>
        /// <param name="variation">The variation to check.</param>
        /// <param name="returnType">The return type to check.</param>
        /// <returns>
        /// True if the return type is allowed; otherwise, false.
        /// </returns>
        protected virtual bool AllowReturnType(CsFunctionOverload function, CsFunctionVariation variation, CsType returnType)
        {
            return returnType.IsPointer && (returnType.PrimitiveType == CsPrimitiveType.Byte || returnType.PrimitiveType == CsPrimitiveType.Char);
        }

        /// <summary>
        /// Generates variations of the specified function and variation
        /// with string pointer return types.
        /// </summary>
        /// <param name="function">The function to generate variations for.</param>
        /// <param name="variation">The variation to generate variations for.</param>
        public override void GenerateVariations(CsFunctionOverload function, CsFunctionVariation variation)
        {
            if (AllowReturnType(function, variation, variation.ReturnType))
            {
                CsFunctionVariation returnVariation = variation.ShallowClone();
                CreateVariation(function, variation, returnVariation);
            }
        }

        /// <summary>
        /// Creates a new variation of the function with an updated return type of string.
        /// </summary>
        /// <param name="function">The original function.</param>
        /// <param name="variation">The original variation.</param>
        /// <param name="returnVariation">The new function variation to be updated.</param>
        protected virtual void CreateVariation(CsFunctionOverload function, CsFunctionVariation variation, CsFunctionVariation returnVariation)
        {
            returnVariation.Name += "S";
            returnVariation.GenericParameters.AddRange(variation.GenericParameters);
            returnVariation.Parameters = variation.Parameters.ToList();
            if (function.TryAddVariation(returnVariation))
            {
                returnVariation.ReturnType = new("string", variation.ReturnType.PrimitiveType);
            }
        }
    }

    public class FunctionGenerator
    {
        private readonly CsCodeGeneratorConfig settings;
        private readonly List<FunctionGenRule> rules = new();
        private readonly List<FunctionGenStep> steps = new();

        public FunctionGenerator(CsCodeGeneratorConfig settings)
        {
            this.settings = settings;
            rules.Add(new FunctionGenRuleRef());
            rules.Add(new FunctionGenRuleSpan());
            rules.Add(new FunctionGenRuleString());
            steps.Add(new DefaultValueGenStep());
            steps.Add(new ReturnVariationGenStep());
            steps.Add(new StringReturnGenStep());
        }

        public IReadOnlyList<FunctionGenRule> Rules => rules;

        public IReadOnlyList<FunctionGenStep> Steps => steps;

        public void OverwriteStep<T>(FunctionGenStep step) where T : FunctionGenStep
        {
            for (int i = 0; i < steps.Count; i++)
            {
                if (steps[i] is T)
                {
                    steps[i] = step;
                    return;
                }
            }
        }

        public void OverwriteRule<T>(FunctionGenRule rule) where T : FunctionGenRule
        {
            for (int i = 0; i < rules.Count; i++)
            {
                if (rules[i] is T)
                {
                    rules[i] = rule;
                    return;
                }
            }
        }

        public void AddRule(FunctionGenRule rule)
        {
            rules.Add(rule);
        }

        public void RemoveRule(FunctionGenRule rule)
        {
            rules.Remove(rule);
        }

        public void AddStep(FunctionGenStep step)
        {
            steps.Add(step);
        }

        public void RemoveStep(FunctionGenStep step)
        {
            steps.Remove(step);
        }

        public virtual void GenerateConstructorVariations(CppClass cppClass, List<CsSubClass> subClasses, string csName, CsFunctionOverload function)
        {
            settings.TryGetFunctionMapping(function.ExportedName, out var mapping);

            if (mapping != null)
            {
                function.Comment = mapping.Comment;
            }

            var fields = cppClass.Fields;

            CsParameterInfo[] parameterList = new CsParameterInfo[fields.Count];
            CsParameterInfo[] spanParameterList = new CsParameterInfo[fields.Count];
            for (int i = 0; i < fields.Count; i++)
            {
                CppField cppField = fields[i];
                CppPrimitiveKind kind = cppField.Type.GetPrimitiveKind();

                var fieldCsName = settings.GetFieldName(cppField.Name);
                var paramCsTypeName = settings.GetCsTypeName(cppField.Type, false);
                var paramCsName = settings.GetParameterName(i, cppField.Name);
                var direction = cppField.Type.GetDirection();

                {
                    if (cppField.Type is CppArrayType arrayType && arrayType.ElementType is CppPointerType pointerType && pointerType.ElementType is CppFunctionType && settings.DelegatesAsVoidPointer)
                    {
                        paramCsTypeName = "nint*";
                    }
                }

                var subClass = subClasses.Find(x => x.CppType == cppField.Type);
                if (subClass != null && cppField.Type is CppClass cppClass1 && cppClass1.ClassKind == CppClassKind.Union)
                {
                    subClass = subClasses.First(x => x.CppType == cppClass1);
                    paramCsTypeName = subClass.Name;
                    paramCsName = subClass.FieldName.ToLower();
                    fieldCsName = subClass.FieldName;
                }

                if (subClass != null)
                {
                    paramCsTypeName = subClass.Name;
                }

                int depth = 0;
                var subClass1 = subClasses.FirstOrDefault(x => x.CppType.IsPointerOf(cppField.Type, ref depth));
                if (subClass1 != null)
                {
                    paramCsTypeName = subClass1.Name + new string('*', depth);
                }

                parameterList[i] = new(paramCsName, cppField.Type, new(paramCsTypeName, kind), direction, "default", fieldCsName);

                {
                    if (cppField.Type is CppArrayType arrayType)
                    {
                        var arrayElementTypeName = settings.GetCsWrappedPointerTypeName(arrayType.ElementType, false);
                        spanParameterList[i] = new(paramCsName, cppField.Type, new($"Span<{arrayElementTypeName}>", kind), direction, "default", fieldCsName);
                    }
                    else
                    {
                        spanParameterList[i] = new(paramCsName, cppField.Type, new(paramCsTypeName, kind), direction, "default", fieldCsName);
                    }
                }
            }

            CsFunctionVariation variation = function.CreateVariationWith();
            variation.Parameters.AddRange(parameterList);

            CsFunctionVariation spanVariation = function.CreateVariationWith();
            spanVariation.Parameters.AddRange(spanParameterList);

            function.TryAddVariation(variation);

            function.TryAddVariation(spanVariation);
        }

        public virtual void GenerateVariations(IList<CppParameter> parameters, CsFunctionOverload function)
        {
            settings.TryGetFunctionMapping(function.ExportedName, out var mapping);

            if (mapping != null && mapping.Comment != null)
            {
                function.Comment = settings.WriteCsSummary(mapping.Comment);
            }

            long maxVariations = (long)Math.Pow(2L, parameters.Count);
            for (long ix = 0; ix < maxVariations; ix++)
            {
                CsParameterInfo[][] parameterLists = new CsParameterInfo[rules.Count][];
                for (int i = 0; i < rules.Count; i++)
                {
                    parameterLists[i] = new CsParameterInfo[parameters.Count];
                }

                CsParameterInfo[][]? customParameterList = mapping != null ? new CsParameterInfo[mapping.CustomVariations.Count][] : null;

                if (customParameterList != null)
                {
                    for (int i = 0; i < customParameterList.Length; i++)
                    {
                        customParameterList[i] = new CsParameterInfo[parameters.Count];
                    }
                }

                for (int j = 0; j < parameters.Count; j++)
                {
                    var bit = (ix & (1 << (j - 64))) != 0;
                    CppParameter cppParameter = parameters[j];
                    CppPrimitiveKind kind = cppParameter.Type.GetPrimitiveKind();

                    var paramCsName = settings.GetParameterName(j, cppParameter.Name);
                    var direction = cppParameter.Type.GetDirection();

                    for (int i = 0; i < rules.Count; i++)
                    {
                        var rule = rules[i];
                        parameterLists[i][j] = bit
                            ? rule.CreateParameter(cppParameter, paramCsName, kind, direction, settings)
                            : rule.CreateDefaultParameter(cppParameter, paramCsName, kind, direction, settings);
                    }

                    if (customParameterList != null)
                    {
                        for (int i = 0; i < customParameterList.Length; i++)
                        {
                            // mapping cant be null here, because customParameterList is only created if mapping is not null
                            customParameterList[i][j] = mapping!.CustomVariations[i].TryGetValue(paramCsName, out var paramType)
                                ? new CsParameterInfo(paramCsName, cppParameter.Type, new CsType(paramType, kind), direction)
                                : new CsParameterInfo(paramCsName, cppParameter.Type, new CsType(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                        }
                    }

                    for (int i = 0; i < rules.Count; i++)
                    {
                        var parameter = parameterLists[i][j];
                        GenerateAttributes(cppParameter, direction, parameter, parameter.Attributes);
                    }

                    if (customParameterList != null)
                    {
                        for (int i = 0; i < customParameterList.Length; i++)
                        {
                            var parameter = customParameterList[i][j];
                            GenerateAttributes(cppParameter, direction, parameter, parameter.Attributes);
                        }
                    }
                }

                for (int i = 0; i < rules.Count; i++)
                {
                    CsFunctionVariation ruleVariation = function.CreateVariationWith();
                    ruleVariation.Parameters.AddRange(parameterLists[i]);
                    if (function.TryAddVariation(ruleVariation))
                    {
                        ApplySteps(function, ruleVariation);
                    }
                }

                if (customParameterList != null)
                {
                    for (int i = 0; i < customParameterList.Length; i++)
                    {
                        CsFunctionVariation customVariation = function.CreateVariationWith();
                        customVariation.Parameters.AddRange(customParameterList[i]);
                        if (function.TryAddVariation(customVariation))
                        {
                            ApplySteps(function, customVariation);
                        }
                    }
                }
            }
        }

        public virtual void GenerateAttributes(CppParameter cppParameter, Direction direction, CsParameterInfo parameter, List<string> attributes)
        {
            string paramAttr = $"[NativeName(NativeNameType.Param, \"{cppParameter.Name}\")]";
            string typeAttr = $"[NativeName(NativeNameType.Type, \"{cppParameter.Type.GetDisplayName()}\")]";
            attributes.Add(paramAttr);
            attributes.Add(typeAttr);
        }

        public virtual unsafe void GenerateCOMVariations(IList<CppParameter> parameters, CsFunctionOverload function)
        {
            settings.TryGetFunctionMapping(function.ExportedName, out var mapping);

            if (mapping != null)
            {
                function.Comment = mapping.Comment;
            }

            long maxVariations = (long)Math.Pow(2L, parameters.Count);
            for (long ix = 0; ix < maxVariations; ix++)
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
                                    refParameterList[j] = new(paramCsName, cppParameter.Type, new("ref " + settings.GetCsTypeName(arrayType.ElementType, false), kind), direction);
                                }
                                else
                                {
                                    refParameterList[j] = new(paramCsName, cppParameter.Type, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }

                                if (arrayType.Size > 0)
                                {
                                    comPtrParameterList[j] = new(paramCsName, cppParameter.Type, new("ref " + settings.GetCsTypeName(arrayType.ElementType, false), kind), direction);
                                }
                                else
                                {
                                    comPtrParameterList[j] = new(paramCsName, cppParameter.Type, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }

                                if (arrayType.ElementType.IsString())
                                {
                                    stringParameterList[j] = new(paramCsName, cppParameter.Type, new("string[]", kind), direction);
                                }
                                else
                                {
                                    stringParameterList[j] = new(paramCsName, cppParameter.Type, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }
                            }
                            else
                            {
                                refParameterList[j] = new(paramCsName, cppParameter.Type, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);

                                int pointerDepth = 0;
                                var name = settings.GetCsTypeName(cppParameter.Type, false).Replace("*", string.Empty);
                                if (cppParameter.Type.IsPointer(ref pointerDepth, out var pointerType) && (cppParameter.Type.IsClass(out var cppClass) && cppClass.IsCOMObject() || pointerType.IsVoid()))
                                {
                                    if (pointerDepth != 0 && name == "void")
                                    {
                                        name = "T";
                                        comVariation.GenericParameters.Add(new("T", "where T : unmanaged, IComObject, IComObject<T>"));

                                        if (j > 0 && comPtrParameterList[j - 1]?.Type?.Name == "Guid*")
                                        {
                                            /*if (!function.DefaultValues.ContainsKey(comPtrParameterList[j - 1].Name))
                                            {
                                                function.DefaultValues.Add(comPtrParameterList[j - 1].Name, "ComUtils.GuidPtrOf<T>()");
                                            }*/

                                            comPtrParameterList[j - 1].DefaultValue = "ComUtils.GuidPtrOf<T>()";
                                        }
                                    }
                                    if (pointerDepth == 1)
                                    {
                                        comPtrParameterList[j] = new(paramCsName, cppParameter.Type, new($"ComPtr<{name}>", kind), direction);
                                    }
                                    else if (pointerDepth == 2)
                                    {
                                        if (j == parameters.Count - 1)
                                        {
                                            comPtrParameterList[j] = new(paramCsName, cppParameter.Type, new($"out ComPtr<{name}>", kind), direction);
                                        }
                                        else
                                        {
                                            comPtrParameterList[j] = new(paramCsName, cppParameter.Type, new($"ref ComPtr<{name}>", kind), direction);
                                        }
                                    }
                                    else
                                    {
                                        comPtrParameterList[j] = new(paramCsName, cppParameter.Type, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                    }
                                }
                                else
                                {
                                    comPtrParameterList[j] = new(paramCsName, cppParameter.Type, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }

                                if (name == "Guid"
                                    && cppParameter.Name == "riid"
                                    && cppParameter.Type is CppReferenceType refer
                                    && refer.ElementType is CppQualifiedType qual
                                    && qual.ElementType is CppTypedef def
                                    && def.Name == "IID")
                                {
                                    /*
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
                                    */
                                }

                                if (cppParameter.Type.IsString())
                                {
                                    stringParameterList[j] = new(paramCsName, cppParameter.Type, new(direction == Direction.InOut ? "ref string" : "string", kind), direction);
                                }
                                else
                                {
                                    stringParameterList[j] = new(paramCsName, cppParameter.Type, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                }
                            }

                            if (mapping != null)
                            {
                                for (int i = 0; i < mapping.CustomVariations.Count; i++)
                                {
                                    if (mapping.CustomVariations[i].TryGetValue(paramCsName, out var paramType))
                                    {
                                        customParameterList[i][j] = new(paramCsName, cppParameter.Type, new(paramType, kind), direction);
                                    }
                                    else
                                    {
                                        customParameterList[i][j] = new(paramCsName, cppParameter.Type, new(settings.GetCsWrapperTypeName(cppParameter.Type, false), kind), direction);
                                    }
                                }
                            }
                        }
                        else
                        {
                            refParameterList[j] = new(paramCsName, cppParameter.Type, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
                            stringParameterList[j] = new(paramCsName, cppParameter.Type, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
                            comPtrParameterList[j] = new(paramCsName, cppParameter.Type, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
                            if (mapping != null)
                            {
                                for (int i = 0; i < mapping.CustomVariations.Count; i++)
                                {
                                    customParameterList[i][j] = new(paramCsName, cppParameter.Type, new(settings.GetCsTypeName(cppParameter.Type, false), kind), direction);
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

                    if (function.TryAddVariation(refVariation))
                    {
                        ApplySteps(function, refVariation);
                    }
                    if (function.TryAddVariation(stringVariation))
                    {
                        ApplySteps(function, stringVariation);
                    }
                    if (function.TryAddVariation(comVariation))
                    {
                        ApplySteps(function, comVariation);
                    }
                    for (int i = 0; i < (mapping?.CustomVariations.Count ?? 0); i++)
                    {
                        CsFunctionVariation customVariation = function.CreateVariationWith();
                        customVariation.Parameters.AddRange(customParameterList[i]);
                        if (function.TryAddVariation(customVariation))
                        {
                            ApplySteps(function, customVariation);
                        }
                    }
                }
            }
        }

        public virtual void ApplySteps(CsFunctionOverload function, CsFunctionVariation variation)
        {
            foreach (var step in steps)
            {
                if (step.ShouldSkip(function, variation)) continue;
                step.GenSteps = steps;
                step.GenerateVariations(function, variation);
            }
        }

        public virtual void ApplyStep<T>(CsFunctionOverload function, CsFunctionVariation variation) where T : FunctionGenStep
        {
            foreach (var step in steps)
            {
                if (step.ShouldSkip(function, variation)) continue;
                if (step is T t)
                {
                    t.GenSteps = steps;
                    t.GenerateVariations(function, variation);
                }
            }
        }
    }
}