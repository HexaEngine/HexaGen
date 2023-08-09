namespace HexaGen.Runtime.COM
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [Guid("0c733a30-2a1c-11ce-ade5-00aa0044773d")]
    public struct ISequentialStream : IComObject, IComObject<ISequentialStream>, IComObject<IUnknown>
    {
        public static readonly Guid Guid = new("0c733a30-2a1c-11ce-ade5-00aa0044773d");

        public unsafe void** LpVtbl;

        unsafe void*** IComObject.AsVtblPtr()
        {
            return (void***)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
        }

        public readonly unsafe HResult QueryInterface(Guid* riid, void** ppvObject)
        {
            ISequentialStream* ptr = (ISequentialStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<ISequentialStream*, Guid*, void**, int>)(*LpVtbl))(ptr, riid, ppvObject);
        }

        public readonly unsafe int QueryInterface(Guid* riid, ref void* ppvObject)
        {
            ISequentialStream* ptr = (ISequentialStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            fixed (void** ptr2 = &ppvObject)
            {
                return ((delegate* unmanaged[Stdcall]<ISequentialStream*, Guid*, void**, int>)(*LpVtbl))(ptr, riid, ptr2);
            }
        }

        public readonly unsafe int QueryInterface(ref Guid riid, void** ppvObject)
        {
            ISequentialStream* ptr = (ISequentialStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            fixed (Guid* ptr2 = &riid)
            {
                return ((delegate* unmanaged[Stdcall]<ISequentialStream*, Guid*, void**, int>)(*LpVtbl))(ptr, ptr2, ppvObject);
            }
        }

        public readonly unsafe int QueryInterface(ref Guid riid, ref void* ppvObject)
        {
            ISequentialStream* ptr = (ISequentialStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            fixed (Guid* ptr2 = &riid)
            {
                fixed (void** ptr3 = &ppvObject)
                {
                    return ((delegate* unmanaged[Stdcall]<ISequentialStream*, Guid*, void**, int>)(*LpVtbl))(ptr, ptr2, ptr3);
                }
            }
        }

        public readonly unsafe uint AddRef()
        {
            ISequentialStream* ptr = (ISequentialStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<ISequentialStream*, uint>)LpVtbl[1])(ptr);
        }

        public readonly unsafe uint Release()
        {
            ISequentialStream* ptr = (ISequentialStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<ISequentialStream*, uint>)LpVtbl[2])(ptr);
        }

        public readonly unsafe HResult Read(void* pv, uint cb, uint* pcbRead)
        {
            ISequentialStream* ptr = (ISequentialStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<ISequentialStream*, void*, uint, uint*, HResult>)LpVtbl[3])(ptr, pv, cb, pcbRead);
        }

        public readonly unsafe HResult Write(void* pv, uint cb, uint* pcbRead)
        {
            ISequentialStream* ptr = (ISequentialStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<ISequentialStream*, void*, uint, uint*, HResult>)LpVtbl[4])(ptr, pv, cb, pcbRead);
        }

        public static unsafe implicit operator IUnknown(ISequentialStream value)
        {
            return Unsafe.As<ISequentialStream, IUnknown>(ref value);
        }
    }
}