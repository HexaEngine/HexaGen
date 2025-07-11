namespace HexaGen.Runtime.COM
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

        public static unsafe implicit operator ComPtr<IUnknown>(ComPtr<T> @this)
        {
            return *(ComPtr<IUnknown>*)&@this;
        }

        /// <summary>
        /// Casts the current <see cref="ComPtr{T}"/> to a <see cref="ComPtr{TAs}"/> of a different type.
        /// </summary>
        /// <typeparam name="TAs">The target interface type to cast to, which must implement <see cref="IComObject{TAs}"/>.</typeparam>
        /// <returns>A <see cref="ComPtr{TAs}"/> cast from the original <see cref="ComPtr{T}"/>. The casted pointer should be used only if both types are compatible.</returns>
        /// <remarks>
        /// Ensure the compatibility of <typeparamref name="T"/> and <typeparamref name="TAs"/>; this method does not enforce type safety.
        /// It is intended for scenarios where a valid cast is known but cannot be verified at compile time.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly unsafe ComPtr<TAs> As<TAs>() where TAs : unmanaged, IComObject<TAs>
        {
            ComPtr<T> ptr = this;
            return *(ComPtr<TAs>*)&ptr;
        }

        public readonly unsafe ComObject? AsComObject()
        {
            return ComObject.FromPtr((IUnknown*)Handle);
        }

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