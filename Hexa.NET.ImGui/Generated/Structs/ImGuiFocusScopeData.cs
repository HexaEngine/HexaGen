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
	[NativeName(NativeNameType.StructOrClass, "ImGuiFocusScopeData")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct ImGuiFocusScopeData
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "ID")]
		[NativeName(NativeNameType.Type, "ImGuiID")]
		public uint ID;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "WindowID")]
		[NativeName(NativeNameType.Type, "ImGuiID")]
		public uint WindowID;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImGuiFocusScopeData(uint id = default, uint windowId = default)
		{
			ID = id;
			WindowID = windowId;
		}


	}

}
