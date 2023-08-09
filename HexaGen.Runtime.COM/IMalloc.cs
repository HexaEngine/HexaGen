namespace HexaGen.Runtime.COM
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [Guid("00000002-0000-0000-C000-000000000046")]
    public struct IMalloc : IComObject, IComObject<IMalloc>, IComObject<IUnknown>
    {
        public static readonly Guid Guid = new("00000002-0000-0000-C000-000000000046");

        public unsafe void** LpVtbl;

        unsafe void*** IComObject.AsVtblPtr()
        {
            return (void***)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
        }

        public readonly unsafe HResult QueryInterface(Guid* riid, void** ppvObject)
        {
            IMalloc* ptr = (IMalloc*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IMalloc*, Guid*, void**, int>)(*LpVtbl))(ptr, riid, ppvObject);
        }

        public readonly unsafe int QueryInterface(Guid* riid, ref void* ppvObject)
        {
            IMalloc* ptr = (IMalloc*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            fixed (void** ptr2 = &ppvObject)
            {
                return ((delegate* unmanaged[Stdcall]<IMalloc*, Guid*, void**, int>)(*LpVtbl))(ptr, riid, ptr2);
            }
        }

        public readonly unsafe int QueryInterface(ref Guid riid, void** ppvObject)
        {
            IMalloc* ptr = (IMalloc*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            fixed (Guid* ptr2 = &riid)
            {
                return ((delegate* unmanaged[Stdcall]<IMalloc*, Guid*, void**, int>)(*LpVtbl))(ptr, ptr2, ppvObject);
            }
        }

        public readonly unsafe int QueryInterface(ref Guid riid, ref void* ppvObject)
        {
            IMalloc* ptr = (IMalloc*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            fixed (Guid* ptr2 = &riid)
            {
                fixed (void** ptr3 = &ppvObject)
                {
                    return ((delegate* unmanaged[Stdcall]<IMalloc*, Guid*, void**, int>)(*LpVtbl))(ptr, ptr2, ptr3);
                }
            }
        }

        public readonly unsafe uint AddRef()
        {
            IMalloc* ptr = (IMalloc*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IMalloc*, uint>)LpVtbl[1])(ptr);
        }

        public readonly unsafe uint Release()
        {
            IMalloc* ptr = (IMalloc*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IMalloc*, uint>)LpVtbl[2])(ptr);
        }

        public readonly unsafe void* Alloc(nuint cb)
        {
            IMalloc* ptr = (IMalloc*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IMalloc*, nuint, void*>)LpVtbl[3])(ptr, cb);
        }

        public readonly unsafe void* Realloc(void* pv, nuint cb)
        {
            IMalloc* ptr = (IMalloc*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IMalloc*, void*, nuint, void*>)LpVtbl[4])(ptr, pv, cb);
        }

        public readonly unsafe void Free(void* pv)
        {
            IMalloc* ptr = (IMalloc*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            ((delegate* unmanaged[Stdcall]<IMalloc*, void*, void>)LpVtbl[5])(ptr, pv);
        }

        public readonly unsafe nuint GetSize(void* pv)
        {
            IMalloc* ptr = (IMalloc*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IMalloc*, void*, nuint>)LpVtbl[6])(ptr, pv);
        }

        public readonly unsafe int DidAlloc(void* pv)
        {
            IMalloc* ptr = (IMalloc*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IMalloc*, void*, int>)LpVtbl[7])(ptr, pv);
        }

        public readonly unsafe void HeapMinimize()
        {
            IMalloc* ptr = (IMalloc*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            ((delegate* unmanaged[Stdcall]<IMalloc*, void>)LpVtbl[8])(ptr);
        }

        public static unsafe implicit operator IUnknown(IMalloc value)
        {
            return Unsafe.As<IMalloc, IUnknown>(ref value);
        }
    }
}