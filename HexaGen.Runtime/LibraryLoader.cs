﻿namespace HexaGen.Runtime
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Runtime.CompilerServices;
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

    public delegate bool ResolvePathHandler(string libraryName, out string? pathToLibrary);

    public delegate bool LibraryNameInterceptor(ref string libraryName);

    public delegate bool LibraryLoadInterceptor(string libraryName, out nint pointer);

    public delegate bool NativeContextInterceptor(string libraryName, out INativeContext? context);

    public static class LibraryLoader
    {
        private static readonly EventHandlerList<ResolvePathHandler> resolvePathHandlers = [];
        private static readonly EventHandlerList<LibraryNameInterceptor> libraryNameInterceptors = [];
        private static readonly EventHandlerList<LibraryLoadInterceptor> libraryLoadInterceptors = [];
        private static readonly EventHandlerList<NativeContextInterceptor> nativeContextInterceptors = [];

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

        public static List<string> CustomLoadFolders { get; } = [];

        public static event ResolvePathHandler ResolvePath { add => resolvePathHandlers.Add(value); remove => resolvePathHandlers.Remove(value); }

        public static event LibraryNameInterceptor InterceptLibraryName { add => libraryNameInterceptors.Add(value); remove => libraryNameInterceptors.Remove(value); }

        public static event LibraryLoadInterceptor InterceptLibraryLoad { add => libraryLoadInterceptors.Add(value); remove => libraryLoadInterceptors.Remove(value); }

        public static event NativeContextInterceptor InterceptNativeContext { add => nativeContextInterceptors.Add(value); remove => nativeContextInterceptors.Remove(value); }

        public static string GetExtension()
        {
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

        public delegate string LibraryNameCallback();

        public delegate string LibraryExtensionCallback();

        public static void LoadFromMainModule(string targetLibraryName)
        {
            LoadFrom(targetLibraryName, Process.GetCurrentProcess().MainModule!.BaseAddress);
        }

        public static void LoadFrom(string targetLibraryName, nint address)
        {
            bool Callback(string libraryName, out nint pointer)
            {
                if (libraryName == targetLibraryName)
                {
                    pointer = address;
                    return true;
                }

                pointer = 0;
                return false;
            }

            libraryLoadInterceptors.Add(Callback);
        }

        public static INativeContext LoadLibraryEx(LibraryNameCallback libraryNameCallback, LibraryExtensionCallback? libraryExtensionCallback)
        {
            var libraryName = libraryNameCallback();

            foreach (var callback in nativeContextInterceptors)
            {
                if (callback(libraryName, out var context))
                {
                    return context ?? throw new InvalidOperationException("'context' cannot be null when returning true.");
                }
            }

            return new NativeLibraryContext(LoadLibrary(libraryNameCallback, libraryExtensionCallback));
        }

        public static nint LoadLibrary(LibraryNameCallback libraryNameCallback, LibraryExtensionCallback? libraryExtensionCallback, [CallerMemberName] string? name = "")
        {
            var libraryName = libraryNameCallback();

            foreach (var callback in libraryNameInterceptors)
            {
                if (callback(ref libraryName))
                {
                    break;
                }
            }

            foreach (var callback in libraryLoadInterceptors)
            {
                if (callback(libraryName, out nint pointer))
                {
                    return pointer;
                }
            }

            var extension = libraryExtensionCallback != null ? libraryExtensionCallback() : GetExtension();

            if (!libraryName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                libraryName += extension;
            }

            var osPlatform = GetOSPlatform();
            var architecture = GetArchitecture();
            var libraryPath = GetNativeAssemblyPath(osPlatform, architecture, libraryName);

            nint handle = NativeLibrary.Load(libraryPath);

            if (handle == IntPtr.Zero)
            {
                throw new DllNotFoundException($"Unable to load library '{libraryName}'.");
            }

            return handle;
        }

        private static string GetNativeAssemblyPath(string osPlatform, string architecture, string libraryName)
        {
            foreach (var callback in resolvePathHandlers)
            {
                if (callback(libraryName, out var pathToLibrary))
                {
                    return pathToLibrary ?? throw new InvalidOperationException("'pathToLibrary' cannot be null when returning true.");
                }
            }

#if ANDROID
            // Get the application info
            ApplicationInfo appInfo = Application.Context.ApplicationInfo!;

            // Get the native library directory path
            string assemblyLocation = appInfo.NativeLibraryDir!;

#else
            string assemblyLocation = AppContext.BaseDirectory;
#endif

            List<string> paths =
            [
                 Path.Combine(assemblyLocation, libraryName),
                 Path.Combine(assemblyLocation, "runtimes", osPlatform, "native", libraryName),
                 Path.Combine(assemblyLocation, "runtimes", $"{osPlatform}-{architecture}", "debug", libraryName), // allows debug builds sideload.
                 Path.Combine(assemblyLocation, "runtimes", $"{osPlatform}-{architecture}", "native", libraryName),
            ];

            foreach (var customPath in CustomLoadFolders)
            {
                if (IsPathFullyQualified(customPath))
                {
                    paths.Add(Path.Combine(customPath, libraryName));
                }
                else
                {
                    paths.Add(Path.Combine(assemblyLocation, customPath, libraryName));
                }
            }

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return libraryName;
        }

        public static bool IsPathFullyQualified(string path)
        {
            if (path.Length == 0)
            {
                return false;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return IsFullyQualifiedWindows(path);
            }

            return IsFullyQualifiedUnix(path);
        }

        private static bool IsFullyQualifiedWindows(string path)
        {
            if (path.Length < 2)
            {
                return false;
            }

            if (char.IsLetter(path[0]) && path[1] == ':' &&
                (path.Length > 2 && (path[2] == '\\' || path[2] == '/')))
            {
                return true;
            }

            if (path.Length > 1 && path[0] == '\\' && path[1] == '\\')
            {
                return true;
            }

            return false;
        }

        private static bool IsFullyQualifiedUnix(string path)
        {
            return path.Length > 0 && path[0] == '/';
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
    }
}