namespace Hexa.NET.OpenAL
{
    public static unsafe partial class OpenAL
    {
        static OpenAL()
        {
            LibraryLoader.SetImportResolver();
        }
    }
}