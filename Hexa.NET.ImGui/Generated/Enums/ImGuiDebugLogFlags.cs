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
	[NativeName(NativeNameType.Enum, "ImGuiDebugLogFlags_")]
	[Flags]
	public enum ImGuiDebugLogFlags : int
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_None")]
		[NativeName(NativeNameType.Value, "0")]
		None = unchecked(0),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_EventActiveId")]
		[NativeName(NativeNameType.Value, "1")]
		EventActiveId = unchecked(1),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_EventFocus")]
		[NativeName(NativeNameType.Value, "2")]
		EventFocus = unchecked(2),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_EventPopup")]
		[NativeName(NativeNameType.Value, "4")]
		EventPopup = unchecked(4),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_EventNav")]
		[NativeName(NativeNameType.Value, "8")]
		EventNav = unchecked(8),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_EventClipper")]
		[NativeName(NativeNameType.Value, "16")]
		EventClipper = unchecked(16),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_EventSelection")]
		[NativeName(NativeNameType.Value, "32")]
		EventSelection = unchecked(32),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_EventIO")]
		[NativeName(NativeNameType.Value, "64")]
		EventIo = unchecked(64),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_EventInputRouting")]
		[NativeName(NativeNameType.Value, "128")]
		EventInputRouting = unchecked(128),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_EventDocking")]
		[NativeName(NativeNameType.Value, "256")]
		EventDocking = unchecked(256),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_EventViewport")]
		[NativeName(NativeNameType.Value, "512")]
		EventViewport = unchecked(512),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_EventMask_")]
		[NativeName(NativeNameType.Value, "1023")]
		EventMask = unchecked(1023),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_OutputToTTY")]
		[NativeName(NativeNameType.Value, "1048576")]
		OutputToTty = unchecked(1048576),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiDebugLogFlags_OutputToTestEngine")]
		[NativeName(NativeNameType.Value, "2097152")]
		OutputToTestEngine = unchecked(2097152),
	}
}
