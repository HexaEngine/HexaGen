namespace D3D11Test
{
    using Hexa.NET.SDL2;

    public class ResizedEventArgs : EventArgs
    {
        public ResizedEventArgs(int oldWidth, int oldHeight, int width, int height)
        {
            OldWidth = oldWidth;
            OldHeight = oldHeight;
            Width = width;
            Height = height;
        }

        public int OldWidth { get; internal set; }

        public int OldHeight { get; internal set; }

        public int Width { get; internal set; }

        public int Height { get; internal set; }
    }

    public unsafe class SDLWindow
    {
        private uint id;
        private Hexa.NET.SDL2.SDLWindow* window;

        private int x;
        private int y;
        private int width;
        private int height;

        public SDLWindow() : this("Untitled", 32, 32, 1280, 720)
        {
        }

        public SDLWindow(string title, int x, int y, int width, int height)
        {
            Console.WriteLine("SDL2 Window Init");
            window = SDL.SDLCreateWindow(title, x, y, width, height, (uint)SDLWindowFlags.Resizable);

            id = SDL.SDLGetWindowID(window);

            SDL.SDLGetWindowPosition(window, &x, &y);

            this.x = x;
            this.y = y;

            SDL.SDLGetWindowSize(window, &width, &height);

            this.width = width;
            this.height = height;

            Console.WriteLine("SDL2 Window Init ... Done");
        }

        public uint Id => id;

        public int Width => width;

        public int Height => height;

        public int X => x;

        public int Y => y;

        public event Action<ResizedEventArgs>? Resized;

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

        internal void ProcessEvent(SDLWindowEvent windowEvent)
        {
            var type = (SDLWindowEventID)windowEvent.Event;
            Console.WriteLine($"{type}");
            switch (type)
            {
                case SDLWindowEventID.WindoweventSizeChanged:
                    Resized?.Invoke(new(width, height, windowEvent.Data1, windowEvent.Data2));
                    width = windowEvent.Data1;
                    height = windowEvent.Data2;
                    break;
            }
        }
    }
}