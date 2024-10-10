namespace HexaGen
{
    using HexaGen.Metadata;
    using System.Text;

    public class FunctionTableBuilder
    {
        private readonly StringBuilder sb = new();
        private int index;

        private readonly List<CsFunctionTableEntry> entries = [];
        private readonly Dictionary<string, int> entryPointToIndex = [];

        public FunctionTableBuilder()
        {
        }

        public FunctionTableBuilder(int funcTableableStart)
        {
            index = funcTableableStart;
        }

        public List<CsFunctionTableEntry> Entries => entries;

        public void Append(List<CsFunctionTableEntry> functionTableEntries)
        {
            foreach (var entry in functionTableEntries)
            {
                entryPointToIndex.Add(entry.EntryPoint, entry.Index);
                entries.Add(entry);
                sb.AppendLine($"funcTable.Load({entry.Index}, \"{entry.EntryPoint}\");");
                index = Math.Max(index, entry.Index + 1);
            }
        }

        public int Add(string name)
        {
            if (entryPointToIndex.TryGetValue(name, out var id))
            {
                return id;
            }

            id = index;
            entries.Add(new(id, name));
            entryPointToIndex.Add(name, id);
            sb.AppendLine($"funcTable.Load({id}, \"{name}\");");
            index++;
            return id;
        }

        public string Finish(out int count)
        {
            count = entries.Count;
            return sb.ToString();
        }
    }
}