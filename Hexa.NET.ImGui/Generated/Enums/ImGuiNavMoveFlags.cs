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
	[NativeName(NativeNameType.Enum, "ImGuiNavMoveFlags_")]
	[Flags]
	public enum ImGuiNavMoveFlags : int
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_None")]
		[NativeName(NativeNameType.Value, "0")]
		None = unchecked(0),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_LoopX")]
		[NativeName(NativeNameType.Value, "1")]
		LoopX = unchecked(1),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_LoopY")]
		[NativeName(NativeNameType.Value, "2")]
		LoopY = unchecked(2),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_WrapX")]
		[NativeName(NativeNameType.Value, "4")]
		WrapX = unchecked(4),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_WrapY")]
		[NativeName(NativeNameType.Value, "8")]
		WrapY = unchecked(8),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_WrapMask_")]
		[NativeName(NativeNameType.Value, "15")]
		WrapMask = unchecked(15),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_AllowCurrentNavId")]
		[NativeName(NativeNameType.Value, "16")]
		AllowCurrentNavId = unchecked(16),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_AlsoScoreVisibleSet")]
		[NativeName(NativeNameType.Value, "32")]
		AlsoScoreVisibleSet = unchecked(32),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_ScrollToEdgeY")]
		[NativeName(NativeNameType.Value, "64")]
		ScrollToEdgeY = unchecked(64),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_Forwarded")]
		[NativeName(NativeNameType.Value, "128")]
		Forwarded = unchecked(128),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_DebugNoResult")]
		[NativeName(NativeNameType.Value, "256")]
		DebugNoResult = unchecked(256),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_FocusApi")]
		[NativeName(NativeNameType.Value, "512")]
		FocusApi = unchecked(512),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_IsTabbing")]
		[NativeName(NativeNameType.Value, "1024")]
		IsTabbing = unchecked(1024),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_IsPageMove")]
		[NativeName(NativeNameType.Value, "2048")]
		IsPageMove = unchecked(2048),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_Activate")]
		[NativeName(NativeNameType.Value, "4096")]
		Activate = unchecked(4096),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_NoSelect")]
		[NativeName(NativeNameType.Value, "8192")]
		NoSelect = unchecked(8192),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_NoSetNavHighlight")]
		[NativeName(NativeNameType.Value, "16384")]
		NoSetNavHighlight = unchecked(16384),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiNavMoveFlags_NoClearActiveId")]
		[NativeName(NativeNameType.Value, "32768")]
		NoClearActiveId = unchecked(32768),
	}
}
