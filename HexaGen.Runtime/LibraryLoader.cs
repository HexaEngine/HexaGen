﻿namespace HexaGen.Runtime
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

#if !NET5_0_OR_GREATER
    using HexaGen.Runtime;
#endif

#if ANDROID
    using Android.Content.PM;
#endif

    public enum TargetPlatform
    {
        Unknown = 0,
        FreeBSD = 1 << 0,
        Linux = 1 << 1,
        OSX = 1 << 2,
        Windows = 1 << 3,
        Android = 1 << 4,
        IOS = 1 << 5,
        Tizen = 1 << 6,
        ChromeOS = 1 << 7,
        WebAssembly = 1 << 8,
        Solaris = 1 << 9,
        WatchOS = 1 << 10,
        TVO = 1 << 11,
        Any = FreeBSD | Linux | OSX | Windows | Android | IOS | Tizen | ChromeOS | WebAssembly | Solaris | WatchOS | TVO
    }

    public struct LibraryName
    {
        public string Name;
        public TargetPlatform TargetPlatform;
    }

    public struct LibraryExtension
    {
        public string Extension;
        public TargetPlatform TargetPlatform;
    }

    public struct LibraryLoaderHints
    {
        public LibraryName[] Names;
        public LibraryExtension[] Extensions;
        public string[] Paths;

        public LibraryLoaderHints(LibraryName[] names, LibraryExtension[] extensions, string[] paths)
        {
            Names = names;
            Extensions = extensions;
            Paths = paths;
        }

        public LibraryLoaderHints(LibraryName[] names, LibraryExtension[] extensions)
        {
            Names = names;
            Extensions = extensions;
            Paths = [];
        }
    }

    public static class LibraryLoader
    {
        public static OSPlatform FreeBSD { get; } = OSPlatform.Create("FREEBSD");

        public static OSPlatform Linux { get; } = OSPlatform.Create("LINUX");

        public static OSPlatform OSX { get; } = OSPlatform.Create("OSX");

        public static OSPlatform Windows { get; } = OSPlatform.Create("WINDOWS");

        public static OSPlatform Android { get; } = OSPlatform.Create("ANDROID");

        public static OSPlatform IOS { get; } = OSPlatform.Create("IOS");

        public static OSPlatform Tizen { get; } = OSPlatform.Create("TIZEN");

        public static OSPlatform ChromeOS { get; } = OSPlatform.Create("CHROMEOS");

        public static OSPlatform WebAssembly { get; } = OSPlatform.Create("WEBASSEMBLY");

        public static OSPlatform Solaris { get; } = OSPlatform.Create("SOLARIS");

        public static OSPlatform WatchOS { get; } = OSPlatform.Create("WATCHOS");

        public static OSPlatform TVOS { get; } = OSPlatform.Create("TVOS");

        public static string GetExtension(IEnumerable<NativeLibraryExtensionAttribute> extensionAttributes)
        {
            foreach (var extensionAttribute in extensionAttributes)
            {
                if (RuntimeInformation.IsOSPlatform(extensionAttribute.TargetPlatform.Convert()))
                {
                    return extensionAttribute.Extension;
                }
            }

            // Default extension based on platform
            if (RuntimeInformation.IsOSPlatform(Windows))
            {
                return ".dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSX))
            {
                return ".dylib";
            }
            else if (RuntimeInformation.IsOSPlatform(Linux))
            {
                return ".so";
            }
            else if (RuntimeInformation.IsOSPlatform(Android))
            {
                return ".so";
            }
            else if (RuntimeInformation.IsOSPlatform(IOS))
            {
                return ".dylib"; // iOS also uses .dylib for dynamic libraries
            }
            else if (RuntimeInformation.IsOSPlatform(FreeBSD))
            {
                return ".so";
            }
            else if (RuntimeInformation.IsOSPlatform(TVOS))
            {
                return ".dylib"; // tvOS uses the same dynamic library extension as iOS
            }
            else if (RuntimeInformation.IsOSPlatform(WatchOS))
            {
                return ".dylib"; // watchOS also uses .dylib
            }
            else if (RuntimeInformation.IsOSPlatform(Solaris))
            {
                return ".so";
            }
            else if (RuntimeInformation.IsOSPlatform(WebAssembly))
            {
                return ".wasm";
            }
            else if (RuntimeInformation.IsOSPlatform(Tizen))
            {
                return ".so";
            }
            else if (RuntimeInformation.IsOSPlatform(ChromeOS))
            {
                return ".so";
            }

            // Default to .so if no platform matches
            return ".so";
        }

        public static nint LoadLibrary()
        {
            var libraryAttributes = Assembly.GetCallingAssembly().GetCustomAttributes<NativeLibraryAttribute>();
            var extensionAttributes = Assembly.GetCallingAssembly().GetCustomAttributes<NativeLibraryExtensionAttribute>();

            var libraryName = GetLibraryName(libraryAttributes);

            var extension = GetExtension(extensionAttributes);

            if (!libraryName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                libraryName += extension;
            }

            var osPlatform = GetOSPlatform();
            var architecture = GetArchitecture();
            var libraryPath = GetNativeAssemblyPath(osPlatform, architecture, libraryName);

            nint handle;

            handle = NativeLibrary.Load(libraryPath);

            if (handle == IntPtr.Zero)
            {
                throw new DllNotFoundException($"Unable to load library '{libraryName}'.");
            }

            return handle;
        }

        private static string GetNativeAssemblyPath(string osPlatform, string architecture, string libraryName)
        {
#if ANDROID
            // Get the application info
            ApplicationInfo appInfo = Application.Context.ApplicationInfo!;

            // Get the native library directory path
            string assemblyLocation = appInfo.NativeLibraryDir!;

#else
            string assemblyLocation = AppContext.BaseDirectory;
#endif

            var paths = new[]
            {
                    Path.Combine(assemblyLocation, libraryName),
                    Path.Combine(assemblyLocation, "runtimes", osPlatform, "native", libraryName),
                    Path.Combine(assemblyLocation, "runtimes", $"{osPlatform}-{architecture}", "debug", libraryName), // allows debug builds sideload.
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

        private static string GetArchitecture()
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

        private static string GetOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(Windows))
            {
                return "win";
            }
            else if (RuntimeInformation.IsOSPlatform(Linux))
            {
                return "linux";
            }
            else if (RuntimeInformation.IsOSPlatform(OSX))
            {
                return "osx";
            }
            else if (RuntimeInformation.IsOSPlatform(Android))
            {
                return "android";
            }
            else if (RuntimeInformation.IsOSPlatform(IOS))
            {
                return "ios";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("FREEBSD")))
            {
                return "freebsd";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("TVOS")))
            {
                return "tvos";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("WATCHOS")))
            {
                return "watchos";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("SOLARIS")))
            {
                return "solaris";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY")))
            {
                return "webassembly";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("TIZEN")))
            {
                return "tizen";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Create("CHROMEOS")))
            {
                return "chromeos";
            }

            throw new ArgumentException("Unsupported OS platform.");
        }

        private static string GetLibraryName(IEnumerable<NativeLibraryAttribute> nativeLibraries)
        {
            NativeLibraryAttribute? nativeLibrary = null;
            foreach (NativeLibraryAttribute attri in nativeLibraries)
            {
                if (attri.TargetPlatform == TargetPlatform.Any)
                {
                    nativeLibrary = attri; // Default
                    continue;
                }

                if (RuntimeInformation.IsOSPlatform(attri.TargetPlatform.Convert()))
                {
                    nativeLibrary = attri;
                    break;
                }
            }

            if (nativeLibrary == null)
            {
                throw new Exception("Dll not specified for this platform");
            }

            return nativeLibrary.LibraryName;
        }

        private static OSPlatform Convert(this TargetPlatform targetPlatform)
        {
            switch (targetPlatform)
            {
                case TargetPlatform.FreeBSD:
                    return FreeBSD;

                case TargetPlatform.Linux:
                    return Linux;

                case TargetPlatform.OSX:
                    return OSX;

                case TargetPlatform.Windows:
                    return Windows;

                case TargetPlatform.Android:
                    return Android;

                case TargetPlatform.IOS:
                    return IOS;

                case TargetPlatform.Tizen:
                    return Tizen;

                case TargetPlatform.ChromeOS:
                    return ChromeOS;

                case TargetPlatform.WebAssembly:
                    return WebAssembly;

                case TargetPlatform.Solaris:
                    return Solaris;

                case TargetPlatform.WatchOS:
                    return WatchOS;

                case TargetPlatform.TVO:
                    return TVOS;

                default:
                    throw new PlatformNotSupportedException();
            }
        }
    }

    [System.AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class NativeLibraryExtensionAttribute : Attribute
    {
        private readonly string extension;
        private readonly TargetPlatform targetPlatform;

        public NativeLibraryExtensionAttribute(string extension, TargetPlatform targetPlatform = TargetPlatform.Any)
        {
            this.extension = extension;
            this.targetPlatform = targetPlatform;
        }

        public string Extension => extension;

        public TargetPlatform TargetPlatform => targetPlatform;
    }

    [System.AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public sealed class NativeLibraryAttribute : Attribute
    {
        private readonly string libraryName;
        private readonly TargetPlatform targetPlatform;

        public NativeLibraryAttribute(string libraryName, TargetPlatform targetPlatform = TargetPlatform.Any)
        {
            this.libraryName = libraryName;
            this.targetPlatform = targetPlatform;
        }

        public string LibraryName => libraryName;

        public TargetPlatform TargetPlatform => targetPlatform;
    }
}