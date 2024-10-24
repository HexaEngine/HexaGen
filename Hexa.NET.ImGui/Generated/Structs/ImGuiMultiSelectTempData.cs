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
	[NativeName(NativeNameType.StructOrClass, "ImGuiMultiSelectTempData")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct ImGuiMultiSelectTempData
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "IO")]
		[NativeName(NativeNameType.Type, "ImGuiMultiSelectIO")]
		public ImGuiMultiSelectIO IO;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Storage")]
		[NativeName(NativeNameType.Type, "ImGuiMultiSelectState*")]
		public unsafe ImGuiMultiSelectState* Storage;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "FocusScopeId")]
		[NativeName(NativeNameType.Type, "ImGuiID")]
		public uint FocusScopeId;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Flags")]
		[NativeName(NativeNameType.Type, "ImGuiMultiSelectFlags")]
		public ImGuiMultiSelectFlags Flags;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "ScopeRectMin")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 ScopeRectMin;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BackupCursorMaxPos")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 BackupCursorMaxPos;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "LastSubmittedItem")]
		[NativeName(NativeNameType.Type, "ImGuiSelectionUserData")]
		public long LastSubmittedItem;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BoxSelectId")]
		[NativeName(NativeNameType.Type, "ImGuiID")]
		public uint BoxSelectId;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "KeyMods")]
		[NativeName(NativeNameType.Type, "ImGuiKeyChord")]
		public int KeyMods;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "LoopRequestSetAll")]
		[NativeName(NativeNameType.Type, "ImS8")]
		public byte LoopRequestSetAll;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "IsEndIO")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte IsEndIO;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "IsFocused")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte IsFocused;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "IsKeyboardSetRange")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte IsKeyboardSetRange;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "NavIdPassedBy")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte NavIdPassedBy;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "RangeSrcPassedBy")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte RangeSrcPassedBy;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "RangeDstPassedBy")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte RangeDstPassedBy;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImGuiMultiSelectTempData(ImGuiMultiSelectIO io = default, ImGuiMultiSelectState* storage = default, uint focusScopeId = default, ImGuiMultiSelectFlags flags = default, Vector2 scopeRectMin = default, Vector2 backupCursorMaxPos = default, long lastSubmittedItem = default, uint boxSelectId = default, int keyMods = default, byte loopRequestSetAll = default, bool isEndIo = default, bool isFocused = default, bool isKeyboardSetRange = default, bool navIdPassedBy = default, bool rangeSrcPassedBy = default, bool rangeDstPassedBy = default)
		{
			IO = io;
			Storage = storage;
			FocusScopeId = focusScopeId;
			Flags = flags;
			ScopeRectMin = scopeRectMin;
			BackupCursorMaxPos = backupCursorMaxPos;
			LastSubmittedItem = lastSubmittedItem;
			BoxSelectId = boxSelectId;
			KeyMods = keyMods;
			LoopRequestSetAll = loopRequestSetAll;
			IsEndIO = isEndIo ? (byte)1 : (byte)0;
			IsFocused = isFocused ? (byte)1 : (byte)0;
			IsKeyboardSetRange = isKeyboardSetRange ? (byte)1 : (byte)0;
			NavIdPassedBy = navIdPassedBy ? (byte)1 : (byte)0;
			RangeSrcPassedBy = rangeSrcPassedBy ? (byte)1 : (byte)0;
			RangeDstPassedBy = rangeDstPassedBy ? (byte)1 : (byte)0;
		}


	}

}
