namespace D3D11Test
{
    using Hexa.NET.D3D11;
    using Hexa.NET.DXGI;
    using HexaGen.Runtime.COM;

    public unsafe class DXGISwapChain : IDisposable
    {
        private readonly D3D11DeviceManager manager;
        private readonly DxgiSwapChainDesc1 desc;
        private ComPtr<IDXGISwapChain1> swapChain;
        private bool disposedValue;

        private ComPtr<ID3D11Texture2D> backbuffer;
        private ComPtr<ID3D11RenderTargetView> rtv;

        public DXGISwapChain(D3D11DeviceManager manager, ComPtr<IDXGISwapChain1> swapChain, DxgiSwapChainDesc1 desc)
        {
            Console.WriteLine("SwapChain Init");

            this.manager = manager;
            this.swapChain = swapChain;
            this.desc = desc;

            swapChain.GetBuffer(0, out backbuffer);
            manager.Device.CreateRenderTargetView((ID3D11Resource*)backbuffer.Handle, null, out rtv);

            Console.WriteLine("SwapChain Init ... Done");
        }

        public ComPtr<ID3D11RenderTargetView> RTV => rtv;

        public ComPtr<ID3D11Texture2D> Backbuffer => backbuffer;

        public void Present(uint syncInterval, uint flags)
        {
            swapChain.Present(syncInterval, flags);
        }

        public void Resize(int width, int height)
        {
            Console.WriteLine($"SwapChain Resize");
            rtv.Release();
            backbuffer.Release();

            swapChain.ResizeBuffers(desc.BufferCount, (uint)width, (uint)height, desc.Format, desc.Flags);

            swapChain.GetBuffer(0, out backbuffer);
            manager.Device.CreateRenderTargetView((ID3D11Resource*)backbuffer.Handle, null, out rtv);
            Console.WriteLine($"SwapChain Resize ... Done");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                rtv.Dispose();
                backbuffer.Dispose();
                swapChain.Dispose();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}