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
	/// Selection request item<br/>
	/// </summary>
	[NativeName(NativeNameType.StructOrClass, "ImGuiSelectionRequest")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct ImGuiSelectionRequest
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Type")]
		[NativeName(NativeNameType.Type, "ImGuiSelectionRequestType")]
		public ImGuiSelectionRequestType Type;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Selected")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte Selected;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "RangeDirection")]
		[NativeName(NativeNameType.Type, "ImS8")]
		public byte RangeDirection;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "RangeFirstItem")]
		[NativeName(NativeNameType.Type, "ImGuiSelectionUserData")]
		public long RangeFirstItem;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "RangeLastItem")]
		[NativeName(NativeNameType.Type, "ImGuiSelectionUserData")]
		public long RangeLastItem;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImGuiSelectionRequest(ImGuiSelectionRequestType type = default, bool selected = default, byte rangeDirection = default, long rangeFirstItem = default, long rangeLastItem = default)
		{
			Type = type;
			Selected = selected ? (byte)1 : (byte)0;
			RangeDirection = rangeDirection;
			RangeFirstItem = rangeFirstItem;
			RangeLastItem = rangeLastItem;
		}


	}

}
