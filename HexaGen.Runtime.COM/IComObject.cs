namespace HexaGen.Runtime.COM
{
    public interface IComObject
    {
        public unsafe void*** AsVtblPtr();
    }

    public interface IComObject<T> : IComObject where T : IComObject
    {
    }
}