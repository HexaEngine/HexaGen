// See https://aka.ms/new-console-template for more information
using HexaEngine.SDL2;

SDL2.SDLSetHint(SDL2.SDL_HINT_MOUSE_FOCUS_CLICKTHROUGH, "1");
SDL2.SDLInit(SDL2.SDL_INIT_EVENTS | SDL2.SDL_INIT_VIDEO);

var window = SDL2.SDLCreateWindow("Test Window", 32, 32, 1280, 720, (uint)(SDLWindowFlags.Resizable));
var windowId = SDL2.SDLGetWindowID(window);

SDLEvent sdlEvent = default;
bool exiting = false;
while (!exiting)
{
    SDL2.SDLPumpEvents();

    while ((SDLBool)SDL2.SDLPollEvent(ref sdlEvent) == SDLBool.True)
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

SDL2.SDLQuit();