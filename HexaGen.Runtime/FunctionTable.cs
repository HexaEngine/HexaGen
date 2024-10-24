namespace HexaGen.Runtime
{
    using System;
    using System.Runtime.InteropServices;

    public unsafe class FunctionTable : IDisposable
    {
        private void** _vtable;
        private int length;
        private readonly INativeContext context;

        public FunctionTable(void** vtable, int length)
        {
            context = null!;
            _vtable = vtable;
            this.length = length;
        }

        public FunctionTable(nint library, int length) : this(new NativeLibraryContext(library), length)
        {
        }

        public FunctionTable(string libraryPath, int length) : this(new NativeLibraryContext(libraryPath), length)
        {
        }

        public FunctionTable(INativeContext context, int length)
        {
            this.context = context;
            _vtable = (void**)Marshal.AllocHGlobal(length * sizeof(void*));
            new Span<nint>(_vtable, length).Clear(); // Fill with null pointers
            this.length = length;
        }

        public int Length => length;

        public void Load(int index, string export)
        {
            if (!context.TryGetProcAddress(export, out var address))
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

            context.Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Free();
        }
    }
}