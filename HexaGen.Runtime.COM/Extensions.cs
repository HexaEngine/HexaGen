namespace HexaGen.Runtime.COM
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static unsafe class Extensions
    {
        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface(this ComPtr<IUnknown> comObj, [NativeName(NativeNameType.Param, "riid")][NativeName(NativeNameType.Type, "const IID&")] Guid* riid, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] void** ppvInterface)
        {
            IUnknown* handle = comObj.Handle;
            HResult ret = ((delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, riid, ppvInterface);
            return ret;
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface(this ComPtr<IUnknown> comObj, [NativeName(NativeNameType.Param, "riid")][NativeName(NativeNameType.Type, "const IID&")] ref Guid riid, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] void** ppvInterface)
        {
            IUnknown* handle = comObj.Handle;
            fixed (Guid* priid = &riid)
            {
                HResult ret = ((delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, (Guid*)priid, ppvInterface);
                return ret;
            }
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface<T>(this ComPtr<IUnknown> comObj, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] out ComPtr<T> ppvInterface) where T : unmanaged, IComObject, IComObject<T>
        {
            IUnknown* handle = comObj.Handle;
            ppvInterface = default;
            HResult ret = ((delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, (Guid*)(ComUtils.GuidPtrOf<T>()), (void**)ppvInterface.GetAddressOf());
            return ret;
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface<T>(this ComPtr<IUnknown> comObj, [NativeName(NativeNameType.Param, "riid")][NativeName(NativeNameType.Type, "const IID&")] ref Guid riid, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] out ComPtr<T> ppvInterface) where T : unmanaged, IComObject, IComObject<T>
        {
            IUnknown* handle = comObj.Handle;
            fixed (Guid* priid = &riid)
            {
                ppvInterface = default;
                HResult ret = ((delegate* unmanaged[Stdcall]<IUnknown*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, (Guid*)priid, (void**)ppvInterface.GetAddressOf());
                return ret;
            }
        }

        [NativeName(NativeNameType.Func, "AddRef")]
        [return: NativeName(NativeNameType.Type, "ULONG")]
        public static uint AddRef(this ComPtr<IUnknown> comObj)
        {
            IUnknown* handle = comObj.Handle;
            uint ret = ((delegate* unmanaged[Stdcall]<IUnknown*, uint>)(handle->LpVtbl[1]))(handle);
            return ret;
        }

        [NativeName(NativeNameType.Func, "Release")]
        [return: NativeName(NativeNameType.Type, "ULONG")]
        public static uint Release(this ComPtr<IUnknown> comObj)
        {
            IUnknown* handle = comObj.Handle;
            uint ret = ((delegate* unmanaged[Stdcall]<IUnknown*, uint>)(handle->LpVtbl[2]))(handle);
            return ret;
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface(this ComPtr<IMalloc> comObj, [NativeName(NativeNameType.Param, "riid")][NativeName(NativeNameType.Type, "const IID&")] Guid* riid, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] void** ppvInterface)
        {
            IMalloc* handle = comObj.Handle;
            HResult ret = ((delegate* unmanaged[Stdcall]<IMalloc*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, riid, ppvInterface);
            return ret;
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface(this ComPtr<IMalloc> comObj, [NativeName(NativeNameType.Param, "riid")][NativeName(NativeNameType.Type, "const IID&")] ref Guid riid, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] void** ppvInterface)
        {
            IMalloc* handle = comObj.Handle;
            fixed (Guid* priid = &riid)
            {
                HResult ret = ((delegate* unmanaged[Stdcall]<IMalloc*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, (Guid*)priid, ppvInterface);
                return ret;
            }
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface<T>(this ComPtr<IMalloc> comObj, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] out ComPtr<T> ppvInterface) where T : unmanaged, IComObject, IComObject<T>
        {
            IMalloc* handle = comObj.Handle;
            ppvInterface = default;
            HResult ret = ((delegate* unmanaged[Stdcall]<IMalloc*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, (Guid*)(ComUtils.GuidPtrOf<T>()), (void**)ppvInterface.GetAddressOf());
            return ret;
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface<T>(this ComPtr<IMalloc> comObj, [NativeName(NativeNameType.Param, "riid")][NativeName(NativeNameType.Type, "const IID&")] ref Guid riid, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] out ComPtr<T> ppvInterface) where T : unmanaged, IComObject, IComObject<T>
        {
            IMalloc* handle = comObj.Handle;
            fixed (Guid* priid = &riid)
            {
                ppvInterface = default;
                HResult ret = ((delegate* unmanaged[Stdcall]<IMalloc*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, (Guid*)priid, (void**)ppvInterface.GetAddressOf());
                return ret;
            }
        }

        [NativeName(NativeNameType.Func, "AddRef")]
        [return: NativeName(NativeNameType.Type, "ULONG")]
        public static uint AddRef(this ComPtr<IMalloc> comObj)
        {
            IMalloc* handle = comObj.Handle;
            uint ret = ((delegate* unmanaged[Stdcall]<IMalloc*, uint>)(handle->LpVtbl[1]))(handle);
            return ret;
        }

        [NativeName(NativeNameType.Func, "Release")]
        [return: NativeName(NativeNameType.Type, "ULONG")]
        public static uint Release(this ComPtr<IMalloc> comObj)
        {
            IMalloc* handle = comObj.Handle;
            uint ret = ((delegate* unmanaged[Stdcall]<IMalloc*, uint>)(handle->LpVtbl[2]))(handle);
            return ret;
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface(this ComPtr<ISequentialStream> comObj, [NativeName(NativeNameType.Param, "riid")][NativeName(NativeNameType.Type, "const IID&")] Guid* riid, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] void** ppvInterface)
        {
            ISequentialStream* handle = comObj.Handle;
            HResult ret = ((delegate* unmanaged[Stdcall]<ISequentialStream*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, riid, ppvInterface);
            return ret;
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface(this ComPtr<ISequentialStream> comObj, [NativeName(NativeNameType.Param, "riid")][NativeName(NativeNameType.Type, "const IID&")] ref Guid riid, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] void** ppvInterface)
        {
            ISequentialStream* handle = comObj.Handle;
            fixed (Guid* priid = &riid)
            {
                HResult ret = ((delegate* unmanaged[Stdcall]<ISequentialStream*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, (Guid*)priid, ppvInterface);
                return ret;
            }
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface<T>(this ComPtr<ISequentialStream> comObj, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] out ComPtr<T> ppvInterface) where T : unmanaged, IComObject, IComObject<T>
        {
            ISequentialStream* handle = comObj.Handle;
            ppvInterface = default;
            HResult ret = ((delegate* unmanaged[Stdcall]<ISequentialStream*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, (Guid*)(ComUtils.GuidPtrOf<T>()), (void**)ppvInterface.GetAddressOf());
            return ret;
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface<T>(this ComPtr<ISequentialStream> comObj, [NativeName(NativeNameType.Param, "riid")][NativeName(NativeNameType.Type, "const IID&")] ref Guid riid, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] out ComPtr<T> ppvInterface) where T : unmanaged, IComObject, IComObject<T>
        {
            ISequentialStream* handle = comObj.Handle;
            fixed (Guid* priid = &riid)
            {
                ppvInterface = default;
                HResult ret = ((delegate* unmanaged[Stdcall]<ISequentialStream*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, (Guid*)priid, (void**)ppvInterface.GetAddressOf());
                return ret;
            }
        }

        [NativeName(NativeNameType.Func, "AddRef")]
        [return: NativeName(NativeNameType.Type, "ULONG")]
        public static uint AddRef(this ComPtr<ISequentialStream> comObj)
        {
            ISequentialStream* handle = comObj.Handle;
            uint ret = ((delegate* unmanaged[Stdcall]<ISequentialStream*, uint>)(handle->LpVtbl[1]))(handle);
            return ret;
        }

        [NativeName(NativeNameType.Func, "Release")]
        [return: NativeName(NativeNameType.Type, "ULONG")]
        public static uint Release(this ComPtr<ISequentialStream> comObj)
        {
            ISequentialStream* handle = comObj.Handle;
            uint ret = ((delegate* unmanaged[Stdcall]<ISequentialStream*, uint>)(handle->LpVtbl[2]))(handle);
            return ret;
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface(this ComPtr<IStream> comObj, [NativeName(NativeNameType.Param, "riid")][NativeName(NativeNameType.Type, "const IID&")] Guid* riid, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] void** ppvInterface)
        {
            IStream* handle = comObj.Handle;
            HResult ret = ((delegate* unmanaged[Stdcall]<IStream*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, riid, ppvInterface);
            return ret;
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface(this ComPtr<IStream> comObj, [NativeName(NativeNameType.Param, "riid")][NativeName(NativeNameType.Type, "const IID&")] ref Guid riid, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] void** ppvInterface)
        {
            IStream* handle = comObj.Handle;
            fixed (Guid* priid = &riid)
            {
                HResult ret = ((delegate* unmanaged[Stdcall]<IStream*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, (Guid*)priid, ppvInterface);
                return ret;
            }
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface<T>(this ComPtr<IStream> comObj, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] out ComPtr<T> ppvInterface) where T : unmanaged, IComObject, IComObject<T>
        {
            IStream* handle = comObj.Handle;
            ppvInterface = default;
            HResult ret = ((delegate* unmanaged[Stdcall]<IStream*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, (Guid*)(ComUtils.GuidPtrOf<T>()), (void**)ppvInterface.GetAddressOf());
            return ret;
        }

        [NativeName(NativeNameType.Func, "QueryInterface")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult QueryInterface<T>(this ComPtr<IStream> comObj, [NativeName(NativeNameType.Param, "riid")][NativeName(NativeNameType.Type, "const IID&")] ref Guid riid, [NativeName(NativeNameType.Param, "ppvInterface")][NativeName(NativeNameType.Type, "void**")] out ComPtr<T> ppvInterface) where T : unmanaged, IComObject, IComObject<T>
        {
            IStream* handle = comObj.Handle;
            fixed (Guid* priid = &riid)
            {
                ppvInterface = default;
                HResult ret = ((delegate* unmanaged[Stdcall]<IStream*, Guid*, void**, HResult>)(*handle->LpVtbl))(handle, (Guid*)priid, (void**)ppvInterface.GetAddressOf());
                return ret;
            }
        }

        [NativeName(NativeNameType.Func, "AddRef")]
        [return: NativeName(NativeNameType.Type, "ULONG")]
        public static uint AddRef(this ComPtr<IStream> comObj)
        {
            IStream* handle = comObj.Handle;
            uint ret = ((delegate* unmanaged[Stdcall]<IStream*, uint>)(handle->LpVtbl[1]))(handle);
            return ret;
        }

        [NativeName(NativeNameType.Func, "Release")]
        [return: NativeName(NativeNameType.Type, "ULONG")]
        public static uint Release(this ComPtr<IStream> comObj)
        {
            IStream* handle = comObj.Handle;
            uint ret = ((delegate* unmanaged[Stdcall]<IStream*, uint>)(handle->LpVtbl[2]))(handle);
            return ret;
        }
    }
}