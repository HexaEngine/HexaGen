namespace BgfxTest
{
    using Hexa.NET.Bgfx;
    using Hexa.NET.SDL2;
    using System;

    internal unsafe class Program
    {
        private static int width;
        private static int height;

        private static void Main(string[] args)
        {
            SDL.SDLSetHint(SDL.SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
            SDL.SDLInit(SDL.SDL_INIT_EVENTS | SDL.SDL_INIT_VIDEO);

            var window = SDL.SDLCreateWindow("Test Window", 32, 32, 1280, 720, (uint)SDLWindowFlags.Resizable);
            var windowId = SDL.SDLGetWindowID(window);

            int w, h;
            SDL.SDLGetWindowSize(window, &w, &h);
            width = w;
            height = h;

            Init(window, windowId);

            SDLEvent sdlEvent = default;
            bool exiting = false;
            while (!exiting)
            {
                SDL.SDLPumpEvents();

                while ((SDLBool)SDL.SDLPollEvent(ref sdlEvent) == SDLBool.True)
                {
                    switch ((SDLEventType)sdlEvent.Type)
                    {
                        case SDLEventType.Quit:
                            exiting = true;
                            break;

                        case SDLEventType.AppTerminating:
                            exiting = true;
                            break;

                        case SDLEventType.Windowevent:
                            var windowEvent = sdlEvent.Window;
                            if (windowEvent.WindowID == windowId)
                            {
                                if ((SDLWindowEventID)windowEvent.Event == SDLWindowEventID.Close)
                                {
                                    exiting = true;
                                }
                            }
                            break;
                    }
                }

                Render();
            }

            SDL.SDLQuit();
        }

        private static void Init(SDLWindow* window, uint windowId)
        {
            BgfxPlatformData platformData = default;

            SDLSysWMInfo wmInfo;

            SDL.SDLGetVersion(&wmInfo.Version);
            SDL.SDLGetWindowWMInfo(window, &wmInfo);

            platformData.Nwh = (void*)wmInfo.Info.Win.Window;
            platformData.Ndt = null;
            platformData.Type = BgfxNativeWindowHandleType.Default;

            int w, h;
            SDL.SDLGetWindowSize(window, &w, &h);

            BgfxInit init = default;
            init.Type = BgfxRendererType.Direct3D12;
            init.VendorId = 0;
            init.PlatformData = platformData;
            init.Resolution.Width = (uint)w;
            init.Resolution.Height = (uint)h;
            init.Resolution.Reset = 0x80;
            init.Resolution.Format = BgfxTextureFormat.Bgra8;
            init.Capabilities = ulong.MaxValue;
            init.Limits.MaxEncoders = 8;
            init.Limits.MinResourceCbSize = 64 << 10;
            init.Limits.TransientVbSize = 6 << 20;
            init.Limits.TransientIbSize = 2 << 20;

            if (!Bgfx.BgfxInit(ref init))
            {
                throw new Exception("Failed to init");
            }

            Bgfx.BgfxSetDebug(8);

            Bgfx.BgfxSetViewClear(0
            , 0x1 | 0x2
            , 0x303030ff
            , 1.0f
            , 0
            );
        }

        private static void Render()
        {
            Bgfx.BgfxSetViewRect(0, 0, 0, (ushort)(width), (ushort)(height));
            Bgfx.BgfxTouch(0);

            Bgfx.BgfxDbgTextClear(0, false);

            Bgfx.BgfxDbgTextPrintf(0, 1, 0x0f, "Color can be changed with ANSI \x1b[9;me\x1b[10;ms\x1b[11;mc\x1b[12;ma\x1b[13;mp\x1b[14;me\x1b[0m code too.");

            Bgfx.BgfxFrame(false);
        }
    }
}