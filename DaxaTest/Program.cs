namespace DaxaTest
{
    using Hexa.NET.Daxa;
    using System.Runtime.InteropServices;

    internal unsafe class Program
    {
        private static void Main(string[] args)
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
        }

        private static unsafe int OnSelector(DaxaDeviceProperties* properties)
        {
            return Daxa.DaxaDefaultDeviceScore(properties);
        }
    }
}