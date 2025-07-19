namespace HexaGen.Metadata
{
    using HexaGen.Core;
    using Newtonsoft.Json;
    using System.Collections.Frozen;

    public class CsFunctionTableMetadata : GeneratorMetadataEntry, ICloneable<CsFunctionTableMetadata>
    {
        [JsonConstructor]
        public CsFunctionTableMetadata(List<CsFunctionTableEntry> entries)
        {
            Entries = entries;
        }

        public CsFunctionTableMetadata()
        {
            Entries = [];
        }

        public List<CsFunctionTableEntry> Entries { get; set; }

        public void Merge(CsFunctionTableMetadata from)
        {
            FrozenDictionary<int, CsFunctionTableEntry> indicesLookupTable = Entries.ToFrozenDictionary(x => x.Index);
            FrozenDictionary<string, CsFunctionTableEntry> entryPointLookupTable = Entries.ToFrozenDictionary(x => x.EntryPoint);
            foreach (CsFunctionTableEntry entry in from.Entries)
            {
                if (entryPointLookupTable.TryGetValue(entry.EntryPoint, out var tableEntry))
                {
                    if (tableEntry.Index != entry.Index)
                    {
                        throw new InvalidOperationException($"Duplicate entry point found '{entry.EntryPoint}' with not the same index '{entry.Index}' vs '{tableEntry.Index}'.");
                    }

                    continue;
                }

                if (indicesLookupTable.TryGetValue(entry.Index, out tableEntry))
                {
                    if (tableEntry.EntryPoint != entry.EntryPoint)
                    {
                        throw new InvalidOperationException($"Duplicate index found '{entry.Index}' with not the same entry point '{entry.EntryPoint}' vs '{tableEntry.EntryPoint}'.");
                    }

                    continue;
                }

                Entries.Add(entry.Clone());
            }
        }

        CsFunctionTableMetadata ICloneable<CsFunctionTableMetadata>.Clone()
        {
            return new(Entries.Select(x => x.Clone()).ToList());
        }

        public override GeneratorMetadataEntry Clone()
        {
            return new CsFunctionTableMetadata(Entries.Select(x => x.Clone()).ToList());
        }

        public override void Merge(GeneratorMetadataEntry from, in MergeOptions options)
        {
            if (options.MergeFunctionTable && from is CsFunctionTableMetadata metadata)
            {
                Merge(metadata);
            }
        }
    }
}