namespace HexaGen.Metadata
{
    public class CsFunctionTableEntry
    {
        public CsFunctionTableEntry(int index, string entryPoint)
        {
            Index = index;
            EntryPoint = entryPoint;
        }

        public int Index { get; set; }

        public string EntryPoint { get; set; }

        public CsFunctionTableEntry Clone()
        {
            return new(Index, EntryPoint);
        }
    }
}