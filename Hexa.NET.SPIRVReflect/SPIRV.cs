namespace Hexa.NET.SPIRVReflect
{
    public static unsafe partial class SPIRV
    {
        static SPIRV()
        {
            LibraryLoader.SetImportResolver();
        }
    }
}