namespace Hexa.NET.XAudio2
{
    public struct WaveFormatEx
    {
        /// <summary>
        /// format type
        /// </summary>
        public ushort FormatTag;

        /// <summary>
        /// number of channels (i.e. mono, stereo...)
        /// </summary>
        public ushort Channels;

        /// <summary>
        /// sample rate
        /// </summary>
        public uint SamplesPerSec;

        /// <summary>
        /// for buffer estimation
        /// </summary>
        public uint AvgBytesPerSec;

        /// <summary>
        /// block size of data
        /// </summary>
        public ushort BlockAlign;

        /// <summary>
        /// number of bits per sample of mono data
        /// </summary>
        public ushort BitsPerSample;

        /// <summary>
        /// the count in bytes of the size of
        /// </summary>
        public ushort Size;
    }
}