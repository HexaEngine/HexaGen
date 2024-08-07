namespace HexaGen.Runtime.COM
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;

    public class ComObject : IDisposable
    {
        private ComObject()
        {
        }

#if NET5_0_OR_GREATER

        [SupportedOSPlatform("windows")]
        public unsafe ComObject(object o)
        {
            Handle = (IUnknown*)(void*)Marshal.GetIUnknownForObject(o);
        }

        [SupportedOSPlatform("windows")]
        public unsafe object Value => Marshal.GetObjectForIUnknown((nint)Handle);

        [SupportedOSPlatform("windows")]
        public unsafe object UniqueValue => Marshal.GetUniqueObjectForIUnknown((nint)Handle);

#else

        public unsafe ComObject(object o)
        {
            Handle = (IUnknown*)(void*)Marshal.GetIUnknownForObject(o);
        }

        public unsafe object Value => Marshal.GetObjectForIUnknown((nint)Handle);

        public unsafe object UniqueValue => Marshal.GetUniqueObjectForIUnknown((nint)Handle);

#endif
        public unsafe IUnknown* Handle { get; set; }

        public static unsafe ComObject? FromPtr(IUnknown* ptr)
        {
            if (ptr != null)
            {
                return new ComObject
                {
                    Handle = ptr
                };
            }

            return null;
        }

        public unsafe int QueryInterface(ref Guid riid, out ComObject? comObject)
        {
            void* ppvObject = null;
            int result = Handle->QueryInterface(ref riid, ref ppvObject);
            comObject = FromPtr((IUnknown*)ppvObject);
            return result;
        }

        public unsafe uint AddRef()
        {
            return Handle->AddRef();
        }

        public unsafe uint Release()
        {
            return Handle->Release();
        }

        private unsafe void ReleaseInternal()
        {
            if (Handle != null)
            {
                Handle->Release();
                Handle = null;
            }
        }

        ~ComObject()
        {
            ReleaseInternal();
            Dispose();
        }

        public void Dispose()
        {
            ReleaseInternal();
            Dispose();
            GC.SuppressFinalize(this);
        }
    }
}