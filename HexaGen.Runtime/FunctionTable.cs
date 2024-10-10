namespace HexaGen.Runtime
{
    using System.Runtime.InteropServices;

    public unsafe class FunctionTable : IDisposable
    {
        private nint library;
        private void** _vtable;
        private int length;

        public FunctionTable(void** vtable, int length)
        {
            _vtable = vtable;
            this.length = length;
        }

        public FunctionTable(nint library, int length)
        {
            _vtable = (void**)Marshal.AllocHGlobal(length * sizeof(void*));
            new Span<nint>(_vtable, length).Clear(); // Fill with null pointers
            this.library = library;
            this.length = length;
        }

        public FunctionTable(string libraryPath, int length)
        {
            library = NativeLibrary.Load(libraryPath);
            _vtable = (void**)Marshal.AllocHGlobal(length * sizeof(void*));
            new Span<nint>(_vtable, length).Clear(); // Fill with null pointers
            this.length = length;
        }

        public int Length => length;

        public void Load(int index, string export)
        {
            if (!NativeLibrary.TryGetExport(library, export, out var address))
            {
                _vtable[index] = null;
                return;
            }

            _vtable[index] = (void*)address;
        }

        public void Resize(int newLength)
        {
            if (newLength == length)
                return;

            _vtable = (void**)Marshal.ReAllocHGlobal((nint)_vtable, (nint)(newLength * sizeof(void*)));
            length = newLength;
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