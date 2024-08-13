using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HexaGen.Runtime;

namespace Hexa.NET.SDL2
{
    /// <summary>
    /// <br/>
    /// </summary>
    [NativeName(NativeNameType.StructOrClass, "SDL_AudioCVT")]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct SDLAudioCVT
    {
        /// <summary>
        /// Set to 1 if conversion possible <br/>
        /// </summary>
        [NativeName(NativeNameType.Field, "needed")]
        [NativeName(NativeNameType.Type, "int")]
        public int Needed;

        /// <summary>
        /// Source audio format <br/>
        /// </summary>
        [NativeName(NativeNameType.Field, "src_format")]
        [NativeName(NativeNameType.Type, "SDL_AudioFormat")]
        public ushort SrcFormat;

        /// <summary>
        /// Target audio format <br/>
        /// </summary>
        [NativeName(NativeNameType.Field, "dst_format")]
        [NativeName(NativeNameType.Type, "SDL_AudioFormat")]
        public ushort DstFormat;

        /// <summary>
        /// Rate conversion increment <br/>
        /// </summary>
        [NativeName(NativeNameType.Field, "rate_incr")]
        [NativeName(NativeNameType.Type, "double")]
        public double RateIncr;

        /// <summary>
        /// Buffer to hold entire audio data <br/>
        /// </summary>
        [NativeName(NativeNameType.Field, "buf")]
        [NativeName(NativeNameType.Type, "Uint8*")]
        public unsafe byte* Buf;

        /// <summary>
        /// Length of original audio buffer <br/>
        /// </summary>
        [NativeName(NativeNameType.Field, "len")]
        [NativeName(NativeNameType.Type, "int")]
        public int Len;

        /// <summary>
        /// Length of converted audio buffer <br/>
        /// </summary>
        [NativeName(NativeNameType.Field, "len_cvt")]
        [NativeName(NativeNameType.Type, "int")]
        public int LenCvt;

        /// <summary>
        /// buffer must be len*len_mult big <br/>
        /// </summary>
        [NativeName(NativeNameType.Field, "len_mult")]
        [NativeName(NativeNameType.Type, "int")]
        public int LenMult;

        /// <summary>
        /// Given len, final size is len*len_ratio <br/>
        /// </summary>
        [NativeName(NativeNameType.Field, "len_ratio")]
        [NativeName(NativeNameType.Type, "double")]
        public double LenRatio;

        /// <summary>
        /// NULL-terminated list of filter functions <br/>
        /// </summary>
        [NativeName(NativeNameType.Field, "filters")]
        [NativeName(NativeNameType.Type, "SDL_AudioFilter[10]")]
        public void* Filters_0;

        public void* Filters_1;
        public void* Filters_2;
        public void* Filters_3;
        public void* Filters_4;
        public void* Filters_5;
        public void* Filters_6;
        public void* Filters_7;
        public void* Filters_8;
        public void* Filters_9;

        /// <summary>
        /// Current audio conversion function <br/>
        /// </summary>
        [NativeName(NativeNameType.Field, "filter_index")]
        [NativeName(NativeNameType.Type, "int")]
        public int FilterIndex;

        public unsafe SDLAudioCVT(int needed = default, ushort srcFormat = default, ushort dstFormat = default, double rateIncr = default, byte* buf = default, int len = default, int lenCvt = default, int lenMult = default, double lenRatio = default, void** filters = default, int filterIndex = default)
        {
            Needed = needed;
            SrcFormat = srcFormat;
            DstFormat = dstFormat;
            RateIncr = rateIncr;
            Buf = buf;
            Len = len;
            LenCvt = lenCvt;
            LenMult = lenMult;
            LenRatio = lenRatio;
            if (filters != default)
            {
                Filters_0 = filters[0];
                Filters_1 = filters[1];
                Filters_2 = filters[2];
                Filters_3 = filters[3];
                Filters_4 = filters[4];
                Filters_5 = filters[5];
                Filters_6 = filters[6];
                Filters_7 = filters[7];
                Filters_8 = filters[8];
                Filters_9 = filters[9];
            }
            FilterIndex = filterIndex;
        }

        public unsafe SDLAudioCVT(int needed = default, ushort srcFormat = default, ushort dstFormat = default, double rateIncr = default, byte* buf = default, int len = default, int lenCvt = default, int lenMult = default, double lenRatio = default, Span<SDLAudioFilter> filters = default, int filterIndex = default)
        {
            Needed = needed;
            SrcFormat = srcFormat;
            DstFormat = dstFormat;
            RateIncr = rateIncr;
            Buf = buf;
            Len = len;
            LenCvt = lenCvt;
            LenMult = lenMult;
            LenRatio = lenRatio;
            if (filters != default(Span<SDLAudioFilter>))
            {
                Filters_0 = (void*)Marshal.GetFunctionPointerForDelegate(filters[0]);
                Filters_1 = (void*)Marshal.GetFunctionPointerForDelegate(filters[1]);
                Filters_2 = (void*)Marshal.GetFunctionPointerForDelegate(filters[2]);
                Filters_3 = (void*)Marshal.GetFunctionPointerForDelegate(filters[3]);
                Filters_4 = (void*)Marshal.GetFunctionPointerForDelegate(filters[4]);
                Filters_5 = (void*)Marshal.GetFunctionPointerForDelegate(filters[5]);
                Filters_6 = (void*)Marshal.GetFunctionPointerForDelegate(filters[6]);
                Filters_7 = (void*)Marshal.GetFunctionPointerForDelegate(filters[7]);
                Filters_8 = (void*)Marshal.GetFunctionPointerForDelegate(filters[8]);
                Filters_9 = (void*)Marshal.GetFunctionPointerForDelegate(filters[9]);
            }
            FilterIndex = filterIndex;
        }
    }
}