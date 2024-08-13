namespace DaxaTest
{
    using Hexa.NET.SDL2;
    using Hexa.NET.Daxa;
    using System.Runtime.InteropServices;
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

            InitGraphics(window);

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

        private static void InitGraphics(SDLWindow* window)
        {
            DaxaInstanceInfo instanceInfo = new();
            DaxaInstance instance;
            Daxa.DaxaCreateInstance(&instanceInfo, &instance).CheckError();

            DaxaDeviceInfo deviceInfo = new();
            deviceInfo.Flags = (uint)DaxaDeviceFlagBits.BufferDeviceAddressCaptureReplayBit;
            deviceInfo.Selector = (void*)Marshal.GetFunctionPointerForDelegate<Selector>(OnSelector);
            deviceInfo.Name = "my device";
            deviceInfo.MaxAllowedImages = 10000;
            deviceInfo.MaxAllowedSamplers = 400;
            deviceInfo.MaxAllowedBuffers = 10000;
            deviceInfo.MaxAllowedAccelerationStructures = 10000;
            DaxaDevice device;

            instance.CreateDevice(&deviceInfo, &device).CheckError();

            SDLSysWMInfo wmInfo;

            SDL.SDLGetVersion(&wmInfo.Version);
            SDL.SDLGetWindowWMInfo(window, &wmInfo);

            DaxaSwapchainInfo swapchainInfo;
            swapchainInfo.NativeWindow = wmInfo.Info.Win.Window;
            swapchainInfo.NativeWindowPlatform = DaxaNativeWindowPlatform.Win32Api;
            swapchainInfo.PresentOperation = VkSurfaceTransformFlagBitsKHR.InheritBitKhr;
            swapchainInfo.PresentMode = VkPresentModeKHR.MailboxKhr;
            swapchainInfo.MaxAllowedFramesInFlight = 2;
            swapchainInfo.ImageUsage = (uint)VkImageUsageFlagBits.TransferDstBit;
            swapchainInfo.SurfaceFormatSelector = (void*)Marshal.GetFunctionPointerForDelegate<SurfaceFormatSelector>(OnSwapchainSurfaceFormatSelector);

            DaxaSwapchain swapchain;
            device.DvcCreateSwapchain(&swapchainInfo, &swapchain).CheckError();
        }

        private static int OnSwapchainSurfaceFormatSelector(VkFormat format)
        {
            switch (format)
            {
                case VkFormat.B8G8R8A8Unorm:
                    return 100;

                case VkFormat.R8G8B8A8Unorm:
                    return 2;

                default:
                    return Daxa.DaxaDefaultFormatSelector(format);
            }
        }

        private static void Render()
        {
            throw new NotImplementedException();
        }

        private static unsafe int OnSelector(DaxaDeviceProperties* properties)
        {
            return Daxa.DaxaDefaultDeviceScore(properties);
        }
    }
}