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
	[NativeName(NativeNameType.Enum, "ImGuiConfigFlags_")]
	[Flags]
	public enum ImGuiConfigFlags : int
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_None")]
		[NativeName(NativeNameType.Value, "0")]
		None = unchecked(0),

		/// <summary>
		/// Master keyboard navigation enable flag. Enable full Tabbing + directional arrows + spaceenter to activate.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_NavEnableKeyboard")]
		[NativeName(NativeNameType.Value, "1")]
		NavEnableKeyboard = unchecked(1),

		/// <summary>
		/// Master gamepad navigation enable flag. Backend also needs to set ImGuiBackendFlags_HasGamepad.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_NavEnableGamepad")]
		[NativeName(NativeNameType.Value, "2")]
		NavEnableGamepad = unchecked(2),

		/// <summary>
		/// Instruct navigation to move the mouse cursor. May be useful on TVconsole systems where moving a virtual mouse is awkward. Will update io.MousePos and set io.WantSetMousePos=true. If enabled you MUST honor io.WantSetMousePos requests in your backend, otherwise ImGui will react as if the mouse is jumping around back and forth.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_NavEnableSetMousePos")]
		[NativeName(NativeNameType.Value, "4")]
		NavEnableSetMousePos = unchecked(4),

		/// <summary>
		/// Instruct navigation to not set the io.WantCaptureKeyboard flag when io.NavActive is set.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_NavNoCaptureKeyboard")]
		[NativeName(NativeNameType.Value, "8")]
		NavNoCaptureKeyboard = unchecked(8),

		/// <summary>
		/// Instruct dear imgui to disable mouse inputs and interactions.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_NoMouse")]
		[NativeName(NativeNameType.Value, "16")]
		NoMouse = unchecked(16),

		/// <summary>
		/// Instruct backend to not alter mouse cursor shape and visibility. Use if the backend cursor changes are interfering with yours and you don't want to use SetMouseCursor() to change mouse cursor. You may want to honor requests from imgui by reading GetMouseCursor() yourself instead.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_NoMouseCursorChange")]
		[NativeName(NativeNameType.Value, "32")]
		NoMouseCursorChange = unchecked(32),

		/// <summary>
		/// Instruct dear imgui to disable keyboard inputs and interactions. This is done by ignoring keyboard events and clearing existing states.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_NoKeyboard")]
		[NativeName(NativeNameType.Value, "64")]
		NoKeyboard = unchecked(64),

		/// <summary>
		/// Docking enable flags.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_DockingEnable")]
		[NativeName(NativeNameType.Value, "128")]
		DockingEnable = unchecked(128),

		/// <summary>
		/// Viewport enable flags (require both ImGuiBackendFlags_PlatformHasViewports + ImGuiBackendFlags_RendererHasViewports set by the respective backends)<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_ViewportsEnable")]
		[NativeName(NativeNameType.Value, "1024")]
		ViewportsEnable = unchecked(1024),

		/// <summary>
		/// [BETA: Don't use] FIXME-DPI: Reposition and resize imgui windows when the DpiScale of a viewport changed (mostly useful for the main viewport hosting other window). Note that resizing the main window itself is up to your application.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_DpiEnableScaleViewports")]
		[NativeName(NativeNameType.Value, "16384")]
		DpiEnableScaleViewports = unchecked(16384),

		/// <summary>
		/// [BETA: Don't use] FIXME-DPI: Request bitmap-scaled fonts to match DpiScale. This is a very low-quality workaround. The correct way to handle DPI is _currently_ to replace the atlas andor fonts in the Platform_OnChangedViewport callback, but this is all early work in progress.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_DpiEnableScaleFonts")]
		[NativeName(NativeNameType.Value, "32768")]
		DpiEnableScaleFonts = unchecked(32768),

		/// <summary>
		/// Application is SRGB-aware.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_IsSRGB")]
		[NativeName(NativeNameType.Value, "1048576")]
		IsSrgb = unchecked(1048576),

		/// <summary>
		/// Application is using a touch screen instead of a mouse.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiConfigFlags_IsTouchScreen")]
		[NativeName(NativeNameType.Value, "2097152")]
		IsTouchScreen = unchecked(2097152),
	}
}
