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
	[NativeName(NativeNameType.StructOrClass, "ImGuiShrinkWidthItem")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct ImGuiShrinkWidthItem
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Index")]
		[NativeName(NativeNameType.Type, "int")]
		public int Index;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Width")]
		[NativeName(NativeNameType.Type, "float")]
		public float Width;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "InitialWidth")]
		[NativeName(NativeNameType.Type, "float")]
		public float InitialWidth;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImGuiShrinkWidthItem(int index = default, float width = default, float initialWidth = default)
		{
			Index = index;
			Width = width;
			InitialWidth = initialWidth;
		}


	}

}