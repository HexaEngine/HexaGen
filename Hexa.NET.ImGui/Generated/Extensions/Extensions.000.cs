// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using HexaGen.Runtime;
using System.Numerics;

namespace Hexa.NET.ImGui
{
	public static unsafe partial class Extensions
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "igImage")]
		[return: NativeName(NativeNameType.Type, "void")]
		public static void Image(this ImTextureID userTextureId, [NativeName(NativeNameType.Param, "image_size")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 imageSize, [NativeName(NativeNameType.Param, "uv0")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 uv0, [NativeName(NativeNameType.Param, "uv1")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 uv1, [NativeName(NativeNameType.Param, "tint_col")] [NativeName(NativeNameType.Type, "const ImVec4")] Vector4 tintCol, [NativeName(NativeNameType.Param, "border_col")] [NativeName(NativeNameType.Type, "const ImVec4")] Vector4 borderCol)
		{
			ImGui.ImageNative(userTextureId, imageSize, uv0, uv1, tintCol, borderCol);
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "igImage")]
		[return: NativeName(NativeNameType.Type, "void")]
		public static void Image(this ImTextureID userTextureId, [NativeName(NativeNameType.Param, "image_size")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 imageSize, [NativeName(NativeNameType.Param, "uv0")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 uv0, [NativeName(NativeNameType.Param, "uv1")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 uv1, [NativeName(NativeNameType.Param, "tint_col")] [NativeName(NativeNameType.Type, "const ImVec4")] Vector4 tintCol)
		{
			ImGui.ImageNative(userTextureId, imageSize, uv0, uv1, tintCol, (Vector4)(new Vector4(0,0,0,0)));
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "igImage")]
		[return: NativeName(NativeNameType.Type, "void")]
		public static void Image(this ImTextureID userTextureId, [NativeName(NativeNameType.Param, "image_size")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 imageSize, [NativeName(NativeNameType.Param, "uv0")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 uv0, [NativeName(NativeNameType.Param, "uv1")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 uv1)
		{
			ImGui.ImageNative(userTextureId, imageSize, uv0, uv1, (Vector4)(new Vector4(1,1,1,1)), (Vector4)(new Vector4(0,0,0,0)));
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "igImage")]
		[return: NativeName(NativeNameType.Type, "void")]
		public static void Image(this ImTextureID userTextureId, [NativeName(NativeNameType.Param, "image_size")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 imageSize, [NativeName(NativeNameType.Param, "uv0")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 uv0)
		{
			ImGui.ImageNative(userTextureId, imageSize, uv0, (Vector2)(new Vector2(1,1)), (Vector4)(new Vector4(1,1,1,1)), (Vector4)(new Vector4(0,0,0,0)));
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "igImage")]
		[return: NativeName(NativeNameType.Type, "void")]
		public static void Image(this ImTextureID userTextureId, [NativeName(NativeNameType.Param, "image_size")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 imageSize)
		{
			ImGui.ImageNative(userTextureId, imageSize, (Vector2)(new Vector2(0,0)), (Vector2)(new Vector2(1,1)), (Vector4)(new Vector4(1,1,1,1)), (Vector4)(new Vector4(0,0,0,0)));
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "igImage")]
		[return: NativeName(NativeNameType.Type, "void")]
		public static void Image(this ImTextureID userTextureId, [NativeName(NativeNameType.Param, "image_size")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 imageSize, [NativeName(NativeNameType.Param, "uv0")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 uv0, [NativeName(NativeNameType.Param, "tint_col")] [NativeName(NativeNameType.Type, "const ImVec4")] Vector4 tintCol)
		{
			ImGui.ImageNative(userTextureId, imageSize, uv0, (Vector2)(new Vector2(1,1)), tintCol, (Vector4)(new Vector4(0,0,0,0)));
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "igImage")]
		[return: NativeName(NativeNameType.Type, "void")]
		public static void Image(this ImTextureID userTextureId, [NativeName(NativeNameType.Param, "image_size")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 imageSize, [NativeName(NativeNameType.Param, "tint_col")] [NativeName(NativeNameType.Type, "const ImVec4")] Vector4 tintCol)
		{
			ImGui.ImageNative(userTextureId, imageSize, (Vector2)(new Vector2(0,0)), (Vector2)(new Vector2(1,1)), tintCol, (Vector4)(new Vector4(0,0,0,0)));
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "igImage")]
		[return: NativeName(NativeNameType.Type, "void")]
		public static void Image(this ImTextureID userTextureId, [NativeName(NativeNameType.Param, "image_size")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 imageSize, [NativeName(NativeNameType.Param, "uv0")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 uv0, [NativeName(NativeNameType.Param, "tint_col")] [NativeName(NativeNameType.Type, "const ImVec4")] Vector4 tintCol, [NativeName(NativeNameType.Param, "border_col")] [NativeName(NativeNameType.Type, "const ImVec4")] Vector4 borderCol)
		{
			ImGui.ImageNative(userTextureId, imageSize, uv0, (Vector2)(new Vector2(1,1)), tintCol, borderCol);
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "igImage")]
		[return: NativeName(NativeNameType.Type, "void")]
		public static void Image(this ImTextureID userTextureId, [NativeName(NativeNameType.Param, "image_size")] [NativeName(NativeNameType.Type, "const ImVec2")] Vector2 imageSize, [NativeName(NativeNameType.Param, "tint_col")] [NativeName(NativeNameType.Type, "const ImVec4")] Vector4 tintCol, [NativeName(NativeNameType.Param, "border_col")] [NativeName(NativeNameType.Type, "const ImVec4")] Vector4 borderCol)
		{
			ImGui.ImageNative(userTextureId, imageSize, (Vector2)(new Vector2(0,0)), (Vector2)(new Vector2(1,1)), tintCol, borderCol);
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "igImFileClose")]
		[return: NativeName(NativeNameType.Type, "bool")]
		public static bool Close(this ImFileHandle file)
		{
			byte ret = ImGui.ImFileCloseNative(file);
			return ret != 0;
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "igImFileGetSize")]
		[return: NativeName(NativeNameType.Type, "ImU64")]
		public static ulong GetSize(this ImFileHandle file)
		{
			ulong ret = ImGui.ImFileGetSizeNative(file);
			return ret;
		}

	}
}
