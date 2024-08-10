namespace HexaGen.Runtime
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

#if !NET5_0_OR_GREATER
    using HexaGen.Runtime;
#endif

    public static class LibraryLoader
    {
        public static nint LoadLibrary()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LoadLocalLibrary("bgfx");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return LoadLocalLibrary("bgfx");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LoadLocalLibrary("bgfx");
            }
            else
            {
                return LoadLocalLibrary("bgfx");
            }
        }

        public static string GetExtension()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return ".dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return ".dylib";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return ".so";
            }

            return ".so";
        }

        public static nint LoadLocalLibrary(string libraryName)
        {
            var extension = GetExtension();

            if (!libraryName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                libraryName += extension;
            }

            var osPlatform = GetOSPlatform();
            var architecture = GetArchitecture();

            var libraryPath = GetNativeAssemblyPath(osPlatform, architecture, libraryName);

            static string GetOSPlatform()
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return "win";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return "linux";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    return "osx";
                }

                throw new ArgumentException("Unsupported OS platform.");
            }

            static string GetArchitecture()
            {
                return RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X86 => "x86",
                    Architecture.X64 => "x64",
                    Architecture.Arm => "arm",
                    Architecture.Arm64 => "arm64",
                    _ => throw new ArgumentException("Unsupported architecture."),
                };
            }

            var attribute = Assembly.GetCallingAssembly().GetCustomAttribute<NativeLibraryAttribute>();
            var methods = typeof(string).GetMethods();

            static string GetNativeAssemblyPath(string osPlatform, string architecture, string libraryName)
            {
                var assemblyLocation = AppContext.BaseDirectory;

                var paths = new[]
                {
                    Path.Combine(assemblyLocation, libraryName),
                    Path.Combine(assemblyLocation, "runtimes", osPlatform, "native", libraryName),
                    Path.Combine(assemblyLocation, "runtimes", $"{osPlatform}-{architecture}", "native", libraryName),
                };

                foreach (var path in paths)
                {
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }

                return libraryName;
            }

            nint handle;

            handle = NativeLibrary.Load(libraryPath);

            if (handle == IntPtr.Zero)
            {
                throw new DllNotFoundException($"Unable to load library '{libraryName}'.");
            }

            return handle;
        }
    }

    [System.AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class NativeLibraryExtensionAttribute : Attribute
    {
        private readonly string extension;
        private readonly OSPlatform targetPlatform;


        public NativeLibraryExtensionAttribute(string extension, OSPlatform targetPlatform)
        {
            this.extension = extension;
            this.targetPlatform = targetPlatform;
        }

        public string Extension => extension;

        public OSPlatform TargetPlatform => targetPlatform;
    }

    [System.AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class NativeLibraryAttribute : Attribute
    {
        private readonly string libraryName;
        private readonly OSPlatform targetPlatform;

        public NativeLibraryAttribute(string libraryName, OSPlatform targetPlatform)
        {
            this.libraryName = libraryName;
            this.TargetPlatform = targetPlatform;
        }

        public string LibraryName => libraryName;

        public OSPlatform TargetPlatform => targetPlatform;
    }

    public enum TargetPlatform
    {
        Windows,
        Linux,
        OSX,
        Android,
    }
}