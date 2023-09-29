namespace D3D11Test
{
    using Hexa.NET.DXGI;
    using HexaGen.Runtime.COM;

    public class DXGISwapChain
    {
        private readonly D3D11DeviceManager manager;
        private ComPtr<IDXGISwapChain2> swapChain;
        private readonly DxgiSwapChainDesc1 desc;

        public DXGISwapChain(D3D11DeviceManager manager, ComPtr<IDXGISwapChain2> swapChain, DxgiSwapChainDesc1 desc)
        {
            this.manager = manager;
            this.swapChain = swapChain;
            this.desc = desc;
        }
    }
}