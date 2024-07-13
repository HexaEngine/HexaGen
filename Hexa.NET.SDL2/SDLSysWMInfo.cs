namespace Hexa.NET.SDL2
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SDLSysWMInfo
    {
        public SDLVersion Version;
        public SdlSyswmType Subsystem;
        public SysWMInfoUnion Info;

        [StructLayout(LayoutKind.Explicit)]
        public struct SysWMInfoUnion
        {
            [FieldOffset(0)] public WinInfo Win;
            [FieldOffset(0)] public WinRTInfo WinRT;
            [FieldOffset(0)] public X11Info X11;
            [FieldOffset(0)] public DFBInfo DFB;
            [FieldOffset(0)] public CocoaInfo Cocoa;
            [FieldOffset(0)] public UIKitInfo UIKit;
            [FieldOffset(0)] public WaylandInfo Wayland;
            [FieldOffset(0)] public MirInfo Mir;
            [FieldOffset(0)] public AndroidInfo Android;
            [FieldOffset(0)] public OS2Info OS2;
            [FieldOffset(0)] public VivanteInfo Vivante;
            [FieldOffset(0)] public KMSDRMInfo KMSDRM;
            [FieldOffset(0)] public unsafe fixed byte Dummy[64];

            [StructLayout(LayoutKind.Sequential)]
            public struct WinInfo
            {
                public nint Window;       // HWND
                public nint Hdc;          // HDC
                public nint HInstance;    // HINSTANCE
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct WinRTInfo
            {
                public nint Window;       // IInspectable*
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct X11Info
            {
                public nint Display;      // Display*
                public nint Window;       // Window
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct DFBInfo
            {
                public nint Dfb;          // IDirectFB*
                public nint Window;       // IDirectFBWindow*
                public nint Surface;      // IDirectFBSurface*
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct CocoaInfo
            {
                public nint Window;       // NSWindow*
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct UIKitInfo
            {
                public nint Window;       // UIWindow*
                public uint Framebuffer;  // GLuint
                public uint Colorbuffer;  // GLuint
                public uint ResolveFramebuffer; // GLuint
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct WaylandInfo
            {
                public nint Display;       // struct wl_display*
                public nint Surface;       // struct wl_surface*
                public nint ShellSurface; // void*
                public nint EglWindow;    // struct wl_egl_window*
                public nint XdgSurface;   // struct xdg_surface*
                public nint XdgToplevel;  // struct xdg_toplevel*
                public nint XdgPopup;     // struct xdg_popup*
                public nint XdgPositioner; // struct xdg_positioner*
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MirInfo
            {
                public nint Connection;    // void*
                public nint Surface;       // void*
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct AndroidInfo
            {
                public nint Window;        // ANativeWindow*
                public nint Surface;       // EGLSurface
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct OS2Info
            {
                public nint Hwnd;          // HWND
                public nint HwndFrame;     // HWND
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct VivanteInfo
            {
                public nint Display;       // EGLNativeDisplayType
                public nint Window;        // EGLNativeWindowType
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct KMSDRMInfo
            {
                public int DevIndex;      // Device index (ex: the X in /dev/dri/cardX)
                public int DrmFd;         // DRM FD (unavailable on Vulkan windows)
                public nint GbmDev;       // struct gbm_device* (unavailable on Vulkan windows)
            }
        }
    }
}