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
	[NativeName(NativeNameType.StructOrClass, "ImGuiPtrOrIndex")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct ImGuiPtrOrIndex
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Ptr")]
		[NativeName(NativeNameType.Type, "void*")]
		public unsafe void* Ptr;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Index")]
		[NativeName(NativeNameType.Type, "int")]
		public int Index;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImGuiPtrOrIndex(void* ptr = default, int index = default)
		{
			Ptr = ptr;
			Index = index;
		}


	}

}
