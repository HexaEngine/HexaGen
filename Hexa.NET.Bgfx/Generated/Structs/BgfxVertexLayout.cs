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

namespace Hexa.NET.Bgfx
{
	/// <summary>
	/// Vertex layout.<br/>
	/// <br/>
	/// </summary>
	[NativeName(NativeNameType.StructOrClass, "bgfx_vertex_layout_s")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct BgfxVertexLayout
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "hash")]
		[NativeName(NativeNameType.Type, "uint32_t")]
		public uint Hash;

		/// <summary>
		/// Hash.                                    <br/>
		/// </summary>
		[NativeName(NativeNameType.Field, "stride")]
		[NativeName(NativeNameType.Type, "uint16_t")]
		public ushort Stride;

		/// <summary>
		/// Stride.                                  <br/>
		/// </summary>
		[NativeName(NativeNameType.Field, "offset")]
		[NativeName(NativeNameType.Type, "uint16_t[18]")]
		public ushort Offset_0;
		public ushort Offset_1;
		public ushort Offset_2;
		public ushort Offset_3;
		public ushort Offset_4;
		public ushort Offset_5;
		public ushort Offset_6;
		public ushort Offset_7;
		public ushort Offset_8;
		public ushort Offset_9;
		public ushort Offset_10;
		public ushort Offset_11;
		public ushort Offset_12;
		public ushort Offset_13;
		public ushort Offset_14;
		public ushort Offset_15;
		public ushort Offset_16;
		public ushort Offset_17;

		/// <summary>
		/// Attribute offsets.                       <br/>
		/// </summary>
		[NativeName(NativeNameType.Field, "attributes")]
		[NativeName(NativeNameType.Type, "uint16_t[18]")]
		public ushort Attributes_0;
		public ushort Attributes_1;
		public ushort Attributes_2;
		public ushort Attributes_3;
		public ushort Attributes_4;
		public ushort Attributes_5;
		public ushort Attributes_6;
		public ushort Attributes_7;
		public ushort Attributes_8;
		public ushort Attributes_9;
		public ushort Attributes_10;
		public ushort Attributes_11;
		public ushort Attributes_12;
		public ushort Attributes_13;
		public ushort Attributes_14;
		public ushort Attributes_15;
		public ushort Attributes_16;
		public ushort Attributes_17;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe BgfxVertexLayout(uint hash = default, ushort stride = default, ushort* offset = default, ushort* attributes = default)
		{
			Hash = hash;
			Stride = stride;
			if (offset != default(ushort*))
			{
				Offset_0 = offset[0];
				Offset_1 = offset[1];
				Offset_2 = offset[2];
				Offset_3 = offset[3];
				Offset_4 = offset[4];
				Offset_5 = offset[5];
				Offset_6 = offset[6];
				Offset_7 = offset[7];
				Offset_8 = offset[8];
				Offset_9 = offset[9];
				Offset_10 = offset[10];
				Offset_11 = offset[11];
				Offset_12 = offset[12];
				Offset_13 = offset[13];
				Offset_14 = offset[14];
				Offset_15 = offset[15];
				Offset_16 = offset[16];
				Offset_17 = offset[17];
			}
			if (attributes != default(ushort*))
			{
				Attributes_0 = attributes[0];
				Attributes_1 = attributes[1];
				Attributes_2 = attributes[2];
				Attributes_3 = attributes[3];
				Attributes_4 = attributes[4];
				Attributes_5 = attributes[5];
				Attributes_6 = attributes[6];
				Attributes_7 = attributes[7];
				Attributes_8 = attributes[8];
				Attributes_9 = attributes[9];
				Attributes_10 = attributes[10];
				Attributes_11 = attributes[11];
				Attributes_12 = attributes[12];
				Attributes_13 = attributes[13];
				Attributes_14 = attributes[14];
				Attributes_15 = attributes[15];
				Attributes_16 = attributes[16];
				Attributes_17 = attributes[17];
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe BgfxVertexLayout(uint hash = default, ushort stride = default, Span<ushort> offset = default, Span<ushort> attributes = default)
		{
			Hash = hash;
			Stride = stride;
			if (offset != default(Span<ushort>))
			{
				Offset_0 = offset[0];
				Offset_1 = offset[1];
				Offset_2 = offset[2];
				Offset_3 = offset[3];
				Offset_4 = offset[4];
				Offset_5 = offset[5];
				Offset_6 = offset[6];
				Offset_7 = offset[7];
				Offset_8 = offset[8];
				Offset_9 = offset[9];
				Offset_10 = offset[10];
				Offset_11 = offset[11];
				Offset_12 = offset[12];
				Offset_13 = offset[13];
				Offset_14 = offset[14];
				Offset_15 = offset[15];
				Offset_16 = offset[16];
				Offset_17 = offset[17];
			}
			if (attributes != default(Span<ushort>))
			{
				Attributes_0 = attributes[0];
				Attributes_1 = attributes[1];
				Attributes_2 = attributes[2];
				Attributes_3 = attributes[3];
				Attributes_4 = attributes[4];
				Attributes_5 = attributes[5];
				Attributes_6 = attributes[6];
				Attributes_7 = attributes[7];
				Attributes_8 = attributes[8];
				Attributes_9 = attributes[9];
				Attributes_10 = attributes[10];
				Attributes_11 = attributes[11];
				Attributes_12 = attributes[12];
				Attributes_13 = attributes[13];
				Attributes_14 = attributes[14];
				Attributes_15 = attributes[15];
				Attributes_16 = attributes[16];
				Attributes_17 = attributes[17];
			}
		}


	}

}
