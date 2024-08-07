namespace D3D11Test
{
    using Hexa.NET.SDL2;

    public static class Application
    {
        private static SDLWindow mainWindow;
        private static readonly Dictionary<uint, SDLWindow> idToWindow = new();
        private static DXGIAdapter adapter;
        private static D3D11DeviceManager deviceManager;
        private static IApp app;

        public static SDLWindow MainWindow => mainWindow;

        public static DXGIAdapter Adapter => adapter;

        public static D3D11DeviceManager DeviceManager => deviceManager;

        public static IApp App => app;

        public static void Run(SDLWindow window)
        {
            mainWindow = window;
            idToWindow.Add(window.Id, window);

            PlatformInit();

            PlatformRun();
        }

        private static void PlatformInit()
        {
            Console.WriteLine("SDL2 Init");

            SDL.SDLSetHint(SDL.SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
            SDL.SDLInit(SDL.SDL_INIT_EVENTS | SDL.SDL_INIT_VIDEO);

            Console.WriteLine("SDL2 Init ... Done");

            adapter = new(true);
            deviceManager = new(adapter, true);

            app = new D3D11App();
        }

        private static void PlatformRun()
        {
            SDLEvent sdlEvent = default;
            bool exiting = false;

            app.Init(mainWindow, deviceManager, adapter);

            Console.WriteLine("Entering Message Loop");

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
                                if ((SDLWindowEventID)windowEvent.Event == SDLWindowEventID.Close)
                                {
                                    exiting = true;
                                }
                            }
                            if (idToWindow.TryGetValue(windowEvent.WindowID, out var window))
                            {
                                window.ProcessEvent(windowEvent);
                            }
                            break;
                    }
                }

                app.Render();
            }

            Console.WriteLine("Exiting Message Loop");

            Console.WriteLine("Cleanup");
            app.Dispose();
            deviceManager.Dispose();
            adapter.Dispose();

            Console.WriteLine("Exit");
            SDL.SDLQuit();

            SDL.FreeApi();
        }
    }
}