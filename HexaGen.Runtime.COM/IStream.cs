namespace HexaGen.Runtime.COM
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [Guid("0000000c-0000-0000-C000-000000000046")]
    public struct IStream : IComObject, IComObject<IStream>, IComObject<ISequentialStream>, IComObject<IUnknown>
    {
        public static readonly Guid Guid = new("0000000c-0000-0000-C000-000000000046");

        public unsafe void** LpVtbl;

        unsafe void*** IComObject.AsVtblPtr()
        {
            return (void***)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
        }

        public readonly unsafe HResult QueryInterface(Guid* riid, void** ppvObject)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, Guid*, void**, int>)(*LpVtbl))(ptr, riid, ppvObject);
        }

        public readonly unsafe int QueryInterface(Guid* riid, ref void* ppvObject)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            fixed (void** ptr2 = &ppvObject)
            {
                return ((delegate* unmanaged[Stdcall]<IStream*, Guid*, void**, int>)(*LpVtbl))(ptr, riid, ptr2);
            }
        }

        public readonly unsafe int QueryInterface(ref Guid riid, void** ppvObject)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            fixed (Guid* ptr2 = &riid)
            {
                return ((delegate* unmanaged[Stdcall]<IStream*, Guid*, void**, int>)(*LpVtbl))(ptr, ptr2, ppvObject);
            }
        }

        public readonly unsafe int QueryInterface(ref Guid riid, ref void* ppvObject)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            fixed (Guid* ptr2 = &riid)
            {
                fixed (void** ptr3 = &ppvObject)
                {
                    return ((delegate* unmanaged[Stdcall]<IStream*, Guid*, void**, int>)(*LpVtbl))(ptr, ptr2, ptr3);
                }
            }
        }

        public readonly unsafe uint AddRef()
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, uint>)LpVtbl[1])(ptr);
        }

        public readonly unsafe uint Release()
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, uint>)LpVtbl[2])(ptr);
        }

        public readonly unsafe HResult Read(void* pv, uint cb, uint* pcbRead)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, void*, uint, uint*, HResult>)LpVtbl[3])(ptr, pv, cb, pcbRead);
        }

        public readonly unsafe HResult Write(void* pv, uint cb, uint* pcbRead)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, void*, uint, uint*, HResult>)LpVtbl[4])(ptr, pv, cb, pcbRead);
        }

        public readonly unsafe HResult Seek(long dlibMove, uint dwOrigin, ulong* plibNewPosition)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, long, uint, ulong*, HResult>)LpVtbl[5])(ptr, dlibMove, dwOrigin, plibNewPosition);
        }

        public readonly unsafe HResult CopyTo(IStream* pstm, ulong cb, ulong* pcbRead, ulong* pcbWritten)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, IStream*, ulong, ulong*, ulong*, HResult>)LpVtbl[6])(ptr, pstm, cb, pcbRead, pcbWritten);
        }

        public readonly unsafe HResult Commit(uint grfCommitFlags)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, uint, HResult>)LpVtbl[7])(ptr, grfCommitFlags);
        }

        public readonly unsafe HResult Revert()
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, HResult>)LpVtbl[8])(ptr);
        }

        public readonly unsafe HResult LockRegion(ulong libOffset, ulong cb, uint dwLockType)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, ulong, ulong, uint, HResult>)LpVtbl[9])(ptr, libOffset, cb, dwLockType);
        }

        public readonly unsafe HResult UnlockRegion(ulong libOffset, ulong cb, uint dwLockType)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, ulong, ulong, uint, HResult>)LpVtbl[10])(ptr, libOffset, cb, dwLockType);
        }

        public readonly unsafe HResult Stat(void* pstatstg, uint grfStatFlag)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, void*, uint, HResult>)LpVtbl[11])(ptr, pstatstg, grfStatFlag);
        }

        public readonly unsafe HResult Clone(IStream** ppstm)
        {
            IStream* ptr = (IStream*)Unsafe.AsPointer(ref Unsafe.AsRef(in this));
            return ((delegate* unmanaged[Stdcall]<IStream*, IStream**, HResult>)LpVtbl[12])(ptr, ppstm);
        }

        public static unsafe implicit operator IUnknown(IStream value)
        {
            return Unsafe.As<IStream, IUnknown>(ref value);
        }

        public static unsafe implicit operator ISequentialStream(IStream value)
        {
            return Unsafe.As<IStream, ISequentialStream>(ref value);
        }
    }
}