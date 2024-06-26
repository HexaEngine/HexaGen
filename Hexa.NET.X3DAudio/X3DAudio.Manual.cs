namespace Hexa.NET.X3DAudio
{
    using HexaGen.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public unsafe partial class X3DAudio
    {
        /// <summary>
        /// --------------<br/>
        /// <F<br/>
        /// -U-N-C-T-I-O-N-S>-----------------------------------------//<br/>
        /// initializes instance handle<br/>
        /// </summary>
        [NativeName(NativeNameType.Func, "X3DAudioInitialize")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        [LibraryImport(LibName, EntryPoint = "X3DAudioInitialize")]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial HResult X3DAudioInitializeNative([NativeName(NativeNameType.Param, "SpeakerChannelMask")][NativeName(NativeNameType.Type, "UINT32")] uint speakerChannelMask, [NativeName(NativeNameType.Param, "SpeedOfSound")][NativeName(NativeNameType.Type, "FLOAT32")] float speedOfSound, [NativeName(NativeNameType.Param, "Instance")][NativeName(NativeNameType.Type, "X3DAUDIO_HANDLE")] X3DAudioHandle* instance);

        /// <summary>
        /// --------------<br/>
        /// <F<br/>
        /// -U-N-C-T-I-O-N-S>-----------------------------------------//<br/>
        /// initializes instance handle<br/>
        /// </summary>
        [NativeName(NativeNameType.Func, "X3DAudioInitialize")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult X3DAudioInitialize([NativeName(NativeNameType.Param, "SpeakerChannelMask")][NativeName(NativeNameType.Type, "UINT32")] uint speakerChannelMask, [NativeName(NativeNameType.Param, "SpeedOfSound")][NativeName(NativeNameType.Type, "FLOAT32")] float speedOfSound, [NativeName(NativeNameType.Param, "Instance")][NativeName(NativeNameType.Type, "X3DAUDIO_HANDLE")] X3DAudioHandle* instance)
        {
            HResult ret = X3DAudioInitializeNative(speakerChannelMask, speedOfSound, instance);
            return ret;
        }

        /// <summary>
        /// --------------<br/>
        /// <F<br/>
        /// -U-N-C-T-I-O-N-S>-----------------------------------------//<br/>
        /// initializes instance handle<br/>
        /// </summary>
        [NativeName(NativeNameType.Func, "X3DAudioInitialize")]
        [return: NativeName(NativeNameType.Type, "HRESULT")]
        public static HResult X3DAudioInitialize([NativeName(NativeNameType.Param, "SpeakerChannelMask")][NativeName(NativeNameType.Type, "UINT32")] uint speakerChannelMask, [NativeName(NativeNameType.Param, "SpeedOfSound")][NativeName(NativeNameType.Type, "FLOAT32")] float speedOfSound, [NativeName(NativeNameType.Param, "Instance")][NativeName(NativeNameType.Type, "X3DAUDIO_HANDLE")] ref X3DAudioHandle instance)
        {
            HResult ret = X3DAudioInitializeNative(speakerChannelMask, speedOfSound, (X3DAudioHandle*)Unsafe.AsPointer(ref instance));
            return ret;
        }
    }
}