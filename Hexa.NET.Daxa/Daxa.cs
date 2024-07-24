namespace Hexa.NET.Daxa
{
    public static unsafe partial class Daxa
    {
        static Daxa()
        {
            LibraryLoader.SetImportResolver();
        }

        public static void CheckError(this DaxaResult result)
        {
            if (result != DaxaResult.Success)
            {
                throw new Exception($"Daxa error: {result}");
            }
        }
    }
}