namespace HexaGen.Runtime
{
    using System.Runtime.InteropServices;

    public unsafe class VTable : IDisposable
    {
        private nint library;
        private void** _vtable;

        public VTable(void** vtable)
        {
            _vtable = vtable;
        }

        public VTable(nint library, int size)
        {
            _vtable = (void**)Marshal.AllocHGlobal(size * sizeof(void*));
            this.library = library;
        }

        public VTable(string libraryPath, int size)
        {
            library = NativeLibrary.Load(libraryPath);
            _vtable = (void**)Marshal.AllocHGlobal(size * sizeof(void*));
        }

        public void Load(int index, string export)
        {
            if (!NativeLibrary.TryGetExport(library, export, out var address))
            {
                return;
            }

            _vtable[index] = (void*)address;
        }

        public void* this[int index]
        {
            get => _vtable[index];
            set => _vtable[index] = value;
        }

        public void Free()
        {
            if (_vtable != null)
            {
                Marshal.FreeHGlobal((nint)_vtable);
                _vtable = null;
            }

            if (library != 0)
            {
                NativeLibrary.Free(library);
                library = 0;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Free();
        }
    }
}