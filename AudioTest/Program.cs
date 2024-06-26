// See https://aka.ms/new-console-template for more information
using Hexa.NET.X3DAudio;
using Hexa.NET.XAudio2;
using HexaGen.Runtime.COM;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using XAudioTest;

unsafe
{
    ComPtr<IXAudio2> audio = default;

    XAudio2.XAudio2CreateWithVersionInfo(audio.GetAddressOf(), 0, XAudio2.XAudio2_USE_DEFAULT_PROCESSOR, 0).ThrowIf();

    audio.StartEngine().ThrowIf();
    ComPtr<IXAudio2MasteringVoice> master = default;
    audio.CreateMasteringVoice(ref master, 8, 192000, 0, null, null, AudioStreamCategory.GameMedia).ThrowIf();

    XAudio2VoiceDetails details;
    master.GetVoiceDetails(&details);

    ComPtr<IXAudio2SubmixVoice> submix = default;
    audio.CreateSubmixVoice(ref submix, details.InputChannels, details.InputSampleRate, 0, 0, null, null).ThrowIf();

    uint channelMask;
    master.GetChannelMask(&channelMask);

    X3DAudioHandle handle = new();
    X3DAudio.X3DAudioInitialize(channelMask, X3DAudio.X3DAudio_SPEED_OF_SOUND, &handle).ThrowIf();

    var fs = File.OpenRead("CantinaBand60.wav");

    XAudio2WaveAudioStream stream = new(fs)
    {
        Looping = true
    };
    var waveFormat = stream.GetWaveFormat();
    ComPtr<IXAudio2SourceVoice> source = default;

    audio.CreateSourceVoice(ref source, ref waveFormat, 0, 1, (IXAudio2VoiceCallback*)null, null, null).ThrowIf();

    stream.Initialize(source);
    source.Start(0, 0).ThrowIf(); source.SetVolume(10, 0);

    X3DAudioListener listener = new(Vector3.UnitZ, Vector3.UnitY, Vector3.Zero, Vector3.Zero);

    X3DAudioEmitter emitter = new(null, Vector3.UnitZ, Vector3.UnitY, Vector3.Zero, Vector3.Zero);

    emitter.ChannelCount = 1;
    emitter.CurveDistanceScaler = emitter.DopplerScaler = 1;

    X3DAudioDspSettings settings = default;
    float* matrix = (float*)Marshal.AllocHGlobal((nint)(emitter.ChannelCount * details.InputChannels) * sizeof(float));

    settings.SrcChannelCount = emitter.ChannelCount;
    settings.DstChannelCount = details.InputChannels;
    settings.PMatrixCoefficients = matrix;

    X3DAudio.X3DAudioCalculate(handle, &listener, &emitter, X3DAudio.X3DAudio_CALCULATE_MATRIX | X3DAudio.X3DAudio_CALCULATE_DOPPLER | X3DAudio.X3DAudio_CALCULATE_LPF_DIRECT | X3DAudio.X3DAudio_CALCULATE_REVERB, &settings);

    float angleXZ = 90.0f;
    float angleY = 0.0f;
    float radiusXZ = 1;
    float radiusY = 1;
    float lastX = radiusXZ * MathF.Cos(angleXZ);
    float lastZ = radiusXZ * MathF.Sin(angleXZ);
    float lastY = radiusY * MathF.Sin(angleY);

    bool running = true;
    while (running)
    {
        stream.Update(source);
        running = !stream.ReachedEnd;

        // Update emitter position to move in a circle around the listener on the XZ plane
        emitter.Position.X = radiusXZ * MathF.Cos(angleXZ);
        emitter.Position.Z = radiusXZ * MathF.Sin(angleXZ);
        emitter.Position.Y = radiusY * MathF.Sin(angleY);

        emitter.Velocity.X = (emitter.Position.X - lastX) / 0.01f;
        emitter.Velocity.Z = (emitter.Position.Z - lastZ) / 0.01f;
        emitter.Velocity.Y = (emitter.Position.Y - lastY) / 0.01f;

        lastX = emitter.Position.X;
        lastZ = emitter.Position.Z;
        lastY = emitter.Position.Y;

        angleXZ += 0.05f;
        angleY += 0.075f;

        X3DAudio.X3DAudioCalculate(handle, &listener, &emitter,
           X3DAudio.X3DAudio_CALCULATE_MATRIX | X3DAudio.X3DAudio_CALCULATE_DOPPLER |
           X3DAudio.X3DAudio_CALCULATE_LPF_DIRECT | X3DAudio.X3DAudio_CALCULATE_REVERB, &settings);

        source.SetOutputMatrix((IXAudio2Voice*)master.Handle, waveFormat.Channels, details.InputChannels, settings.PMatrixCoefficients, 0u);
        source.SetFrequencyRatio(settings.DopplerFactor, XAudio2.XAudio2_COMMIT_NOW);
        source.SetOutputMatrix((IXAudio2Voice*)submix.Handle, 1, 1, &settings.ReverbLevel, 0);

        XAudio2FilterParameters FilterParameters = new(XAudio2FilterType.LowPassFilter, 2.0f * MathF.Sin(float.Pi / 6.0f * settings.LPFDirectCoefficient), 1.0f);
        source.SetFilterParameters(&FilterParameters, 0);

        Thread.Sleep(10);
    }

    Marshal.FreeHGlobal((nint)settings.PMatrixCoefficients);

    stream.Dispose();

    source.Release();
    submix.Release();
    master.Release();
    audio.Release();
}