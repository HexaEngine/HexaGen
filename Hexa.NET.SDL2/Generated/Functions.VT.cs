// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HexaGen.Runtime;

namespace Hexa.NET.SDL2
{
	public unsafe partial class SDL
	{
		internal static VTable vt;

		public static void InitApi()
		{
			vt = new VTable(GetLibraryName(), 1424);
			vt.Load(0, "SDL_GetPlatform");

		public static void FreeApi()
		{
			vt.Free();
		}
	}
}