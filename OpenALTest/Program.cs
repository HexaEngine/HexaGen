// See https://aka.ms/new-console-template for more information

using HexaEngine.OpenAL;
using System.Diagnostics;
using System.Runtime.CompilerServices;

unsafe
{
    ALCdevice* device = OpenAL.AlcOpenDevice(null);

    ALCcontext* context = OpenAL.AlcCreateContext(device, null);
    CheckError(device);
    OpenAL.AlDistanceModel(OpenAL.AL_INVERSE_DISTANCE_CLAMPED);
    CheckError(device);

    OpenAL.AlcMakeContextCurrent(context);
    CheckError(device);

    var fs = File.OpenRead("CantinaBand60.wav");

    OpenALWaveAudioStream stream = new(fs)
    {
        Looping = true
    };

    uint source = 0;
    OpenAL.AlGenSources(1, &source);

    stream.Initialize(source);

    OpenAL.AlSourcePlay(source);

    bool running = true;
    while (running)
    {
        stream.Update(source);
        running = !stream.ReachedEnd;
        Thread.Sleep(10);
    }

    stream.Dispose();

    OpenAL.AlDeleteSources(1, &source);
    OpenAL.AlcDestroyContext(context);
    OpenAL.AlcCloseDevice(device);
}

static unsafe bool CheckError(ALCdevice* device, [CallerFilePath] string filename = "", [CallerLineNumber] int line = 0, [CallerMemberName] string name = "")
{
    int error = OpenAL.AlcGetError(device);
    if (error != OpenAL.ALC_NO_ERROR)
    {
        Debug.WriteLine($"***OpenAL ERROR*** ({filename}: {line}, {name})");
        throw error switch
        {
            OpenAL.ALC_INVALID_VALUE => new Exception("ALC_INVALID_VALUE: an invalid value was passed to an OpenAL function"),
            OpenAL.ALC_INVALID_DEVICE => new Exception("ALC_INVALID_DEVICE: a bad device was passed to an OpenAL function"),
            OpenAL.ALC_INVALID_CONTEXT => new Exception("ALC_INVALID_CONTEXT: a bad context was passed to an OpenAL function"),
            OpenAL.ALC_INVALID_ENUM => new Exception("ALC_INVALID_ENUM: an unknown enum value was passed to an OpenAL function"),
            OpenAL.ALC_OUT_OF_MEMORY => new Exception("ALC_OUT_OF_MEMORY: an unknown enum value was passed to an OpenAL function"),
            _ => new Exception($"UNKNOWN ALC ERROR: {error}"),
        };
    }
    return true;
}