// See https://aka.ms/new-console-template for more information
using HexaEngine.SDL2;

unsafe
{
    SDL.SDLSetHint(SDL.SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
    SDL.SDLInit(SDL.SDL_INIT_EVENTS | SDL.SDL_INIT_VIDEO);

    var window = SDL.SDLCreateWindow("Test Window", 32, 32, 1280, 720, (uint)SDLWindowFlags.Resizable);
    var windowId = SDL.SDLGetWindowID(window);

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
                        if ((SDLWindowEventID)windowEvent.Event == SDLWindowEventID.WindoweventClose)
                        {
                            exiting = true;
                        }
                    }
                    break;
            }
        }
    }

    SDL.SDLQuit();
}