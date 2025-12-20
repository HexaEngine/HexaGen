namespace HexaGen.FunctionGeneration
{
    using HexaGen;
    using HexaGen.Core;
    using HexaGen.Core.Collections;
    using HexaGen.Core.CSharp;
    using HexaGen.Core.Mapping;
    using HexaGen.CppAst.Model.Declarations;
    using HexaGen.CppAst.Model.Types;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FunctionGenerator
    {
        private readonly CsCodeGeneratorConfig settings;
        private readonly List<FunctionGenRule> rules = [];
        private readonly List<FunctionGenStep> steps = [];

        protected FunctionGenerator(CsCodeGeneratorConfig settings)
        {
            this.settings = settings;
        }

        public static FunctionGenerator CreateDefault(CsCodeGeneratorConfig config)
        {
            FunctionGenerator generator = new(config);
            generator.rules.Add(new FunctionGenRuleRef());
            generator.rules.Add(new FunctionGenRuleSpan());
            generator.rules.Add(new FunctionGenRuleString());
            generator.rules.Add(new FunctionGenRuleArray(config));
            generator.steps.Add(new DefaultValueGenStep());
            generator.steps.Add(new ReturnVariationGenStep());
            generator.steps.Add(new StringReturnGenStep());
            return generator;
        }

        public static FunctionGenerator CreateForCOM(CsCodeGeneratorConfig config)
        {
            FunctionGenerator generator = new(config);
            generator.rules.Add(new FunctionGenRuleRef());
            generator.rules.Add(new FunctionGenRuleSpan());
            generator.rules.Add(new FunctionGenRuleString());
            generator.rules.Add(new FunctionGenRuleCOM());
            generator.rules.Add(new FunctionGenRuleArray(config));
            generator.steps.Add(new DefaultValueGenStep());
            generator.steps.Add(new ReturnVariationGenStep());
            generator.steps.Add(new StringReturnGenStep());
            return generator;
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
                var paramCsTypeName = settings.GetCsTypeName(cppField.Type);
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
                        var arrayElementTypeName = settings.GetCsWrappedPointerTypeName(arrayType.ElementType);
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

            List<ParameterMapping>? parameterMappings = mapping?.Parameters;

            long maxVariations = (long)Math.Pow(2L, parameters.Count);
            for (long ix = 0; ix < maxVariations; ix++)
            {
                CsFunctionVariation[] variations = new CsFunctionVariation[rules.Count];
                CsParameterInfo[][] parameterLists = new CsParameterInfo[rules.Count][];
                for (int i = 0; i < rules.Count; i++)
                {
                    variations[i] = function.CreateVariationWith();
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
                    var bit = (ix & 1 << j - 64) != 0;
                    CppParameter cppParameter = parameters[j];
                    CppPrimitiveKind kind = cppParameter.Type.GetPrimitiveKind();

                    var paramCsName = settings.GetParameterName(j, cppParameter.Name);
                    var direction = cppParameter.Type.GetDirection();

                    ParameterMapping? paramMapping = parameterMappings.Get(j);

                    if (paramMapping?.FriendlyName != null)
                    {
                        paramCsName = paramMapping.FriendlyName;
                    }

                    for (int i = 0; i < rules.Count; i++)
                    {
                        var rule = rules[i];
                        parameterLists[i][j] = bit
                            ? rule.CreateParameter(cppParameter, paramMapping, paramCsName, kind, direction, settings, parameters, parameterLists[i], j, variations[i])
                            : rule.CreateDefaultParameter(cppParameter, paramMapping, paramCsName, kind, direction, settings);
                    }

                    if (customParameterList != null)
                    {
                        for (int i = 0; i < customParameterList.Length; i++)
                        {
                            // mapping cant be null here, because customParameterList is only created if mapping is not null
                            customParameterList[i][j] = mapping!.CustomVariations[i].TryGetValue(paramCsName, out var paramType)
                                ? new CsParameterInfo(paramCsName, cppParameter.Type, new CsType(paramType, kind), direction)
                                : new CsParameterInfo(paramCsName, cppParameter.Type, new CsType(settings.GetCsWrapperTypeName(cppParameter.Type), kind), direction);
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
                    CsFunctionVariation ruleVariation = variations[i];
                    ruleVariation.Parameters.AddRange(parameterLists[i]);
                    if (function.TryAddVariation(ruleVariation))
                    {
                        ApplySteps(function, ruleVariation);
                    }
                }

                if (settings.GenerateAdditionalOverloads)
                {
                    foreach (var valueVariation in GenerateAdditionalOverloads(function.Name, parameterLists))
                    {
                        if (function.TryAddVariation(valueVariation, out var variation))
                        {
                            ApplySteps(function, variation);
                        }
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

        private static (int, int) FindDifferenceRange(CsParameterInfo[][] overloads)
        {
            int start = -1;
            int end = -1;
            for (int i = 0; i < overloads[0].Length; i++)
            {
                bool found = false;
                for (int j = 1; j < overloads.Length; j++)
                {
                    var overload = overloads[j];
                    if (overloads[0][i].Type.Name != overload[i].Type.Name || overloads[0][i].DefaultValue != overload[i].DefaultValue)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    if (start == -1)
                    {
                        start = i;
                    }
                    end = i + 1;
                }
            }

            return (start, end);
        }

        private IEnumerable<ValueVariation> GenerateAdditionalOverloads(string name, CsParameterInfo[][] overloads)
        {
            var (start, end) = FindDifferenceRange(overloads);

            if (start == -1)
            {
                return Array.Empty<ValueVariation>();
            }

            HashSet<ValueVariation> variations = [];

            foreach (var originalOverload in overloads)
            {
                variations.Add(new ValueVariation(name, originalOverload));
            }

            var baseList = new List<CsParameterInfo>();
            baseList.AddRange(overloads[0].AsSpan(0, start));

            return GenerateCombinations(name, overloads, variations, baseList, start, end);
        }

        private IEnumerable<ValueVariation> GenerateCombinations(string name, CsParameterInfo[][] overloads, HashSet<ValueVariation> variations, List<CsParameterInfo> current, int depth, int end)
        {
            if (depth == end)
            {
                int delta = overloads[0].Length - end;
                if (delta != 0)
                {
                    current.AddRange(overloads[0].AsSpan(end, delta));
                }

                var variation = new ValueVariation(name, current);
                if (!variations.Contains(variation))
                {
                    var clone = current.Clone();
                    var newVariation = new ValueVariation(name, clone);
                    yield return newVariation;
                    variations.Add(newVariation);
                }

                if (delta != 0)
                {
                    current.RemoveRange(end, delta);
                }

                yield break;
            }

            for (int i = 0; i < overloads.Length; i++)
            {
                var param = overloads[i][depth];
                if (settings.VaryingTypes.Count > 0 && !settings.VaryingTypes.Contains(param.Type.Name))
                {
                    param = overloads[0][depth];
                }

                current.Add(param);
                foreach (var variation in GenerateCombinations(name, overloads, variations, current, depth + 1, end))
                {
                    yield return variation;
                }
                current.RemoveAt(current.Count - 1);
            }
        }

        public virtual void GenerateAttributes(CppParameter cppParameter, Direction direction, CsParameterInfo parameter, List<string> attributes)
        {
            string paramAttr = $"[NativeName(NativeNameType.Param, \"{cppParameter.Name}\")]";
            string typeAttr = $"[NativeName(NativeNameType.Type, \"{cppParameter.Type.GetDisplayName()}\")]";
            attributes.Add(paramAttr);
            attributes.Add(typeAttr);
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