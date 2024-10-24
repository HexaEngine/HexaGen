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
	/// See ImFontAtlas::AddCustomRectXXX functions.<br/>
	/// </summary>
	[NativeName(NativeNameType.StructOrClass, "ImFontAtlasCustomRect")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct ImFontAtlasCustomRect
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Width")]
		[NativeName(NativeNameType.Type, "unsigned short")]
		public ushort Width;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Height")]
		[NativeName(NativeNameType.Type, "unsigned short")]
		public ushort Height;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "X")]
		[NativeName(NativeNameType.Type, "unsigned short")]
		public ushort X;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Y")]
		[NativeName(NativeNameType.Type, "unsigned short")]
		public ushort Y;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "GlyphID")]
		[NativeName(NativeNameType.Type, "unsigned int")]
		public uint GlyphID;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "GlyphAdvanceX")]
		[NativeName(NativeNameType.Type, "float")]
		public float GlyphAdvanceX;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "GlyphOffset")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 GlyphOffset;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Font")]
		[NativeName(NativeNameType.Type, "ImFont*")]
		public unsafe ImFont* Font;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImFontAtlasCustomRect(ushort width = default, ushort height = default, ushort x = default, ushort y = default, uint glyphId = default, float glyphAdvanceX = default, Vector2 glyphOffset = default, ImFont* font = default)
		{
			Width = width;
			Height = height;
			X = x;
			Y = y;
			GlyphID = glyphId;
			GlyphAdvanceX = glyphAdvanceX;
			GlyphOffset = glyphOffset;
			Font = font;
		}


		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImFontAtlasCustomRect_destroy")]
		[return: NativeName(NativeNameType.Type, "void")]
		public unsafe void Destroy()
		{
			fixed (ImFontAtlasCustomRect* @this = &this)
			{
				ImGui.DestroyNative(@this);
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImFontAtlasCustomRect_IsPacked")]
		[return: NativeName(NativeNameType.Type, "bool")]
		public unsafe bool IsPacked()
		{
			fixed (ImFontAtlasCustomRect* @this = &this)
			{
				byte ret = ImGui.IsPackedNative(@this);
				return ret != 0;
			}
		}

	}

}
