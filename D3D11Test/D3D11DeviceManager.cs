namespace D3D11Test
{
    using Hexa.NET.D3D11;
    using Hexa.NET.D3DCommon;
    using Hexa.NET.DXGI;
    using HexaGen.Runtime.COM;
    using System.Runtime.CompilerServices;

    public unsafe class D3D11DeviceManager : IDisposable
    {
        private ComPtr<ID3D11Device5> device;
        private ComPtr<ID3D11DeviceContext4> deviceContext;
        private ComPtr<ID3D11Debug> debug;
        private bool disposedValue;

        public D3D11DeviceManager(DXGIAdapter adapter, bool debug)
        {
            Console.WriteLine("D3D11 Init");
            D3DFeatureLevel[] levelsArr = new D3DFeatureLevel[]
            {
                D3DFeatureLevel.Level111,
                D3DFeatureLevel.Level110
            };

            D3D11CreateDeviceFlag flags = D3D11CreateDeviceFlag.BgraSupport;

            if (debug)
            {
                flags |= D3D11CreateDeviceFlag.Debug;
            }

            ComPtr<ID3D11Device> tempDevice = default;
            ComPtr<ID3D11DeviceContext> tempContext = default;

            D3DFeatureLevel level = 0;
            D3DFeatureLevel* levels = (D3DFeatureLevel*)Unsafe.AsPointer(ref levelsArr[0]);

            D3D11.D3D11CreateDevice((IDXGIAdapter*)adapter.Adapter.Handle, D3DDriverType.Unknown, IntPtr.Zero, (uint)flags, levels, (uint)levelsArr.Length, D3D11.D3D11_SDK_VERSION, tempDevice.GetAddressOf(), &level, tempContext.GetAddressOf());
            Level = level;

            tempDevice.QueryInterface(out device);
            tempContext.QueryInterface(out deviceContext);

            tempDevice.Release();
            tempContext.Release();

            if (debug)
            {
                device.QueryInterface(out this.debug);
            }

            Console.WriteLine("D3D11 Init ... Done");
        }

        public D3DFeatureLevel Level { get; }

        public ComPtr<ID3D11Device5> Device => device;

        public ComPtr<ID3D11DeviceContext4> Context => deviceContext;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                device.Dispose();
                deviceContext.Dispose();
                GC.WaitForPendingFinalizers();
                GC.WaitForFullGCComplete();

                debug.ReportLiveDeviceObjects(D3D11RldoFlags.Detail);

                debug.Dispose();
                GC.WaitForPendingFinalizers();
                GC.WaitForFullGCComplete();
                disposedValue = true;
            }
        }

        ~D3D11DeviceManager()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}