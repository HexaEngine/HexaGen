﻿namespace HexaGen.Runtime.COM
{
    using System.Runtime.CompilerServices;
    using System.Runtime.Versioning;

    public unsafe struct ComPtr<T> : IComObject, IComObject<T>, IDisposable where T : unmanaged, IComObject<T>
    {
        public T* Handle;

        public ComPtr(T* handle)
        {
            Handle = handle;
        }

        public unsafe ComPtr(ComPtr<T> other)
            : this(other.Handle)
        {
        }

#if NET5_0_OR_GREATER

        [SupportedOSPlatform("windows")]
        public unsafe ComPtr(ComObject obj) : this((T*)obj.Handle)
        {
        }

#else

        public unsafe ComPtr(ComObject obj) : this((T*)obj.Handle)
        {
        }

#endif

        public static unsafe implicit operator ComPtr<T>(T* other)
        {
            return new ComPtr<T>(other);
        }

        public static unsafe implicit operator T*(ComPtr<T> @this)
        {
            return @this.Handle;
        }

        public static unsafe implicit operator IUnknown*(ComPtr<T> @this)
        {
            return (IUnknown*)@this.Handle;
        }

#if NET5_0_OR_GREATER

        [SupportedOSPlatform("windows")]
        public readonly unsafe ComObject? AsComObject()
        {
            return ComObject.FromPtr((IUnknown*)Handle);
        }

#else

        public readonly unsafe ComObject? AsComObject()
        {
            return ComObject.FromPtr((IUnknown*)Handle);
        }
#endif

        private readonly unsafe void AddRef()
        {
            if (Handle != null && (*Handle) is IComObject<IUnknown>)
            {
                ((IUnknown*)Handle->AsVtblPtr())->AddRef();
            }
        }

        public uint Release()
        {
            uint result = 0u;
            if (Handle != null && (*Handle) is IComObject<IUnknown>)
            {
                result = ((IUnknown*)Handle->AsVtblPtr())->Release();
            }

            return result;
        }

        public void Dispose()
        {
            Release();
        }

        public unsafe T* Detach()
        {
            T* handle = Handle;
            Handle = null;
            return handle;
        }

        public readonly unsafe T** GetAddressOf()
        {
            return (T**)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
        }

        public readonly unsafe ref T* GetPinnableReference()
        {
            return ref *GetAddressOf();
        }

        public readonly unsafe ref T Get()
        {
            return ref *Handle;
        }

        public unsafe void*** AsVtblPtr()
        {
            return (void***)Handle;
        }
    }
}