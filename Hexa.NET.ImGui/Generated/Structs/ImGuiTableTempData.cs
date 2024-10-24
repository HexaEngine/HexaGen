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
	[NativeName(NativeNameType.StructOrClass, "ImGuiTableTempData")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct ImGuiTableTempData
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "TableIndex")]
		[NativeName(NativeNameType.Type, "int")]
		public int TableIndex;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "LastTimeActive")]
		[NativeName(NativeNameType.Type, "float")]
		public float LastTimeActive;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "AngledHeadersExtraWidth")]
		[NativeName(NativeNameType.Type, "float")]
		public float AngledHeadersExtraWidth;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "AngledHeadersRequests")]
		[NativeName(NativeNameType.Type, "ImVector_ImGuiTableHeaderData")]
		public ImVectorImGuiTableHeaderData AngledHeadersRequests;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "UserOuterSize")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 UserOuterSize;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "DrawSplitter")]
		[NativeName(NativeNameType.Type, "ImDrawListSplitter")]
		public ImDrawListSplitter DrawSplitter;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "HostBackupWorkRect")]
		[NativeName(NativeNameType.Type, "ImRect")]
		public ImRect HostBackupWorkRect;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "HostBackupParentWorkRect")]
		[NativeName(NativeNameType.Type, "ImRect")]
		public ImRect HostBackupParentWorkRect;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "HostBackupPrevLineSize")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 HostBackupPrevLineSize;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "HostBackupCurrLineSize")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 HostBackupCurrLineSize;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "HostBackupCursorMaxPos")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 HostBackupCursorMaxPos;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "HostBackupColumnsOffset")]
		[NativeName(NativeNameType.Type, "ImVec1")]
		public ImVec1 HostBackupColumnsOffset;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "HostBackupItemWidth")]
		[NativeName(NativeNameType.Type, "float")]
		public float HostBackupItemWidth;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "HostBackupItemWidthStackSize")]
		[NativeName(NativeNameType.Type, "int")]
		public int HostBackupItemWidthStackSize;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImGuiTableTempData(int tableIndex = default, float lastTimeActive = default, float angledHeadersExtraWidth = default, ImVectorImGuiTableHeaderData angledHeadersRequests = default, Vector2 userOuterSize = default, ImDrawListSplitter drawSplitter = default, ImRect hostBackupWorkRect = default, ImRect hostBackupParentWorkRect = default, Vector2 hostBackupPrevLineSize = default, Vector2 hostBackupCurrLineSize = default, Vector2 hostBackupCursorMaxPos = default, ImVec1 hostBackupColumnsOffset = default, float hostBackupItemWidth = default, int hostBackupItemWidthStackSize = default)
		{
			TableIndex = tableIndex;
			LastTimeActive = lastTimeActive;
			AngledHeadersExtraWidth = angledHeadersExtraWidth;
			AngledHeadersRequests = angledHeadersRequests;
			UserOuterSize = userOuterSize;
			DrawSplitter = drawSplitter;
			HostBackupWorkRect = hostBackupWorkRect;
			HostBackupParentWorkRect = hostBackupParentWorkRect;
			HostBackupPrevLineSize = hostBackupPrevLineSize;
			HostBackupCurrLineSize = hostBackupCurrLineSize;
			HostBackupCursorMaxPos = hostBackupCursorMaxPos;
			HostBackupColumnsOffset = hostBackupColumnsOffset;
			HostBackupItemWidth = hostBackupItemWidth;
			HostBackupItemWidthStackSize = hostBackupItemWidthStackSize;
		}


	}

}
