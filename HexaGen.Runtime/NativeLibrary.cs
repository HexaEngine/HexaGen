#if !NET5_0_OR_GREATER
namespace HexaGen.Runtime
{
    using System.Runtime.InteropServices;

    public unsafe class NativeLibrary
    {
        // Windows
        [DllImport("kernel32", EntryPoint = "LoadLibrary", SetLastError = true)]
        private static extern nint LoadLibraryNative(byte* lpFileName);

        [DllImport("kernel32", EntryPoint = "FreeLibrary", SetLastError = true)]
        private static extern bool FreeLibraryNative(nint hModule);

        [DllImport("kernel32", EntryPoint = "GetProcAddress", SetLastError = true)]
        private static extern nint GetProcAddressNative(nint hModule, byte* lpProcName);

        // Unix/Linux/Android
        [DllImport("libdl.so", EntryPoint = "dlopen")]
        private static extern nint DLOpenNative(byte* fileName, int flags);

        [DllImport("libdl.so", EntryPoint = "dlclose")]
        private static extern int DLCloseNative(nint handle);

        [DllImport("libdl.so", EntryPoint = "dlsym")]
        private static extern nint DLSymNative(nint handle, byte* name);

        private const int RTLD_NOW = 2;

        public static nint Load(string libraryPath)
        {
            byte* pLibraryPath = Utils.StringToUTF8Ptr(libraryPath);
            nint libraryHandle;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                libraryHandle = LoadLibraryNative(pLibraryPath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                libraryHandle = DLOpenNative(pLibraryPath, RTLD_NOW);
            }
            else
            {
                libraryHandle = DLOpenNative(pLibraryPath, RTLD_NOW);
            }
            Utils.Free(pLibraryPath);
            return libraryHandle;
        }

        public static bool Free(nint libraryHandle)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return FreeLibraryNative(libraryHandle);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return DLCloseNative(libraryHandle) == 0;
            }
            else
            {
                return DLCloseNative(libraryHandle) == 0;
            }
        }
        private static nint GetProcAddress(nint libraryHandle, byte* pFunctionName)
        {
            nint functionAddress;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                functionAddress = GetProcAddressNative(libraryHandle, pFunctionName);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                     RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                functionAddress = DLSymNative(libraryHandle, pFunctionName);
            }
            else
            {
                functionAddress = DLSymNative(libraryHandle, pFunctionName);
            }

            return functionAddress;
        }

        public static nint GetExport(nint libraryHandle, string functionName)
        {
            byte* pFunctionName = Utils.StringToUTF8Ptr(functionName);

            nint functionAddress = GetProcAddress(libraryHandle, pFunctionName);

            Utils.Free(pFunctionName);

            if (functionAddress == 0)
            {
                throw new EntryPointNotFoundException(functionName);
            }

            return functionAddress;
        }

        public static bool TryGetExport(nint libraryHandle, string functionName, out nint functionAddress)
        {
            byte* pFunctionName = Utils.StringToUTF8Ptr(functionName);

            functionAddress = GetProcAddress(libraryHandle, pFunctionName);

            Utils.Free(pFunctionName);

            if (functionAddress == 0)
            {
                return false;
            }

            return true;
        }
    }
}
#endif