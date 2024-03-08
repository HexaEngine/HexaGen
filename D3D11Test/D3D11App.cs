namespace D3D11Test
{
    using Hexa.NET.D3D11;
    using HexaGen.Runtime.COM;
    using System.Numerics;

    public unsafe class D3D11App : IApp
    {
        private ComPtr<ID3D11Device5> device;
        private ComPtr<ID3D11DeviceContext4> context;
        private SDLWindow window;
        private DXGISwapChain swapChain;
        private D3D11DeviceManager deviceManager;
        private bool resized = false;

        public void Init(SDLWindow window, D3D11DeviceManager manager, DXGIAdapter adapter)
        {
            Console.WriteLine("App Init");
            this.window = window;
            swapChain = adapter.CreateSwapChain(manager, window);
            deviceManager = manager;
            device = manager.Device;
            context = manager.Context;
            window.Resized += WindowResized;
        }

        private void WindowResized(ResizedEventArgs obj)
        {
            resized = true;
        }

        public void Render()
        {
            if (resized)
            {
                swapChain.Resize(window.Width, window.Height);
                resized = false;
            }

            Vector4 color = new(0.2f, 0.4f, 0.6f, 1);
            context.ClearRenderTargetView(swapChain.RTV, (float*)&color);

            swapChain.Present(1, 0);
        }

        public void Dispose()
        {
            swapChain.Dispose();
        }
    }
}