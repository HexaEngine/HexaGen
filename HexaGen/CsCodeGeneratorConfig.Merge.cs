namespace HexaGen
{
    using System.Collections.Generic;

    public partial class CsCodeGeneratorConfig
    {
        public void Merge(CsCodeGeneratorConfig baseConfig, MergeOptions mergeOptions)
        {
            if (mergeOptions.HasFlag(MergeOptions.EnableExperimentalOptions))
            {
                EnableExperimentalOptions = baseConfig.EnableExperimentalOptions;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateSizeOfStructs))
            {
                GenerateSizeOfStructs = baseConfig.GenerateSizeOfStructs;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateConstructorsForStructs))
            {
                GenerateConstructorsForStructs = baseConfig.GenerateConstructorsForStructs;
            }

            if (mergeOptions.HasFlag(MergeOptions.DelegatesAsVoidPointer))
            {
                DelegatesAsVoidPointer = baseConfig.DelegatesAsVoidPointer;
            }

            if (mergeOptions.HasFlag(MergeOptions.WrapPointersAsHandle))
            {
                WrapPointersAsHandle = baseConfig.WrapPointersAsHandle;
            }

            if (mergeOptions.HasFlag(MergeOptions.GeneratePlaceholderComments))
            {
                GeneratePlaceholderComments = baseConfig.GeneratePlaceholderComments;
            }

            if (mergeOptions.HasFlag(MergeOptions.ImportType))
            {
                ImportType = baseConfig.ImportType;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateMetadata))
            {
                GenerateMetadata = baseConfig.GenerateMetadata;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateConstants))
            {
                GenerateConstants = baseConfig.GenerateConstants;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateEnums))
            {
                GenerateEnums = baseConfig.GenerateEnums;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateExtensions))
            {
                GenerateExtensions = baseConfig.GenerateExtensions;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateFunctions))
            {
                GenerateFunctions = baseConfig.GenerateFunctions;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateHandles))
            {
                GenerateHandles = baseConfig.GenerateHandles;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateTypes))
            {
                GenerateTypes = baseConfig.GenerateTypes;
            }

            if (mergeOptions.HasFlag(MergeOptions.GenerateDelegates))
            {
                GenerateDelegates = baseConfig.GenerateDelegates;
            }

            if (mergeOptions.HasFlag(MergeOptions.OneFilePerType))
            {
                OneFilePerType = baseConfig.OneFilePerType;
            }

            if (mergeOptions.HasFlag(MergeOptions.BoolType))
            {
                BoolType = baseConfig.BoolType;
            }

            MergeDictionaries(KnownConstantNames, baseConfig.KnownConstantNames, mergeOptions, MergeOptions.KnownConstantNames);
            MergeDictionaries(KnownEnumValueNames, baseConfig.KnownEnumValueNames, mergeOptions, MergeOptions.KnownEnumValueNames);
            MergeDictionaries(KnownEnumPrefixes, baseConfig.KnownEnumPrefixes, mergeOptions, MergeOptions.KnownEnumPrefixes);
            MergeDictionaries(KnownExtensionPrefixes, baseConfig.KnownExtensionPrefixes, mergeOptions, MergeOptions.KnownExtensionPrefixes);
            MergeDictionaries(KnownExtensionNames, baseConfig.KnownExtensionNames, mergeOptions, MergeOptions.KnownExtensionNames);
            MergeDictionaries(KnownDefaultValueNames, baseConfig.KnownDefaultValueNames, mergeOptions, MergeOptions.KnownDefaultValueNames);
            MergeDictionaries(KnownConstructors, baseConfig.KnownConstructors, mergeOptions, MergeOptions.KnownConstructors);
            MergeDictionaries(KnownMemberFunctions, baseConfig.KnownMemberFunctions, mergeOptions, MergeOptions.KnownMemberFunctions);
            MergeDictionaries(IIDMappings, baseConfig.IIDMappings, mergeOptions, MergeOptions.IIDMappings);
            MergeDictionaries(NameMappings, baseConfig.NameMappings, mergeOptions, MergeOptions.NameMappings);
            MergeDictionaries(TypeMappings, baseConfig.TypeMappings, mergeOptions, MergeOptions.TypeMappings);

            MergeHashSets(IgnoredParts, baseConfig.IgnoredParts, mergeOptions, MergeOptions.IgnoredParts);
            MergeHashSets(Keywords, baseConfig.Keywords, mergeOptions, MergeOptions.Keywords);
            MergeHashSets(IgnoredFunctions, baseConfig.IgnoredFunctions, mergeOptions, MergeOptions.IgnoredFunctions);
            MergeHashSets(IgnoredTypes, baseConfig.IgnoredTypes, mergeOptions, MergeOptions.IgnoredTypes);
            MergeHashSets(IgnoredEnums, baseConfig.IgnoredEnums, mergeOptions, MergeOptions.IgnoredEnums);
            MergeHashSets(IgnoredTypedefs, baseConfig.IgnoredTypedefs, mergeOptions, MergeOptions.IgnoredTypedefs);
            MergeHashSets(IgnoredDelegates, baseConfig.IgnoredDelegates, mergeOptions, MergeOptions.IgnoredDelegates);
            MergeHashSets(IgnoredConstants, baseConfig.IgnoredConstants, mergeOptions, MergeOptions.IgnoredConstants);
            MergeHashSets(AllowedFunctions, baseConfig.AllowedFunctions, mergeOptions, MergeOptions.AllowedFunctions);
            MergeHashSets(AllowedTypes, baseConfig.AllowedTypes, mergeOptions, MergeOptions.AllowedTypes);
            MergeHashSets(AllowedEnums, baseConfig.AllowedEnums, mergeOptions, MergeOptions.AllowedEnums);
            MergeHashSets(AllowedTypedefs, baseConfig.AllowedTypedefs, mergeOptions, MergeOptions.AllowedTypedefs);
            MergeHashSets(AllowedDelegates, baseConfig.AllowedDelegates, mergeOptions, MergeOptions.AllowedDelegates);
            MergeHashSets(AllowedConstants, baseConfig.AllowedConstants, mergeOptions, MergeOptions.AllowedConstants);

            MergeLists(ConstantMappings, baseConfig.ConstantMappings, mergeOptions, MergeOptions.ConstantMappings);
            MergeLists(EnumMappings, baseConfig.EnumMappings, mergeOptions, MergeOptions.EnumMappings);
            MergeLists(FunctionMappings, baseConfig.FunctionMappings, mergeOptions, MergeOptions.FunctionMappings);
            MergeLists(HandleMappings, baseConfig.HandleMappings, mergeOptions, MergeOptions.HandleMappings);
            MergeLists(ClassMappings, baseConfig.ClassMappings, mergeOptions, MergeOptions.ClassMappings);
            MergeLists(DelegateMappings, baseConfig.DelegateMappings, mergeOptions, MergeOptions.DelegateMappings);
            MergeLists(ArrayMappings, baseConfig.ArrayMappings, mergeOptions, MergeOptions.ArrayMappings);
            MergeLists(Usings, baseConfig.Usings, mergeOptions, MergeOptions.Usings);
            MergeLists(IncludeFolders, baseConfig.IncludeFolders, mergeOptions, MergeOptions.IncludeFolders);
            MergeLists(SystemIncludeFolders, baseConfig.SystemIncludeFolders, mergeOptions, MergeOptions.SystemIncludeFolders);
            MergeLists(Defines, baseConfig.Defines, mergeOptions, MergeOptions.Defines);
            MergeLists(AdditionalArguments, baseConfig.AdditionalArguments, mergeOptions, MergeOptions.AdditionalArguments);

            // Naming conventions to be merged
            if (mergeOptions.HasFlag(MergeOptions.ConstantNamingConvention))
            {
                ConstantNamingConvention = baseConfig.ConstantNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.EnumNamingConvention))
            {
                EnumNamingConvention = baseConfig.EnumNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.EnumItemNamingConvention))
            {
                EnumItemNamingConvention = baseConfig.EnumItemNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.ExtensionNamingConvention))
            {
                ExtensionNamingConvention = baseConfig.ExtensionNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.FunctionNamingConvention))
            {
                FunctionNamingConvention = baseConfig.FunctionNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.HandleNamingConvention))
            {
                HandleNamingConvention = baseConfig.HandleNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.TypeNamingConvention))
            {
                TypeNamingConvention = baseConfig.TypeNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.DelegateNamingConvention))
            {
                DelegateNamingConvention = baseConfig.DelegateNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.ParameterNamingConvention))
            {
                ParameterNamingConvention = baseConfig.ParameterNamingConvention;
            }

            if (mergeOptions.HasFlag(MergeOptions.MemberNamingConvention))
            {
                MemberNamingConvention = baseConfig.MemberNamingConvention;
            }

            AutoSquashTypedef = baseConfig.AutoSquashTypedef;
        }

        private static void MergeDictionaries<TKey, TValue>(Dictionary<TKey, TValue> target, Dictionary<TKey, TValue> source, MergeOptions mergeOptions, MergeOptions targetOption) where TKey : notnull
        {
            if (!mergeOptions.HasFlag(targetOption))
            {
                return;
            }

            foreach (var kvp in source)
            {
                target.TryAdd(kvp.Key, kvp.Value);
            }
        }

        private static void MergeHashSets<T>(HashSet<T> target, HashSet<T> source, MergeOptions mergeOptions, MergeOptions targetOption)
        {
            if (!mergeOptions.HasFlag(targetOption))
            {
                return;
            }

            foreach (var item in source)
            {
                target.Add(item);
            }
        }

        private static void MergeLists<T>(List<T> target, List<T> source, MergeOptions mergeOptions, MergeOptions targetOption)
        {
            if (!mergeOptions.HasFlag(targetOption))
            {
                return;
            }

            foreach (var item in source)
            {
                if (!target.Contains(item))
                {
                    target.Add(item);
                }
            }
        }
    }
}