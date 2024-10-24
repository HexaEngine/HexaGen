namespace HexaGen.Runtime
{
    using System;

    public interface INativeContext : IDisposable
    {
        nint GetProcAddress(string procName);

        bool TryGetProcAddress(string procName, out nint procAddress);

        bool IsExtensionSupported(string extensionName);
    }
}