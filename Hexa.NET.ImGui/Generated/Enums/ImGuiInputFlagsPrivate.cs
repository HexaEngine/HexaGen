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
	[NativeName(NativeNameType.Enum, "ImGuiInputFlagsPrivate_")]
	public enum ImGuiInputFlagsPrivate : int
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RepeatRateDefault")]
		[NativeName(NativeNameType.Value, "2")]
		RepeatRateDefault = unchecked(2),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RepeatRateNavMove")]
		[NativeName(NativeNameType.Value, "4")]
		RepeatRateNavMove = unchecked(4),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RepeatRateNavTweak")]
		[NativeName(NativeNameType.Value, "8")]
		RepeatRateNavTweak = unchecked(8),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RepeatUntilRelease")]
		[NativeName(NativeNameType.Value, "16")]
		RepeatUntilRelease = unchecked(16),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RepeatUntilKeyModsChange")]
		[NativeName(NativeNameType.Value, "32")]
		RepeatUntilKeyModsChange = unchecked(32),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RepeatUntilKeyModsChangeFromNone")]
		[NativeName(NativeNameType.Value, "64")]
		RepeatUntilKeyModsChangeFromNone = unchecked(64),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RepeatUntilOtherKeyPress")]
		[NativeName(NativeNameType.Value, "128")]
		RepeatUntilOtherKeyPress = unchecked(128),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_LockThisFrame")]
		[NativeName(NativeNameType.Value, "1048576")]
		LockThisFrame = unchecked(1048576),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_LockUntilRelease")]
		[NativeName(NativeNameType.Value, "2097152")]
		LockUntilRelease = unchecked(2097152),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_CondHovered")]
		[NativeName(NativeNameType.Value, "4194304")]
		CondHovered = unchecked(4194304),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_CondActive")]
		[NativeName(NativeNameType.Value, "8388608")]
		CondActive = unchecked(8388608),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_CondDefault_")]
		[NativeName(NativeNameType.Value, "12582912")]
		CondDefault = unchecked(12582912),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RepeatRateMask_")]
		[NativeName(NativeNameType.Value, "14")]
		RepeatRateMask = unchecked(14),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RepeatUntilMask_")]
		[NativeName(NativeNameType.Value, "240")]
		RepeatUntilMask = unchecked(240),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RepeatMask_")]
		[NativeName(NativeNameType.Value, "255")]
		RepeatMask = unchecked(255),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_CondMask_")]
		[NativeName(NativeNameType.Value, "12582912")]
		CondMask = unchecked(12582912),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RouteTypeMask_")]
		[NativeName(NativeNameType.Value, "15360")]
		RouteTypeMask = unchecked(15360),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RouteOptionsMask_")]
		[NativeName(NativeNameType.Value, "245760")]
		RouteOptionsMask = unchecked(245760),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_SupportedByIsKeyPressed")]
		[NativeName(NativeNameType.Value, "ImGuiInputFlags_RepeatMask_")]
		SupportedByIsKeyPressed = RepeatMask,

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_SupportedByIsMouseClicked")]
		[NativeName(NativeNameType.Value, "ImGuiInputFlags_Repeat")]
		SupportedByIsMouseClicked = unchecked((int)ImGuiInputFlags.Repeat),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_SupportedByShortcut")]
		[NativeName(NativeNameType.Value, "261375")]
		SupportedByShortcut = unchecked(261375),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_SupportedBySetNextItemShortcut")]
		[NativeName(NativeNameType.Value, "523519")]
		SupportedBySetNextItemShortcut = unchecked(523519),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_SupportedBySetKeyOwner")]
		[NativeName(NativeNameType.Value, "3145728")]
		SupportedBySetKeyOwner = unchecked(3145728),

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_SupportedBySetItemKeyOwner")]
		[NativeName(NativeNameType.Value, "15728640")]
		SupportedBySetItemKeyOwner = unchecked(15728640),
	}
}