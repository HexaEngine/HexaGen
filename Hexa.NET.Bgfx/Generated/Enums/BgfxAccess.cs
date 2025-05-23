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
	/// Access mode enum.<br/>
	/// <br/>
	/// </summary>
	[NativeName(NativeNameType.Enum, "bgfx_access")]
	public enum BgfxAccess : int
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_ACCESS_READ")]
		[NativeName(NativeNameType.Value, "0")]
		Read = unchecked(0),

		/// <summary>
		/// ( 0) Read.                          <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_ACCESS_WRITE")]
		[NativeName(NativeNameType.Value, "1")]
		Write = unchecked(1),

		/// <summary>
		/// ( 1) Write.                         <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_ACCESS_READWRITE")]
		[NativeName(NativeNameType.Value, "2")]
		Readwrite = unchecked(2),

		/// <summary>
		/// ( 2) Read and write.                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_ACCESS_COUNT")]
		[NativeName(NativeNameType.Value, "3")]
		Count = unchecked(3),
	}
}
