namespace HexaGen.FunctionGeneration.ParameterWriters
{
    public readonly struct ParameterPriorityComparer : IComparer<IParameterWriter>
    {
        public int Compare(IParameterWriter? x, IParameterWriter? y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            return y.Priority.CompareTo(x.Priority);
        }
    }
}