namespace Hexa.NET.FreeType
{
    public static unsafe partial class FreeType
    {
        static FreeType()
        {
            LibraryLoader.SetImportResolver();
        }
    }
}