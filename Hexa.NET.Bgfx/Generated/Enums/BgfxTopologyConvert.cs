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

namespace Hexa.NET.Bgfx
{
	/// <summary>
	/// Topology conversion function.<br/>
	/// <br/>
	/// </summary>
	[NativeName(NativeNameType.Enum, "bgfx_topology_convert")]
	public enum BgfxTopologyConvert : int
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TOPOLOGY_CONVERT_TRI_LIST_FLIP_WINDING")]
		[NativeName(NativeNameType.Value, "0")]
		TriListFlipWinding = unchecked(0),

		/// <summary>
		/// ( 0) Flip winding order of triangle list. <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TOPOLOGY_CONVERT_TRI_STRIP_FLIP_WINDING")]
		[NativeName(NativeNameType.Value, "1")]
		TriStripFlipWinding = unchecked(1),

		/// <summary>
		/// ( 1) Flip winding order of triangle strip. <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TOPOLOGY_CONVERT_TRI_LIST_TO_LINE_LIST")]
		[NativeName(NativeNameType.Value, "2")]
		TriListToLineList = unchecked(2),

		/// <summary>
		/// ( 2) Convert triangle list to line list. <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TOPOLOGY_CONVERT_TRI_STRIP_TO_TRI_LIST")]
		[NativeName(NativeNameType.Value, "3")]
		TriStripToTriList = unchecked(3),

		/// <summary>
		/// ( 3) Convert triangle strip to triangle list. <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TOPOLOGY_CONVERT_LINE_STRIP_TO_LINE_LIST")]
		[NativeName(NativeNameType.Value, "4")]
		LineStripToLineList = unchecked(4),

		/// <summary>
		/// ( 4) Convert line strip to line list. <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TOPOLOGY_CONVERT_COUNT")]
		[NativeName(NativeNameType.Value, "5")]
		Count = unchecked(5),
	}
}
