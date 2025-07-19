namespace HexaGen.Metadata
{
    using HexaGen.Core.CSharp;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System.Diagnostics.CodeAnalysis;

    public class CsCodeGeneratorMetadata
    {
        private readonly Dictionary<string, GeneratorMetadataEntry> entries = [];

        public CsCodeGeneratorConfig Settings { get; set; } = null!;

        public Dictionary<string, GeneratorMetadataEntry> Entries => entries;

        public GeneratorMetadataEntry this[string index]
        {
            get => Entries[index];
            set => Entries[index] = value;
        }

        public List<CsConstantMetadata> DefinedConstants
        {
            get => GetOrCreate<MetadataListEntry<CsConstantMetadata>>("DefinedConstants").Values;
            set => entries["DefinedConstants"] = new MetadataListEntry<CsConstantMetadata>(value);
        }

        public List<CsEnumMetadata> DefinedEnums
        {
            get => GetOrCreate<MetadataListEntry<CsEnumMetadata>>("DefinedEnums").Values;
            set => entries["DefinedEnums"] = new MetadataListEntry<CsEnumMetadata>(value);
        }

        public List<string> DefinedExtensionTypes
        {
            get => GetOrCreate<MetadataListEntry<string>>("DefinedExtensionTypes").Values;
            set => entries["DefinedExtensionTypes"] = new MetadataListEntry<string>(value);
        }

        public List<CsFunction> DefinedExtensions
        {
            get => GetOrCreate<MetadataListEntry<CsFunction>>("DefinedExtensions").Values;
            set => entries["DefinedExtensions"] = new MetadataListEntry<CsFunction>(value);
        }

        public List<string> DefinedCOMExtensionTypes
        {
            get => GetOrCreate<MetadataListEntry<string>>("DefinedCOMExtensionTypes").Values;
            set => entries["DefinedCOMExtensionTypes"] = new MetadataListEntry<string>(value);
        }

        public Dictionary<string, HashSet<CsFunctionVariation>> DefinedCOMExtensions
        {
            get => GetOrCreate<MetadataDictionaryEntry<string, HashSet<CsFunctionVariation>>>("DefinedCOMExtensions").Dictionary;
            set => entries["DefinedCOMExtensions"] = new MetadataDictionaryEntry<string, HashSet<CsFunctionVariation>>(value);
        }

        public List<string> CppDefinedFunctions
        {
            get => GetOrCreate<MetadataListEntry<string>>("CppDefinedFunctions").Values;
            set => entries["CppDefinedFunctions"] = new MetadataListEntry<string>(value);
        }

        public List<CsFunction> DefinedFunctions
        {
            get => GetOrCreate<MetadataListEntry<CsFunction>>("DefinedFunctions").Values;
            set => entries["DefinedFunctions"] = new MetadataListEntry<CsFunction>(value);
        }

        public List<string> DefinedTypedefs
        {
            get => GetOrCreate<MetadataListEntry<string>>("DefinedTypedefs").Values;
            set => entries["DefinedTypedefs"] = new MetadataListEntry<string>(value);
        }

        public List<string> DefinedTypes
        {
            get => GetOrCreate<MetadataListEntry<string>>("DefinedTypes").Values;
            set => entries["DefinedTypes"] = new MetadataListEntry<string>(value);
        }

        public List<CsDelegate> DefinedDelegates
        {
            get => GetOrCreate<MetadataListEntry<CsDelegate>>("DefinedDelegates").Values;
            set => entries["DefinedDelegates"] = new MetadataListEntry<CsDelegate>(value);
        }

        public Dictionary<string, string> WrappedPointers
        {
            get => GetOrCreate<MetadataDictionaryEntry<string, string>>("WrappedPointers").Dictionary;
            set => entries["WrappedPointers"] = new MetadataDictionaryEntry<string, string>(value);
        }

        public CsFunctionTableMetadata FunctionTable
        {
            get => GetOrCreate<CsFunctionTableMetadata>("FunctionTable");
            set => entries["FunctionTable"] = value;
        }

        public bool ContainsKey(string key)
        {
            return Entries.ContainsKey(key);
        }

        public bool TryGetEntry(string key, [NotNullWhen(true)] out GeneratorMetadataEntry? entry)
        {
            return Entries.TryGetValue(key, out entry);
        }

        public bool TryGetEntry<T>(string key, [NotNullWhen(true)] out T? entry) where T : GeneratorMetadataEntry
        {
            bool result = Entries.TryGetValue(key, out var metadataEntry);
            if (result && metadataEntry is T t)
            {
                entry = t;
                return true;
            }
            entry = default;
            return false;
        }

        public T? GetEntry<T>(string key) where T : GeneratorMetadataEntry
        {
            bool result = Entries.TryGetValue(key, out var metadataEntry);
            if (result && metadataEntry is T t)
            {
                return t;
            }

            return default;
        }

        public T GetOrCreate<T>(string key) where T : GeneratorMetadataEntry, new()
        {
            T entryT;
            if (TryGetEntry(key, out var entry))
            {
                if (entry is T t)
                {
                    return t;
                }
            }

            entryT = new T();
            Entries[key] = entryT;
            return entryT;
        }

        public void Merge(CsCodeGeneratorMetadata from, in MergeOptions options)
        {
            foreach (var item in from.Entries)
            {
                if (TryGetEntry(item.Key, out var entry))
                {
                    entry.Merge(item.Value, options);
                }
            }
        }

        public CsCodeGeneratorMetadata Clone(bool shallow = false)
        {
            CsCodeGeneratorMetadata metadata = new();
            metadata.Settings = Settings;
            foreach (var item in Entries)
            {
                if (shallow)
                {
                    metadata.Entries[item.Key] = item.Value;
                }
                else
                {
                    metadata.Entries[item.Key] = item.Value.Clone();
                }
            }
            return metadata;
        }

        private static readonly JsonSerializerSettings options = new()
        {
            Formatting = Formatting.Indented,
            Converters = { new StringEnumConverter() }
        };

        private static readonly JsonSerializer serializer = JsonSerializer.Create(options);

        public void Save(string path)
        {
            using var fs = File.CreateText(path);
            using JsonTextWriter writer = new(fs);
            serializer.Serialize(writer, this);
        }

        public static CsCodeGeneratorMetadata Load(string path)
        {
            using var fs = File.OpenText(path);
            using JsonTextReader reader = new(fs);
            return serializer.Deserialize<CsCodeGeneratorMetadata>(reader) ?? new();
        }
    }
}