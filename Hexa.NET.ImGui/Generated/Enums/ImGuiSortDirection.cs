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
	[NativeName(NativeNameType.Enum, "ImGuiSortDirection")]
	[Flags]
	public enum ImGuiSortDirection : int
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiSortDirection_None")]
		[NativeName(NativeNameType.Value, "0")]
		None = unchecked(0),

		/// <summary>
		/// Ascending = 0-&gt;9, A-&gt;Z etc.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiSortDirection_Ascending")]
		[NativeName(NativeNameType.Value, "1")]
		Ascending = unchecked(1),

		/// <summary>
		/// Descending = 9-&gt;0, Z-&gt;A etc.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiSortDirection_Descending")]
		[NativeName(NativeNameType.Value, "2")]
		Descending = unchecked(2),
	}
}
