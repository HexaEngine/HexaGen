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
	/// Primitive topology.<br/>
	/// <br/>
	/// </summary>
	[NativeName(NativeNameType.Enum, "bgfx_topology")]
	public enum BgfxTopology : int
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TOPOLOGY_TRI_LIST")]
		[NativeName(NativeNameType.Value, "0")]
		TriList = unchecked(0),

		/// <summary>
		/// ( 0) Triangle list.                 <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TOPOLOGY_TRI_STRIP")]
		[NativeName(NativeNameType.Value, "1")]
		TriStrip = unchecked(1),

		/// <summary>
		/// ( 1) Triangle strip.                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TOPOLOGY_LINE_LIST")]
		[NativeName(NativeNameType.Value, "2")]
		LineList = unchecked(2),

		/// <summary>
		/// ( 2) Line list.                     <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TOPOLOGY_LINE_STRIP")]
		[NativeName(NativeNameType.Value, "3")]
		LineStrip = unchecked(3),

		/// <summary>
		/// ( 3) Line strip.                    <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TOPOLOGY_POINT_LIST")]
		[NativeName(NativeNameType.Value, "4")]
		PointList = unchecked(4),

		/// <summary>
		/// ( 4) Point list.                    <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TOPOLOGY_COUNT")]
		[NativeName(NativeNameType.Value, "5")]
		Count = unchecked(5),
	}
}
