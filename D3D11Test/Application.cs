namespace D3D11Test
{
    using Hexa.NET.SDL2;

    public static class Application
    {
        private static SDLWindow mainWindow;
        private static DXGIAdapter adapter;
        private static D3D11DeviceManager deviceManager;

        public static SDLWindow MainWindow => mainWindow;

        public static DXGIAdapter Adapter => adapter;

        public static D3D11DeviceManager DeviceManager => deviceManager;

        public static void Run(SDLWindow window)
        {
            mainWindow = window;

            PlatformInit();

            PlatformRun();
        }

        private static void PlatformInit()
        {
            SDL.SDLSetHint(SDL.SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
            SDL.SDLInit(SDL.SDL_INIT_EVENTS | SDL.SDL_INIT_VIDEO);

            adapter = new(true);
            deviceManager = new(adapter, true);
        }

        private static void PlatformRun()
        {
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
                            if (windowEvent.WindowID == mainWindow.Id)
                            {
                                if ((SDLWindowEventID)windowEvent.Event == SDLWindowEventID.WindoweventClose)
                                {
                                    exiting = true;
                                }
                            }
                            break;
                    }
                }
            }

            deviceManager.Dispose();
            adapter.Dispose();

            SDL.SDLQuit();
        }
    }
}