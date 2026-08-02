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
                    //var bit = (ix & (1 << (j - 64))) != 0;
                    var bit = (ix & (1L << j)) != 0;
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
                    GenerateAdditionalOverloads(function, parameterLists, variations[0].Parameters);
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

        private static int FindDifferences(CsParameterInfo[][] sources, List<CsParameterInfo> baseline, ref Bitset bitset)
        {
            int setBits = 0;
            for (int parameterIndex = 0; parameterIndex < baseline.Count; parameterIndex++)
            {
                CsParameterInfo baselineParameter = baseline[parameterIndex];
                for (int sourceIndex = 1; sourceIndex < sources.Length; sourceIndex++)
                {
                    if (!baselineParameter.Conflicts(sources[sourceIndex][parameterIndex]))
                    {
                        bitset[parameterIndex] = true;
                        setBits++;
                        break;
                    }
                }
            }
            return setBits;
        }

        public unsafe struct Bitset
        {
            public const int ArraySize = 8;

            private long* bits;
            private int length;

            public Bitset(long* bits, int length, bool clear)
            {
                this.bits = bits;
                this.length = length;
                if (clear)
                {
                    Clear();
                }
            }

            public bool this[int index]
            {
                readonly get
                {
                    int bitIndex = index & 63;
                    int arrayIndex = index >> 6;
                    if ((uint)arrayIndex >= (uint)length)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    return (bits[arrayIndex] & (1L << bitIndex)) != 0;
                }
                set
                {
                    int bitIndex = index & 63;
                    int arrayIndex = index >> 6;
                    if ((uint)arrayIndex >= (uint)length)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    if (value)
                    {
                        bits[arrayIndex] |= 1L << bitIndex;
                    }
                    else
                    {
                        bits[arrayIndex] &= ~(1L << bitIndex);
                    }
                }
            }

            public readonly int Length => length;

            public readonly int BitCount => length * 64;

            public void Clear()
            {
                for (int i = 0; i < length; i++)
                {
                    bits[i] = 0;
                }
            }

            public void Resize(long* newBits, int newLength, bool clearNewBits)
            {
                int copyLength = Math.Min(length, newLength);
                for (int i = 0; i < copyLength; i++)
                {
                    newBits[i] = bits[i];
                }
                if (clearNewBits)
                {
                    for (int i = copyLength; i < newLength; i++)
                    {
                        newBits[i] = 0;
                    }
                }
                bits = newBits;
                length = newLength;
            }

            public readonly int FindFirstSet(int startIndex)
            {
                for (int i = startIndex; i < BitCount; i++)
                {
                    if (this[i])
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        private unsafe void GenerateAdditionalOverloads(CsFunctionOverload function, CsParameterInfo[][] sources, List<CsParameterInfo> baseline)
        {
            int sourceCount = sources.Length;
            CsParameterInfo[] scratch = sources[0];
            int parameterCount = scratch.Length;
            if (sourceCount < 2 || parameterCount == 0)
            {
                return;
            }

            int bitsetLength = (parameterCount + 63) / 64;
            long* bitsetStorage = stackalloc long[bitsetLength];
            Bitset differences = new(bitsetStorage, bitsetLength, true);
            int differenceCount = FindDifferences(sources, baseline, ref differences);
            if (differenceCount == 0)
            {
                return;
            }

            Span<int> differingPositions = stackalloc int[differenceCount];
            int parameterPosition = differences.FindFirstSet(0);
            for (int i = 0; i < differenceCount; i++)
            {
                differingPositions[i] = parameterPosition;
                parameterPosition = differences.FindFirstSet(parameterPosition + 1);
            }

            Span<int> choiceCounts = stackalloc int[differenceCount];
            Span<int> choiceSources = stackalloc int[differenceCount * sourceCount];
            Span<int> varyingPositions = stackalloc int[differenceCount];
            choiceCounts.Clear();

            int varyingCount = 0;
            for (int differenceIndex = 0; differenceIndex < differenceCount; differenceIndex++)
            {
                int parameterIndex = differingPositions[differenceIndex];
                int choiceOffset = differenceIndex * sourceCount;
                for (int sourceIndex = 0; sourceIndex < sourceCount; sourceIndex++)
                {
                    int candidateSource = sourceIndex;
                    CsParameterInfo candidate = sources[sourceIndex][parameterIndex];
                    if (settings.VaryingTypes.Count > 0 && !settings.VaryingTypes.Contains(candidate.Type.Name))
                    {
                        candidateSource = 0;
                        candidate = baseline[parameterIndex];
                    }

                    bool duplicate = false;
                    for (int choiceIndex = 0; choiceIndex < choiceCounts[differenceIndex]; choiceIndex++)
                    {
                        int existingSource = choiceSources[choiceOffset + choiceIndex];
                        CsParameterInfo existing = existingSource == 0
                            ? baseline[parameterIndex]
                            : sources[existingSource][parameterIndex];
                        if (existing.Conflicts(candidate))
                        {
                            duplicate = true;
                            break;
                        }
                    }

                    if (!duplicate)
                    {
                        choiceSources[choiceOffset + choiceCounts[differenceIndex]++] = candidateSource;
                    }
                }

                if (choiceCounts[differenceIndex] > 1)
                {
                    varyingPositions[varyingCount++] = differenceIndex;
                }
            }

            if (varyingCount == 0)
            {
                return;
            }

            Span<int> indices = stackalloc int[varyingCount];
            indices.Clear();
            for (int i = 0; i < varyingCount; i++)
            {
                int differenceIndex = varyingPositions[i];
                int parameterIndex = differingPositions[differenceIndex];
                int sourceIndex = choiceSources[differenceIndex * sourceCount];
                scratch[parameterIndex] = sourceIndex == 0
                    ? baseline[parameterIndex]
                    : sources[sourceIndex][parameterIndex];
            }

            while (true)
            {
                ValueVariation candidate = new(function.Name, scratch);
                if (!IsSourceVariation(sources, baseline, scratch, differingPositions) && !function.HasVariation(candidate))
                {
                    CsFunctionVariation variation = function.CreateVariationWith();
                    variation.Parameters.Capacity = parameterCount;
                    variation.Parameters.AddRange(scratch);
                    if (function.TryAddVariation(variation))
                    {
                        ApplySteps(function, variation);
                    }
                }

                int varyingIndex = varyingCount - 1;
                for (; varyingIndex >= 0; varyingIndex--)
                {
                    int differenceIndex = varyingPositions[varyingIndex];
                    int parameterIndex = differingPositions[differenceIndex];
                    int next = indices[varyingIndex] + 1;
                    if (next < choiceCounts[differenceIndex])
                    {
                        indices[varyingIndex] = next;
                        int sourceIndex = choiceSources[differenceIndex * sourceCount + next];
                        scratch[parameterIndex] = sourceIndex == 0
                            ? baseline[parameterIndex]
                            : sources[sourceIndex][parameterIndex];
                        break;
                    }

                    indices[varyingIndex] = 0;
                    int firstSource = choiceSources[differenceIndex * sourceCount];
                    scratch[parameterIndex] = firstSource == 0
                        ? baseline[parameterIndex]
                        : sources[firstSource][parameterIndex];
                }

                if (varyingIndex < 0)
                {
                    return;
                }
            }
        }

        private static bool IsSourceVariation(CsParameterInfo[][] sources, List<CsParameterInfo> baseline, CsParameterInfo[] candidate, Span<int> differingPositions)
        {
            for (int sourceIndex = 0; sourceIndex < sources.Length; sourceIndex++)
            {
                bool matches = true;
                for (int i = 0; i < differingPositions.Length; i++)
                {
                    int parameterIndex = differingPositions[i];
                    CsParameterInfo sourceParameter = sourceIndex == 0
                        ? baseline[parameterIndex]
                        : sources[sourceIndex][parameterIndex];
                    if (!sourceParameter.Conflicts(candidate[parameterIndex]))
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                {
                    return true;
                }
            }
            return false;
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
