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
	[NativeName(NativeNameType.StructOrClass, "ImGuiNextWindowData")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct ImGuiNextWindowData
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Flags")]
		[NativeName(NativeNameType.Type, "ImGuiNextWindowDataFlags")]
		public ImGuiNextWindowDataFlags Flags;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "PosCond")]
		[NativeName(NativeNameType.Type, "ImGuiCond")]
		public ImGuiCond PosCond;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "SizeCond")]
		[NativeName(NativeNameType.Type, "ImGuiCond")]
		public ImGuiCond SizeCond;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "CollapsedCond")]
		[NativeName(NativeNameType.Type, "ImGuiCond")]
		public ImGuiCond CollapsedCond;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "DockCond")]
		[NativeName(NativeNameType.Type, "ImGuiCond")]
		public ImGuiCond DockCond;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "PosVal")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 PosVal;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "PosPivotVal")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 PosPivotVal;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "SizeVal")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 SizeVal;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "ContentSizeVal")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 ContentSizeVal;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "ScrollVal")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 ScrollVal;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "ChildFlags")]
		[NativeName(NativeNameType.Type, "ImGuiChildFlags")]
		public ImGuiChildFlags ChildFlags;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "PosUndock")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte PosUndock;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "CollapsedVal")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte CollapsedVal;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "SizeConstraintRect")]
		[NativeName(NativeNameType.Type, "ImRect")]
		public ImRect SizeConstraintRect;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "SizeCallback")]
		[NativeName(NativeNameType.Type, "ImGuiSizeCallback")]
		public unsafe void* SizeCallback;
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "SizeCallbackUserData")]
		[NativeName(NativeNameType.Type, "void*")]
		public unsafe void* SizeCallbackUserData;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "BgAlphaVal")]
		[NativeName(NativeNameType.Type, "float")]
		public float BgAlphaVal;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "ViewportId")]
		[NativeName(NativeNameType.Type, "ImGuiID")]
		public uint ViewportId;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "DockId")]
		[NativeName(NativeNameType.Type, "ImGuiID")]
		public uint DockId;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "WindowClass")]
		[NativeName(NativeNameType.Type, "ImGuiWindowClass")]
		public ImGuiWindowClass WindowClass;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "MenuBarOffsetMinVal")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 MenuBarOffsetMinVal;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "RefreshFlagsVal")]
		[NativeName(NativeNameType.Type, "ImGuiWindowRefreshFlags")]
		public ImGuiWindowRefreshFlags RefreshFlagsVal;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImGuiNextWindowData(ImGuiNextWindowDataFlags flags = default, ImGuiCond posCond = default, ImGuiCond sizeCond = default, ImGuiCond collapsedCond = default, ImGuiCond dockCond = default, Vector2 posVal = default, Vector2 posPivotVal = default, Vector2 sizeVal = default, Vector2 contentSizeVal = default, Vector2 scrollVal = default, ImGuiChildFlags childFlags = default, bool posUndock = default, bool collapsedVal = default, ImRect sizeConstraintRect = default, ImGuiSizeCallback sizeCallback = default, void* sizeCallbackUserData = default, float bgAlphaVal = default, uint viewportId = default, uint dockId = default, ImGuiWindowClass windowClass = default, Vector2 menuBarOffsetMinVal = default, ImGuiWindowRefreshFlags refreshFlagsVal = default)
		{
			Flags = flags;
			PosCond = posCond;
			SizeCond = sizeCond;
			CollapsedCond = collapsedCond;
			DockCond = dockCond;
			PosVal = posVal;
			PosPivotVal = posPivotVal;
			SizeVal = sizeVal;
			ContentSizeVal = contentSizeVal;
			ScrollVal = scrollVal;
			ChildFlags = childFlags;
			PosUndock = posUndock ? (byte)1 : (byte)0;
			CollapsedVal = collapsedVal ? (byte)1 : (byte)0;
			SizeConstraintRect = sizeConstraintRect;
			SizeCallback = (void*)Marshal.GetFunctionPointerForDelegate(sizeCallback);
			SizeCallbackUserData = sizeCallbackUserData;
			BgAlphaVal = bgAlphaVal;
			ViewportId = viewportId;
			DockId = dockId;
			WindowClass = windowClass;
			MenuBarOffsetMinVal = menuBarOffsetMinVal;
			RefreshFlagsVal = refreshFlagsVal;
		}


	}

}
