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
    using System.Runtime.CompilerServices;

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

            if (settings.GenerateAdditionalOverloads)
            {
                foreach (var valueVariation in GenerateAdditionalOverloads(function.Name, function.Variations))
                {
                    if (function.TryAddVariation(valueVariation, out var variation))
                    {
                        ApplySteps(function, variation);
                    }
                }
            }
        }

        private static (int, int) FindDifferenceRange(ConcurrentList<CsFunctionVariation> overloads)
        {
            int start = -1;
            int end = -1;

            var baseOverload = overloads[0];
            for (int i = 0; i < baseOverload.Parameters.Count; i++)
            {
                var rootParam = baseOverload.Parameters[i];
                bool found = false;
                for (int j = 1; j < overloads.Count; j++)
                {
                    var overload = overloads[j];
                    if (rootParam.Type.Name != overload.Parameters[i].Type.Name || rootParam.DefaultValue != overload.Parameters[i].DefaultValue)
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

        private static int FindDifference(ConcurrentList<CsFunctionVariation> overloads, ref Bitset bitset)
        {
            int setBits = 0;
            var baseOverload = overloads[0];
            for (int i = 0; i < baseOverload.Parameters.Count; i++)
            {
                var rootParam = baseOverload.Parameters[i];
                for (int j = 1; j < overloads.Count; j++)
                {
                    var overload = overloads[j];
                    if (rootParam.Type.Name != overload.Parameters[i].Type.Name || rootParam.DefaultValue != overload.Parameters[i].DefaultValue)
                    {
                        ++setBits;
                        bitset[i] = true;
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

            public Bitset(long* bits, int length, bool zero) // meant to be used with stackalloc, so no need to free the memory
            {
                this.bits = bits;
                this.length = length;
                if (zero)
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
                    if (arrayIndex >= length || arrayIndex < 0)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    return (bits[arrayIndex] & (1L << bitIndex)) != 0;
                }
                set
                {
                    int bitIndex = index & 63;
                    int arrayIndex = index >> 6;
                    if (arrayIndex >= length || arrayIndex < 0)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    if (value)
                    {
                        bits[arrayIndex] |= (1L << bitIndex);
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
                for (int i = 0; i < Length; i++)
                {
                    bits[i] = 0;
                }
            }

            public void Resize(long* newBits, int newLength, bool zero)
            {
                var toCopy = Math.Min(length, newLength);
                for (int i = 0; i < toCopy; i++)
                {
                    newBits[i] = bits[i];
                }
                if (zero && newLength > length)
                {
                    for (int i = toCopy; i < newLength; i++)
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

        private unsafe IEnumerable<ValueVariation> GenerateAdditionalOverloads(string name, ConcurrentList<CsFunctionVariation> overloads)
        {
            var overloadsBoundary = overloads.Count;
            var bits = overloads[0].Parameters.Count;
            var bitsetArraySize = (bits + 63) / 64;
            ulong* bitsetArray = stackalloc ulong[bitsetArraySize];
            Bitset bitset = new((long*)bitsetArray, bitsetArraySize, true);
            var setBits = FindDifference(overloads, ref bitset);

            // var (start, end) = FindDifferenceRange(overloads);

            if (setBits == 0)
            {
                return [];
            }

            HashSet<ValueVariation> variations = [];

            foreach (var originalOverload in overloads)
            {
                variations.Add(new ValueVariation(name, originalOverload.Parameters));
            }

            var start = bitset.FindFirstSet(0);
            var baseList = new List<CsParameterInfo>();
            baseList.AddRange(overloads[0].Parameters.Take(start));

            return GenerateCombinations(name, overloads, variations, baseList, bitset, start, setBits, overloadsBoundary);
        }

        private IEnumerable<ValueVariation> GenerateCombinations(string name, ConcurrentList<CsFunctionVariation> overloads, HashSet<ValueVariation> variations, List<CsParameterInfo> current, int depth, int end, int overloadsBoundary)
        {
            if (depth == end)
            {
                int delta = overloads[0].Parameters.Count - end;
                if (delta != 0)
                {
                    current.AddRange(overloads[0].Parameters.Skip(end).Take(delta));
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

            for (int i = 0; i < overloadsBoundary; i++)
            {
                var param = overloads[i].Parameters[depth];
                if (settings.VaryingTypes.Count > 0 && !settings.VaryingTypes.Contains(param.Type.Name))
                {
                    param = overloads[0].Parameters[depth];
                }

                current.Add(param);
                foreach (var variation in GenerateCombinations(name, overloads, variations, current, depth + 1, end, overloadsBoundary))
                {
                    yield return variation;
                }
                current.RemoveAt(current.Count - 1);
            }
        }

        private IEnumerable<ValueVariation> GenerateCombinations(string name, ConcurrentList<CsFunctionVariation> overloads, HashSet<ValueVariation> variations, List<CsParameterInfo> current, Bitset bitset, int index, int setBits, int overloadsBoundary)
        {
            int nextIndex = bitset.FindFirstSet(index);

            if (nextIndex == -1 || setBits == 0)
            {
                int totalParams = overloads[0].Parameters.Count;
                int delta = totalParams - current.Count;
                if (delta > 0)
                {
                    current.AddRange(overloads[0].Parameters.Skip(current.Count).Take(delta));
                }

                var variation = new ValueVariation(name, current);
                if (!variations.Contains(variation))
                {
                    var clone = current.Clone();
                    var newVariation = new ValueVariation(name, clone);
                    yield return newVariation;
                    variations.Add(newVariation);
                }

                if (delta > 0)
                {
                    current.RemoveRange(current.Count - delta, delta);
                }

                yield break;
            }

            int diff = nextIndex - current.Count;
            if (diff > 0)
            {
                current.AddRange(overloads[0].Parameters.Skip(current.Count).Take(diff));
            }

            for (int i = 0; i < overloadsBoundary; i++)
            {
                var param = overloads[i].Parameters[nextIndex];
                if (settings.VaryingTypes.Count > 0 && !settings.VaryingTypes.Contains(param.Type.Name))
                {
                    param = overloads[0].Parameters[nextIndex];
                }

                current.Add(param);
                foreach (var variation in GenerateCombinations(name, overloads, variations, current, bitset, nextIndex + 1, setBits - 1, overloadsBoundary))
                {
                    yield return variation;
                }
                current.RemoveAt(current.Count - 1);
            }

            if (diff > 0)
            {
                current.RemoveRange(current.Count - diff, diff);
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