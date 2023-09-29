namespace D3D11Test
{
    using Hexa.NET.SDL2;

    public unsafe class SDLWindow
    {
        private uint id;
        private Hexa.NET.SDL2.SDLWindow* window;

        public SDLWindow() : this("Untitled", 1280, 720)
        {
        }

        public SDLWindow(string title, int width, int height)
        {
            window = SDL.SDLCreateWindow(title, 32, 32, width, height, (uint)SDLWindowFlags.Resizable);
            id = SDL.SDLGetWindowID(window);
        }

        public uint Id => id;

        public (nint Hwnd, nint HDC, nint HInstance)? Win32
        {
            get
            {
                SDLSysWMinfo wmInfo;

                SDL.SDLGetVersion(&wmInfo.Version);
                SDL.SDLGetWindowWMInfo(window, &wmInfo);

                return (wmInfo.Union.Win.Window, wmInfo.Union.Win.Hdc, wmInfo.Union.Win.Hinstance);
            }
        }
    }
}