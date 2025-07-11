namespace HexaGen.Metadata
{
    public abstract class GeneratorMetadataEntry
    {
        public abstract GeneratorMetadataEntry Clone();

        public abstract void Merge(GeneratorMetadataEntry from, in MergeOptions options);
    }

    public abstract class GeneratorMetadataEntry<T> : GeneratorMetadataEntry where T : GeneratorMetadataEntry
    {
        public override void Merge(GeneratorMetadataEntry from, in MergeOptions options)
        {
            if (from is T t)
            {
                Merge(t, options);
            }
        }

        public abstract void Merge(T from, in MergeOptions options);
    }
}