namespace HexaGen.Runtime
{
    using System;
    using System.Runtime.InteropServices;

    public class NativeLibraryContext : INativeContext
    {
        private nint library;

        public NativeLibraryContext(nint library)
        {
            this.library = library;
        }

        public NativeLibraryContext(string libraryPath)
        {
            library = NativeLibrary.Load(libraryPath);
        }

        public nint GetProcAddress(string procName)
        {
            if (!NativeLibrary.TryGetExport(library, procName, out var address))
            {
                return 0;
            }

            return address;
        }

        public bool TryGetProcAddress(string procName, out nint address)
        {
            return NativeLibrary.TryGetExport(library, procName, out address);
        }

        public void Dispose()
        {
            if (library != 0)
            {
                NativeLibrary.Free(library);
                library = 0;
            }
            GC.SuppressFinalize(this);
        }

        public bool IsExtensionSupported(string extensionName)
        {
            return false;
        }
    }
}