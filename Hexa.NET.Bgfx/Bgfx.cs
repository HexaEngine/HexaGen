#nullable disable

namespace Hexa.NET.Bgfx
{
    using System.Numerics;
    using System.Runtime.InteropServices;

    public static unsafe partial class Bgfx
    {
        static Bgfx()
        {
            InitApi();
        }

        public static nint GetLibraryName()
        {
            return LibraryLoader.LoadLibrary();
        }
    }
}