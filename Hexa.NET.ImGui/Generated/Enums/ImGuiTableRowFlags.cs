// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using HexaGen.Runtime;
using System.Numerics;

namespace Hexa.NET.ImGui
{
	/// <summary>
	/// To be documented.
	/// </summary>
	[NativeName(NativeNameType.Enum, "ImGuiTableRowFlags_")]
	[Flags]
	public enum ImGuiTableRowFlags : int
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiTableRowFlags_None")]
		[NativeName(NativeNameType.Value, "0")]
		None = unchecked(0),

		/// <summary>
		/// Identify header row (set default background color + width of its contents accounted differently for auto column width)<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiTableRowFlags_Headers")]
		[NativeName(NativeNameType.Value, "1")]
		Headers = unchecked(1),
	}
}
