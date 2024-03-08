namespace D3D11Test
{
    public interface IApp
    {
        public void Init(SDLWindow window, D3D11DeviceManager manager, DXGIAdapter adapter);

        public void Render();

        public void Dispose();
    }
}