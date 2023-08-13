// See https://aka.ms/new-console-template for more information
using HexaEngine.XAudio2;
using HexaGen.Runtime.COM;
using XAudioTest;

unsafe
{
    ComPtr<IXAudio2> audio = default;

    XAudio2.XAudio2CreateWithVersionInfo(audio.GetAddressOf(), 0, XAudio2.XAudio2UseDefaultProcessor, 0).ThrowIf();

    audio.StartEngine().ThrowIf();
    ComPtr<IXAudio2MasteringVoice> master = default;
    audio.CreateMasteringVoice(ref master, 2, 192000, 0, null, null, AudioStreamCategory.GameMedia).ThrowIf();

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