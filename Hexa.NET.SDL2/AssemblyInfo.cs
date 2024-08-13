using HexaGen.Runtime;

#if NET7_0_OR_GREATER
[assembly: System.Runtime.CompilerServices.DisableRuntimeMarshalling]
#endif

[assembly: NativeLibrary("SDL2", TargetPlatform.Windows)]
[assembly: NativeLibrary("libSDL2-2.0", TargetPlatform.Linux)]
[assembly: NativeLibrary("libSDL2-2.0", TargetPlatform.Android)]
[assembly: NativeLibrary("libSDL2-2.0", TargetPlatform.OSX)]