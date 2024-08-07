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
	[NativeName(NativeNameType.StructOrClass, "ImFontConfig")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct ImFontConfig
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "FontData")]
		[NativeName(NativeNameType.Type, "void*")]
		public unsafe void* FontData;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "FontDataSize")]
		[NativeName(NativeNameType.Type, "int")]
		public int FontDataSize;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "FontDataOwnedByAtlas")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte FontDataOwnedByAtlas;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "FontNo")]
		[NativeName(NativeNameType.Type, "int")]
		public int FontNo;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "SizePixels")]
		[NativeName(NativeNameType.Type, "float")]
		public float SizePixels;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "OversampleH")]
		[NativeName(NativeNameType.Type, "int")]
		public int OversampleH;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "OversampleV")]
		[NativeName(NativeNameType.Type, "int")]
		public int OversampleV;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "PixelSnapH")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte PixelSnapH;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "GlyphExtraSpacing")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 GlyphExtraSpacing;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "GlyphOffset")]
		[NativeName(NativeNameType.Type, "ImVec2")]
		public Vector2 GlyphOffset;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "GlyphRanges")]
		[NativeName(NativeNameType.Type, "const ImWchar*")]
		public unsafe char* GlyphRanges;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "GlyphMinAdvanceX")]
		[NativeName(NativeNameType.Type, "float")]
		public float GlyphMinAdvanceX;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "GlyphMaxAdvanceX")]
		[NativeName(NativeNameType.Type, "float")]
		public float GlyphMaxAdvanceX;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "MergeMode")]
		[NativeName(NativeNameType.Type, "bool")]
		public byte MergeMode;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "FontBuilderFlags")]
		[NativeName(NativeNameType.Type, "unsigned int")]
		public uint FontBuilderFlags;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "RasterizerMultiply")]
		[NativeName(NativeNameType.Type, "float")]
		public float RasterizerMultiply;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "RasterizerDensity")]
		[NativeName(NativeNameType.Type, "float")]
		public float RasterizerDensity;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "EllipsisChar")]
		[NativeName(NativeNameType.Type, "ImWchar")]
		public char EllipsisChar;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Name")]
		[NativeName(NativeNameType.Type, "char[40]")]
		public byte Name_0;
		public byte Name_1;
		public byte Name_2;
		public byte Name_3;
		public byte Name_4;
		public byte Name_5;
		public byte Name_6;
		public byte Name_7;
		public byte Name_8;
		public byte Name_9;
		public byte Name_10;
		public byte Name_11;
		public byte Name_12;
		public byte Name_13;
		public byte Name_14;
		public byte Name_15;
		public byte Name_16;
		public byte Name_17;
		public byte Name_18;
		public byte Name_19;
		public byte Name_20;
		public byte Name_21;
		public byte Name_22;
		public byte Name_23;
		public byte Name_24;
		public byte Name_25;
		public byte Name_26;
		public byte Name_27;
		public byte Name_28;
		public byte Name_29;
		public byte Name_30;
		public byte Name_31;
		public byte Name_32;
		public byte Name_33;
		public byte Name_34;
		public byte Name_35;
		public byte Name_36;
		public byte Name_37;
		public byte Name_38;
		public byte Name_39;

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "DstFont")]
		[NativeName(NativeNameType.Type, "ImFont*")]
		public unsafe ImFont* DstFont;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImFontConfig(void* fontData = default, int fontDataSize = default, bool fontDataOwnedByAtlas = default, int fontNo = default, float sizePixels = default, int oversampleH = default, int oversampleV = default, bool pixelSnapH = default, Vector2 glyphExtraSpacing = default, Vector2 glyphOffset = default, char* glyphRanges = default, float glyphMinAdvanceX = default, float glyphMaxAdvanceX = default, bool mergeMode = default, uint fontBuilderFlags = default, float rasterizerMultiply = default, float rasterizerDensity = default, char ellipsisChar = default, byte* name = default, ImFont* dstFont = default)
		{
			FontData = fontData;
			FontDataSize = fontDataSize;
			FontDataOwnedByAtlas = fontDataOwnedByAtlas ? (byte)1 : (byte)0;
			FontNo = fontNo;
			SizePixels = sizePixels;
			OversampleH = oversampleH;
			OversampleV = oversampleV;
			PixelSnapH = pixelSnapH ? (byte)1 : (byte)0;
			GlyphExtraSpacing = glyphExtraSpacing;
			GlyphOffset = glyphOffset;
			GlyphRanges = glyphRanges;
			GlyphMinAdvanceX = glyphMinAdvanceX;
			GlyphMaxAdvanceX = glyphMaxAdvanceX;
			MergeMode = mergeMode ? (byte)1 : (byte)0;
			FontBuilderFlags = fontBuilderFlags;
			RasterizerMultiply = rasterizerMultiply;
			RasterizerDensity = rasterizerDensity;
			EllipsisChar = ellipsisChar;
			if (name != default(byte*))
			{
				Name_0 = name[0];
				Name_1 = name[1];
				Name_2 = name[2];
				Name_3 = name[3];
				Name_4 = name[4];
				Name_5 = name[5];
				Name_6 = name[6];
				Name_7 = name[7];
				Name_8 = name[8];
				Name_9 = name[9];
				Name_10 = name[10];
				Name_11 = name[11];
				Name_12 = name[12];
				Name_13 = name[13];
				Name_14 = name[14];
				Name_15 = name[15];
				Name_16 = name[16];
				Name_17 = name[17];
				Name_18 = name[18];
				Name_19 = name[19];
				Name_20 = name[20];
				Name_21 = name[21];
				Name_22 = name[22];
				Name_23 = name[23];
				Name_24 = name[24];
				Name_25 = name[25];
				Name_26 = name[26];
				Name_27 = name[27];
				Name_28 = name[28];
				Name_29 = name[29];
				Name_30 = name[30];
				Name_31 = name[31];
				Name_32 = name[32];
				Name_33 = name[33];
				Name_34 = name[34];
				Name_35 = name[35];
				Name_36 = name[36];
				Name_37 = name[37];
				Name_38 = name[38];
				Name_39 = name[39];
			}
			DstFont = dstFont;
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImFontConfig(void* fontData = default, int fontDataSize = default, bool fontDataOwnedByAtlas = default, int fontNo = default, float sizePixels = default, int oversampleH = default, int oversampleV = default, bool pixelSnapH = default, Vector2 glyphExtraSpacing = default, Vector2 glyphOffset = default, char* glyphRanges = default, float glyphMinAdvanceX = default, float glyphMaxAdvanceX = default, bool mergeMode = default, uint fontBuilderFlags = default, float rasterizerMultiply = default, float rasterizerDensity = default, char ellipsisChar = default, Span<byte> name = default, ImFont* dstFont = default)
		{
			FontData = fontData;
			FontDataSize = fontDataSize;
			FontDataOwnedByAtlas = fontDataOwnedByAtlas ? (byte)1 : (byte)0;
			FontNo = fontNo;
			SizePixels = sizePixels;
			OversampleH = oversampleH;
			OversampleV = oversampleV;
			PixelSnapH = pixelSnapH ? (byte)1 : (byte)0;
			GlyphExtraSpacing = glyphExtraSpacing;
			GlyphOffset = glyphOffset;
			GlyphRanges = glyphRanges;
			GlyphMinAdvanceX = glyphMinAdvanceX;
			GlyphMaxAdvanceX = glyphMaxAdvanceX;
			MergeMode = mergeMode ? (byte)1 : (byte)0;
			FontBuilderFlags = fontBuilderFlags;
			RasterizerMultiply = rasterizerMultiply;
			RasterizerDensity = rasterizerDensity;
			EllipsisChar = ellipsisChar;
			if (name != default(Span<byte>))
			{
				Name_0 = name[0];
				Name_1 = name[1];
				Name_2 = name[2];
				Name_3 = name[3];
				Name_4 = name[4];
				Name_5 = name[5];
				Name_6 = name[6];
				Name_7 = name[7];
				Name_8 = name[8];
				Name_9 = name[9];
				Name_10 = name[10];
				Name_11 = name[11];
				Name_12 = name[12];
				Name_13 = name[13];
				Name_14 = name[14];
				Name_15 = name[15];
				Name_16 = name[16];
				Name_17 = name[17];
				Name_18 = name[18];
				Name_19 = name[19];
				Name_20 = name[20];
				Name_21 = name[21];
				Name_22 = name[22];
				Name_23 = name[23];
				Name_24 = name[24];
				Name_25 = name[25];
				Name_26 = name[26];
				Name_27 = name[27];
				Name_28 = name[28];
				Name_29 = name[29];
				Name_30 = name[30];
				Name_31 = name[31];
				Name_32 = name[32];
				Name_33 = name[33];
				Name_34 = name[34];
				Name_35 = name[35];
				Name_36 = name[36];
				Name_37 = name[37];
				Name_38 = name[38];
				Name_39 = name[39];
			}
			DstFont = dstFont;
		}


		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImFontConfig_destroy")]
		[return: NativeName(NativeNameType.Type, "void")]
		public unsafe void Destroy()
		{
			fixed (ImFontConfig* @this = &this)
			{
				ImGui.DestroyNative(@this);
			}
		}

	}

}
