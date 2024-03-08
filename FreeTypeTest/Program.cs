namespace FreeTypeTest
{
    using Hexa.NET.FreeType;
    using System.Runtime.InteropServices;

    internal unsafe class Program
    {
        private static int Main(string[] args)
        {
            FTError error;
            FTLibrary library;
            error = (FTError)FreeType.FTInitFreeType(&library);
            if (error != FTError.FtErrOk)
            {
                Console.WriteLine($"Failed to initialize library, {error}");
                return 1;
            }

            FTFace faceHandle;

            error = (FTError)FreeType.FTNewFace(library, "arial.ttf", 0, &faceHandle);
            if (error != FTError.FtErrOk)
            {
                Console.WriteLine($"Failed to load font file, {error}");
                return 1;
            }

            FTFaceRec* face = (FTFaceRec*)faceHandle.Handle;

            Console.WriteLine($"Font Family: {Marshal.PtrToStringUTF8((nint)face->FamilyName)}");
            Console.WriteLine($"Font Style: {Marshal.PtrToStringUTF8((nint)face->StyleName)}");
            Console.WriteLine($"Num Glyphs: {face->NumGlyphs}");

            faceHandle.DoneFace();
            FreeType.FTDoneFreeType(library);
            return 0;
        }
    }
}