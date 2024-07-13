namespace Hexa.NET.SDL2
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct SDLSysWMMsg
    {
        public SDLVersion Version;
        public SdlSyswmType Subsystem;
        public SysWMMsgUnion Msg;

        [StructLayout(LayoutKind.Explicit)]
        public struct SysWMMsgUnion
        {
            [FieldOffset(0)] public WinMsg Win;
            //[FieldOffset(0)] public X11Msg X11;
            //[FieldOffset(0)] public DFBMsg Dfb;
            [FieldOffset(0)] public CocoaMsg Cocoa;
            [FieldOffset(0)] public UIKitMsg UIKit;
            [FieldOffset(0)] public VivanteMsg Vivante;
            [FieldOffset(0)] public OS2Msg OS2;
            [FieldOffset(0)] public int Dummy;

            [StructLayout(LayoutKind.Sequential)]
            public struct WinMsg
            {
                public nint Hwnd;     // HWND
                public uint Msg;      // UINT
                public nint WParam;   // WPARAM
                public nint LParam;   // LPARAM
            }
            /*
            [StructLayout(LayoutKind.Sequential)]
            public struct X11Msg
            {
                public XEvent Event; // XEvent
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct DFBMsg
            {
                public DFBEvent Event; // DFBEvent
            }
            */
            [StructLayout(LayoutKind.Sequential)]
            public struct CocoaMsg
            {
                public int Dummy;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct UIKitMsg
            {
                public int Dummy;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct VivanteMsg
            {
                public int Dummy;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct OS2Msg
            {
                public int FFrame;  // BOOL
                public nint Hwnd;   // HWND
                public uint Msg;    // ULONG
                public nint Mp1;    // MPARAM
                public nint Mp2;    // MPARAM
            }
        }
    }
}