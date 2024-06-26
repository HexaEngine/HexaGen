namespace Hexa.NET.X3DAudio
{
    using System.Runtime.CompilerServices;

    public unsafe struct X3DAudioHandle
    {
        public DataInlineArray Data;

        [InlineArray(80)]
        public struct DataInlineArray
        {
            private byte _byte;
        }
    }
}