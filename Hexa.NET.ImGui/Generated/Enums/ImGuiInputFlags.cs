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
	[NativeName(NativeNameType.Enum, "ImGuiInputFlags_")]
	[Flags]
	public enum ImGuiInputFlags : int
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_None")]
		[NativeName(NativeNameType.Value, "0")]
		None = unchecked(0),

		/// <summary>
		/// Enable repeat. Return true on successive repeats. Default for legacy IsKeyPressed(). NOT Default for legacy IsMouseClicked(). MUST BE == 1.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_Repeat")]
		[NativeName(NativeNameType.Value, "1")]
		Repeat = unchecked(1),

		/// <summary>
		/// Route to active item only.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RouteActive")]
		[NativeName(NativeNameType.Value, "1024")]
		RouteActive = unchecked(1024),

		/// <summary>
		/// Route to windows in the focus stack (DEFAULT). Deep-most focused window takes inputs. Active item takes inputs over deep-most focused window.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RouteFocused")]
		[NativeName(NativeNameType.Value, "2048")]
		RouteFocused = unchecked(2048),

		/// <summary>
		/// Global route (unless a focused window or active item registered the route).<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RouteGlobal")]
		[NativeName(NativeNameType.Value, "4096")]
		RouteGlobal = unchecked(4096),

		/// <summary>
		/// Do not register route, poll keys directly.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RouteAlways")]
		[NativeName(NativeNameType.Value, "8192")]
		RouteAlways = unchecked(8192),

		/// <summary>
		/// Option: global route: higher priority than focused route (unless active item in focused route).<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RouteOverFocused")]
		[NativeName(NativeNameType.Value, "16384")]
		RouteOverFocused = unchecked(16384),

		/// <summary>
		/// Option: global route: higher priority than active item. Unlikely you need to use that: will interfere with every active items, e.g. CTRL+A registered by InputText will be overridden by this. May not be fully honored as userinternal code is likely to always assume they can access keys when active.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RouteOverActive")]
		[NativeName(NativeNameType.Value, "32768")]
		RouteOverActive = unchecked(32768),

		/// <summary>
		/// Option: global route: will not be applied if underlying backgroundvoid is focused (== no Dear ImGui windows are focused). Useful for overlay applications.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RouteUnlessBgFocused")]
		[NativeName(NativeNameType.Value, "65536")]
		RouteUnlessBgFocused = unchecked(65536),

		/// <summary>
		/// Option: route evaluated from the point of view of root window rather than current window.<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_RouteFromRootWindow")]
		[NativeName(NativeNameType.Value, "131072")]
		RouteFromRootWindow = unchecked(131072),

		/// <summary>
		/// Automatically display a tooltip when hovering item [BETA] Unsure of right api (opt-inopt-out)<br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "ImGuiInputFlags_Tooltip")]
		[NativeName(NativeNameType.Value, "262144")]
		Tooltip = unchecked(262144),
	}
}
