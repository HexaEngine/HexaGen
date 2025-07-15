namespace HexaGen
{
    public unsafe static class PathHelper
    {
        public static string GetPath(string path)
        {
            if (path == null) return null;
            string sanitizedPath = Path.GetFullPath(path);

            fixed (char* p = sanitizedPath)
            {
                char* pPath = p;
                char* end = pPath + sanitizedPath.Length;
                while (pPath != end)
                {
                    char c = *pPath;
                    if (c == '\\' || c == '/')
                    {
                        *pPath = Path.DirectorySeparatorChar;
                    }
                    pPath++;
                }
            }

            return sanitizedPath;
        }
    }
}
