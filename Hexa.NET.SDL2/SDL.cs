namespace Hexa.NET.SDL2
{
    public static unsafe partial class SDL
    {
        static SDL()
        {
            LibraryLoader.SetImportResolver();
        }
    }
}