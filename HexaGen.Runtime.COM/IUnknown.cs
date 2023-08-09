namespace HexaGen.Runtime.COM
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [Guid("00000000-0000-0000-C000-000000000046")]
    public struct IUnknown : IComObject, IComObject<IUnknown>
    {
        public static readonly Guid Guid = new("00000000-0000-0000-C000-000000000046");

        public unsafe void** LpVtbl;

        unsafe void*** IComObject.AsVtblPtr()
        {
            return (void***)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
        }

        public readonly unsafe HResult QueryInterface(Guid* riid, void** ppvObject)
        {
            IUnknown* ptr = (IUnknown*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, int>)(*LpVtbl))(ptr, riid, ppvObject);
        }

        public readonly unsafe int QueryInterface(Guid* riid, ref void* ppvObject)
        {
            IUnknown* ptr = (IUnknown*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            fixed (void** ptr2 = &ppvObject)
            {
                return ((delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, int>)(*LpVtbl))(ptr, riid, ptr2);
            }
        }

        public readonly unsafe int QueryInterface(ref Guid riid, void** ppvObject)
        {
            IUnknown* ptr = (IUnknown*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            fixed (Guid* ptr2 = &riid)
            {
                return ((delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, int>)(*LpVtbl))(ptr, ptr2, ppvObject);
            }
        }

        public readonly unsafe int QueryInterface(ref Guid riid, ref void* ppvObject)
        {
            IUnknown* ptr = (IUnknown*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            fixed (Guid* ptr2 = &riid)
            {
                fixed (void** ptr3 = &ppvObject)
                {
                    return ((delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, int>)(*LpVtbl))(ptr, ptr2, ptr3);
                }
            }
        }

        public readonly unsafe uint AddRef()
        {
            IUnknown* ptr = (IUnknown*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IUnknown*, uint>)LpVtbl[1])(ptr);
        }

        public readonly unsafe uint Release()
        {
            IUnknown* ptr = (IUnknown*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IUnknown*, uint>)LpVtbl[2])(ptr);
        }
    }
}