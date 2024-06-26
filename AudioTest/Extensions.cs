namespace XAudioTest
{
    using HexaGen.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class Extensions
    {
        public static void ThrowIf(this int hresult)
        {
            HResult result = hresult;
            result.ThrowIf();
        }
    }
}