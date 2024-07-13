namespace Hexa.NET.SDL2
{
    using HexaGen.Runtime;

    public static unsafe partial class SDL
    {
        static SDL()
        {
            LibraryLoader.SetImportResolver();
        }

        public static Exception? GetErrorAsException()
        {
            byte* ex = SDLGetError();

            if (ex == null || ex[0] == '\0')
            {
                return null;
            }

            return new Exception(Utils.DecodeStringUTF8(ex));
        }
    }
}