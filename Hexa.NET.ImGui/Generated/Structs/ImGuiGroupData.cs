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
	[NativeName(NativeNameType.StructOrClass, "ImGuiGroupData")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct ImGuiGroupData
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "WindowID")]
		[NativeName(NativeNameType.Type, "ImGuiID")]
		public uint WindowID;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BackupCursorPos")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 BackupCursorPos;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BackupCursorMaxPos")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 BackupCursorMaxPos;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BackupCursorPosPrevLine")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 BackupCursorPosPrevLine;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BackupIndent")]
		[NativeName(NativeNameType.Type, "ImVec1")]
		public ImVec1 BackupIndent;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BackupGroupOffset")]
		[NativeName(NativeNameType.Type, "ImVec1")]
		public ImVec1 BackupGroupOffset;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BackupCurrLineSize")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 BackupCurrLineSize;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BackupCurrLineTextBaseOffset")]
		[NativeName(NativeNameType.Type, "float")]
		public float BackupCurrLineTextBaseOffset;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BackupActiveIdIsAlive")]
		[NativeName(NativeNameType.Type, "ImGuiID")]
		public uint BackupActiveIdIsAlive;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BackupActiveIdPreviousFrameIsAlive")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte BackupActiveIdPreviousFrameIsAlive;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BackupHoveredIdIsAlive")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte BackupHoveredIdIsAlive;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BackupIsSameLine")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte BackupIsSameLine;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "EmitItem")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte EmitItem;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImGuiGroupData(uint windowId = default, Vector2 backupCursorPos = default, Vector2 backupCursorMaxPos = default, Vector2 backupCursorPosPrevLine = default, ImVec1 backupIndent = default, ImVec1 backupGroupOffset = default, Vector2 backupCurrLineSize = default, float backupCurrLineTextBaseOffset = default, uint backupActiveIdIsAlive = default, bool backupActiveIdPreviousFrameIsAlive = default, bool backupHoveredIdIsAlive = default, bool backupIsSameLine = default, bool emitItem = default)
		{
			WindowID = windowId;
			BackupCursorPos = backupCursorPos;
			BackupCursorMaxPos = backupCursorMaxPos;
			BackupCursorPosPrevLine = backupCursorPosPrevLine;
			BackupIndent = backupIndent;
			BackupGroupOffset = backupGroupOffset;
			BackupCurrLineSize = backupCurrLineSize;
			BackupCurrLineTextBaseOffset = backupCurrLineTextBaseOffset;
			BackupActiveIdIsAlive = backupActiveIdIsAlive;
			BackupActiveIdPreviousFrameIsAlive = backupActiveIdPreviousFrameIsAlive ? (byte)1 : (byte)0;
			BackupHoveredIdIsAlive = backupHoveredIdIsAlive ? (byte)1 : (byte)0;
			BackupIsSameLine = backupIsSameLine ? (byte)1 : (byte)0;
			EmitItem = emitItem ? (byte)1 : (byte)0;
		}


	}

}