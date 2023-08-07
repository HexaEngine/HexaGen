namespace HexaGen.Runtime.COM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    public struct Point32
    {
        public uint X;
        public uint Y;
    }

    public struct Rect32
    {
        public uint X;
        public uint Y;
        public uint Width;
        public uint Height;
    }

    public struct SecurityAttributes
    {
        public uint Length;
        public unsafe void* SecurityDescriptor;
        public Bool32 InheritHandle;
    }
}