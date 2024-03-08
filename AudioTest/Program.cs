// See https://aka.ms/new-console-template for more information
using Hexa.NET.X3DAudio;
using Hexa.NET.XAudio2;
using HexaGen.Runtime.COM;
using System.Numerics;
using XAudioTest;

unsafe
{
    ComPtr<IXAudio2> audio = default;

    XAudio2.XAudio2CreateWithVersionInfo(audio.GetAddressOf(), 0, XAudio2.XAudio2_USE_DEFAULT_PROCESSOR, 0).ThrowIf();

    audio.StartEngine().ThrowIf();
    ComPtr<IXAudio2MasteringVoice> master = default;
    audio.CreateMasteringVoice(ref master, 2, 192000, 0, null, null, AudioStreamCategory.GameMedia).ThrowIf();

    uint channelMask;
    master.GetChannelMask(&channelMask);

    X3DAudioHandle handle = new();
    X3DAudio.X3DAudioInitialize(channelMask, X3DAudio.X3DAudio_SPEED_OF_SOUND, &handle).ThrowIf();

    for (int i = 0; i < 20; i++)
    {
        Console.WriteLine(handle.Data[i]);
    }

    var fs = File.OpenRead("CantinaBand60.wav");

    XAudio2WaveAudioStream stream = new(fs)
    {
        Looping = true
    };
    var waveFormat = stream.GetWaveFormat();
    ComPtr<IXAudio2SourceVoice> source = default;

    audio.CreateSourceVoice(ref source, ref waveFormat, 0, 1, (IXAudio2VoiceCallback*)null, null, null).ThrowIf();

    stream.Initialize(source);
    source.Start(0, 0).ThrowIf();

    X3DAudioListener listener = new(Vector4.UnitZ, Vector4.UnitY, Vector4.Zero, Vector4.Zero);

    X3DAudioEmitter emitter = new(null, Vector4.UnitZ, Vector4.UnitY, Vector4.Zero, Vector4.Zero);
    float* channelAzu = stackalloc float[waveFormat.Channels + 32];
    emitter.PChannelAzimuths = channelAzu;
    emitter.ChannelCount = 1;
    emitter.CurveDistanceScaler = emitter.DopplerScaler = 1;
    var pos = emitter.Position;

    X3DAudioDspSettings settings = default;
    float* matrix = stackalloc float[waveFormat.Channels + 32];
    settings.SrcChannelCount = 1;
    settings.DstChannelCount = waveFormat.Channels;
    settings.PMatrixCoefficients = matrix;
    X3DAudio.X3DAudioCalculate(&handle, &listener, &emitter, X3DAudio.X3DAudio_CALCULATE_MATRIX | X3DAudio.X3DAudio_CALCULATE_DOPPLER | X3DAudio.X3DAudio_CALCULATE_LPF_DIRECT | X3DAudio.X3DAudio_CALCULATE_REVERB, &settings);

    bool running = true;
    while (running)
    {
        stream.Update(source);
        running = !stream.ReachedEnd;
        Thread.Sleep(10);
    }

    stream.Dispose();

    source.Release();
    master.Release();
    audio.Release();
}