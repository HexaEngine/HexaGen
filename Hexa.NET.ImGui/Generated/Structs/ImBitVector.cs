// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HexaGen.Runtime;
using System.Numerics;

namespace Hexa.NET.ImGui
{
	/// <summary>
	/// To be documented.
	/// </summary>
	[NativeName(NativeNameType.StructOrClass, "ImBitVector")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct ImBitVector
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Storage")]
		[NativeName(NativeNameType.Type, "ImVector_ImU32")]
		public ImVectorImU32 Storage;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImBitVector(ImVectorImU32 storage = default)
		{
			Storage = storage;
		}


	}

}