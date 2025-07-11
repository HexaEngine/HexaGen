namespace HexaGen.Runtime
{
    public interface IGLContext : INativeContext
    {
        nint Handle { get; }

        bool IsCurrent { get; }

        void MakeCurrent();

        void SwapBuffers();

        void SwapInterval(int interval);
    }
}