#nullable disable

namespace Hexa.NET.Bgfx
{
    using HexaGen.Runtime;
    using System.Numerics;
    using System.Runtime.InteropServices;

    public static unsafe partial class Bgfx
    {
        static Bgfx()
        {
            InitApi();

            LibraryLoader.CustomLoadFolders.Add("your/custom/path/relative/to/app");

            LibraryLoader.CustomLoadFolders.Add("C:/your/custom/path/absolute");

            LibraryLoader.InterceptLibraryName = (ref string libraryName) =>
            {
                // note extensions will be automatically added if not present.
                if (libraryName == "cimgui")
                {
                    libraryName = "customCimguiName";
                }
            };

            LibraryLoader.ResolvePath = (string libraryName, out string pathToLibrary) =>
            {
                pathToLibrary = Path.GetFullPath(libraryName);
            };
        }

        public static nint GetLibraryName()
        {
            return LibraryLoader.LoadLibrary();
        }
    }
}