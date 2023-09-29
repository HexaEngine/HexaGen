namespace Hexa.NET.SPIRVCross
{
    public static unsafe partial class SPIRV
    {
        static SPIRV()
        {
            LibraryLoader.SetImportResolver();
        }
    }
}