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

namespace Hexa.NET.Bgfx
{
	/// <summary>
	/// Texture format enum.<br/>
	/// Notation:<br/>
	/// RGBA16S<br/>
	/// ^   ^ ^<br/>
	/// |   | +-- [ ]Unorm<br/>
	/// |   |     [F]loat<br/>
	/// |   |     [S]norm<br/>
	/// |   |     [I]nt<br/>
	/// |   |     [U]int<br/>
	/// |   +---- Number of bits per component<br/>
	/// +-------- Components<br/>
	/// <br/>
	/// <summary>
	/// To be documented.
	/// </summary>
	/// <br/>
	/// </summary>
	[NativeName(NativeNameType.Enum, "bgfx_texture_format")]
	public enum BgfxTextureFormat : int
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_BC1")]
		[NativeName(NativeNameType.Value, "0")]
		Bc1 = unchecked(0),

		/// <summary>
		/// ( 0) DXT1 R5G6B5A1                  <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_BC2")]
		[NativeName(NativeNameType.Value, "1")]
		Bc2 = unchecked(1),

		/// <summary>
		/// ( 1) DXT3 R5G6B5A4                  <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_BC3")]
		[NativeName(NativeNameType.Value, "2")]
		Bc3 = unchecked(2),

		/// <summary>
		/// ( 2) DXT5 R5G6B5A8                  <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_BC4")]
		[NativeName(NativeNameType.Value, "3")]
		Bc4 = unchecked(3),

		/// <summary>
		/// ( 3) LATC1/ATI1 R8                  <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_BC5")]
		[NativeName(NativeNameType.Value, "4")]
		Bc5 = unchecked(4),

		/// <summary>
		/// ( 4) LATC2/ATI2 RG8                 <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_BC6H")]
		[NativeName(NativeNameType.Value, "5")]
		Bc6H = unchecked(5),

		/// <summary>
		/// ( 5) BC6H RGB16F                    <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_BC7")]
		[NativeName(NativeNameType.Value, "6")]
		Bc7 = unchecked(6),

		/// <summary>
		/// ( 6) BC7 RGB 4-7 bits per color channel, 0-8 bits alpha <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ETC1")]
		[NativeName(NativeNameType.Value, "7")]
		Etc1 = unchecked(7),

		/// <summary>
		/// ( 7) ETC1 RGB8                      <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ETC2")]
		[NativeName(NativeNameType.Value, "8")]
		Etc2 = unchecked(8),

		/// <summary>
		/// ( 8) ETC2 RGB8                      <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ETC2A")]
		[NativeName(NativeNameType.Value, "9")]
		Etc2A = unchecked(9),

		/// <summary>
		/// ( 9) ETC2 RGBA8                     <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ETC2A1")]
		[NativeName(NativeNameType.Value, "10")]
		Etc2A1 = unchecked(10),

		/// <summary>
		/// (10) ETC2 RGB8A1                    <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_PTC12")]
		[NativeName(NativeNameType.Value, "11")]
		Ptc12 = unchecked(11),

		/// <summary>
		/// (11) PVRTC1 RGB 2BPP                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_PTC14")]
		[NativeName(NativeNameType.Value, "12")]
		Ptc14 = unchecked(12),

		/// <summary>
		/// (12) PVRTC1 RGB 4BPP                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_PTC12A")]
		[NativeName(NativeNameType.Value, "13")]
		Ptc12A = unchecked(13),

		/// <summary>
		/// (13) PVRTC1 RGBA 2BPP               <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_PTC14A")]
		[NativeName(NativeNameType.Value, "14")]
		Ptc14A = unchecked(14),

		/// <summary>
		/// (14) PVRTC1 RGBA 4BPP               <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_PTC22")]
		[NativeName(NativeNameType.Value, "15")]
		Ptc22 = unchecked(15),

		/// <summary>
		/// (15) PVRTC2 RGBA 2BPP               <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_PTC24")]
		[NativeName(NativeNameType.Value, "16")]
		Ptc24 = unchecked(16),

		/// <summary>
		/// (16) PVRTC2 RGBA 4BPP               <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ATC")]
		[NativeName(NativeNameType.Value, "17")]
		Atc = unchecked(17),

		/// <summary>
		/// (17) ATC RGB 4BPP                   <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ATCE")]
		[NativeName(NativeNameType.Value, "18")]
		Atce = unchecked(18),

		/// <summary>
		/// (18) ATCE RGBA 8 BPP explicit alpha <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ATCI")]
		[NativeName(NativeNameType.Value, "19")]
		Atci = unchecked(19),

		/// <summary>
		/// (19) ATCI RGBA 8 BPP interpolated alpha <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC4X4")]
		[NativeName(NativeNameType.Value, "20")]
		Astc4X4 = unchecked(20),

		/// <summary>
		/// (20) ASTC 4x4 8.0 BPP               <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC5X4")]
		[NativeName(NativeNameType.Value, "21")]
		Astc5X4 = unchecked(21),

		/// <summary>
		/// (21) ASTC 5x4 6.40 BPP              <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC5X5")]
		[NativeName(NativeNameType.Value, "22")]
		Astc5X5 = unchecked(22),

		/// <summary>
		/// (22) ASTC 5x5 5.12 BPP              <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC6X5")]
		[NativeName(NativeNameType.Value, "23")]
		Astc6X5 = unchecked(23),

		/// <summary>
		/// (23) ASTC 6x5 4.27 BPP              <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC6X6")]
		[NativeName(NativeNameType.Value, "24")]
		Astc6X6 = unchecked(24),

		/// <summary>
		/// (24) ASTC 6x6 3.56 BPP              <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC8X5")]
		[NativeName(NativeNameType.Value, "25")]
		Astc8X5 = unchecked(25),

		/// <summary>
		/// (25) ASTC 8x5 3.20 BPP              <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC8X6")]
		[NativeName(NativeNameType.Value, "26")]
		Astc8X6 = unchecked(26),

		/// <summary>
		/// (26) ASTC 8x6 2.67 BPP              <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC8X8")]
		[NativeName(NativeNameType.Value, "27")]
		Astc8X8 = unchecked(27),

		/// <summary>
		/// (27) ASTC 8x8 2.00 BPP              <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC10X5")]
		[NativeName(NativeNameType.Value, "28")]
		Astc10X5 = unchecked(28),

		/// <summary>
		/// (28) ASTC 10x5 2.56 BPP             <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC10X6")]
		[NativeName(NativeNameType.Value, "29")]
		Astc10X6 = unchecked(29),

		/// <summary>
		/// (29) ASTC 10x6 2.13 BPP             <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC10X8")]
		[NativeName(NativeNameType.Value, "30")]
		Astc10X8 = unchecked(30),

		/// <summary>
		/// (30) ASTC 10x8 1.60 BPP             <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC10X10")]
		[NativeName(NativeNameType.Value, "31")]
		Astc10X10 = unchecked(31),

		/// <summary>
		/// (31) ASTC 10x10 1.28 BPP            <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC12X10")]
		[NativeName(NativeNameType.Value, "32")]
		Astc12X10 = unchecked(32),

		/// <summary>
		/// (32) ASTC 12x10 1.07 BPP            <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_ASTC12X12")]
		[NativeName(NativeNameType.Value, "33")]
		Astc12X12 = unchecked(33),

		/// <summary>
		/// (33) ASTC 12x12 0.89 BPP            <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_UNKNOWN")]
		[NativeName(NativeNameType.Value, "34")]
		Unknown = unchecked(34),

		/// <summary>
		/// (34) Compressed formats above.      <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R1")]
		[NativeName(NativeNameType.Value, "35")]
		R1 = unchecked(35),

		/// <summary>
		/// (35)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_A8")]
		[NativeName(NativeNameType.Value, "36")]
		A8 = unchecked(36),

		/// <summary>
		/// (36)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R8")]
		[NativeName(NativeNameType.Value, "37")]
		R8 = unchecked(37),

		/// <summary>
		/// (37)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R8I")]
		[NativeName(NativeNameType.Value, "38")]
		R8I = unchecked(38),

		/// <summary>
		/// (38)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R8U")]
		[NativeName(NativeNameType.Value, "39")]
		R8U = unchecked(39),

		/// <summary>
		/// (39)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R8S")]
		[NativeName(NativeNameType.Value, "40")]
		R8S = unchecked(40),

		/// <summary>
		/// (40)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R16")]
		[NativeName(NativeNameType.Value, "41")]
		R16 = unchecked(41),

		/// <summary>
		/// (41)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R16I")]
		[NativeName(NativeNameType.Value, "42")]
		R16I = unchecked(42),

		/// <summary>
		/// (42)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R16U")]
		[NativeName(NativeNameType.Value, "43")]
		R16U = unchecked(43),

		/// <summary>
		/// (43)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R16F")]
		[NativeName(NativeNameType.Value, "44")]
		R16F = unchecked(44),

		/// <summary>
		/// (44)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R16S")]
		[NativeName(NativeNameType.Value, "45")]
		R16S = unchecked(45),

		/// <summary>
		/// (45)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R32I")]
		[NativeName(NativeNameType.Value, "46")]
		R32I = unchecked(46),

		/// <summary>
		/// (46)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R32U")]
		[NativeName(NativeNameType.Value, "47")]
		R32U = unchecked(47),

		/// <summary>
		/// (47)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R32F")]
		[NativeName(NativeNameType.Value, "48")]
		R32F = unchecked(48),

		/// <summary>
		/// (48)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG8")]
		[NativeName(NativeNameType.Value, "49")]
		Rg8 = unchecked(49),

		/// <summary>
		/// (49)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG8I")]
		[NativeName(NativeNameType.Value, "50")]
		Rg8I = unchecked(50),

		/// <summary>
		/// (50)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG8U")]
		[NativeName(NativeNameType.Value, "51")]
		Rg8U = unchecked(51),

		/// <summary>
		/// (51)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG8S")]
		[NativeName(NativeNameType.Value, "52")]
		Rg8S = unchecked(52),

		/// <summary>
		/// (52)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG16")]
		[NativeName(NativeNameType.Value, "53")]
		Rg16 = unchecked(53),

		/// <summary>
		/// (53)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG16I")]
		[NativeName(NativeNameType.Value, "54")]
		Rg16I = unchecked(54),

		/// <summary>
		/// (54)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG16U")]
		[NativeName(NativeNameType.Value, "55")]
		Rg16U = unchecked(55),

		/// <summary>
		/// (55)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG16F")]
		[NativeName(NativeNameType.Value, "56")]
		Rg16F = unchecked(56),

		/// <summary>
		/// (56)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG16S")]
		[NativeName(NativeNameType.Value, "57")]
		Rg16S = unchecked(57),

		/// <summary>
		/// (57)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG32I")]
		[NativeName(NativeNameType.Value, "58")]
		Rg32I = unchecked(58),

		/// <summary>
		/// (58)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG32U")]
		[NativeName(NativeNameType.Value, "59")]
		Rg32U = unchecked(59),

		/// <summary>
		/// (59)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG32F")]
		[NativeName(NativeNameType.Value, "60")]
		Rg32F = unchecked(60),

		/// <summary>
		/// (60)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGB8")]
		[NativeName(NativeNameType.Value, "61")]
		Rgb8 = unchecked(61),

		/// <summary>
		/// (61)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGB8I")]
		[NativeName(NativeNameType.Value, "62")]
		Rgb8I = unchecked(62),

		/// <summary>
		/// (62)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGB8U")]
		[NativeName(NativeNameType.Value, "63")]
		Rgb8U = unchecked(63),

		/// <summary>
		/// (63)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGB8S")]
		[NativeName(NativeNameType.Value, "64")]
		Rgb8S = unchecked(64),

		/// <summary>
		/// (64)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGB9E5F")]
		[NativeName(NativeNameType.Value, "65")]
		Rgb9E5F = unchecked(65),

		/// <summary>
		/// (65)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_BGRA8")]
		[NativeName(NativeNameType.Value, "66")]
		Bgra8 = unchecked(66),

		/// <summary>
		/// (66)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA8")]
		[NativeName(NativeNameType.Value, "67")]
		Rgba8 = unchecked(67),

		/// <summary>
		/// (67)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA8I")]
		[NativeName(NativeNameType.Value, "68")]
		Rgba8I = unchecked(68),

		/// <summary>
		/// (68)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA8U")]
		[NativeName(NativeNameType.Value, "69")]
		Rgba8U = unchecked(69),

		/// <summary>
		/// (69)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA8S")]
		[NativeName(NativeNameType.Value, "70")]
		Rgba8S = unchecked(70),

		/// <summary>
		/// (70)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA16")]
		[NativeName(NativeNameType.Value, "71")]
		Rgba16 = unchecked(71),

		/// <summary>
		/// (71)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA16I")]
		[NativeName(NativeNameType.Value, "72")]
		Rgba16I = unchecked(72),

		/// <summary>
		/// (72)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA16U")]
		[NativeName(NativeNameType.Value, "73")]
		Rgba16U = unchecked(73),

		/// <summary>
		/// (73)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA16F")]
		[NativeName(NativeNameType.Value, "74")]
		Rgba16F = unchecked(74),

		/// <summary>
		/// (74)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA16S")]
		[NativeName(NativeNameType.Value, "75")]
		Rgba16S = unchecked(75),

		/// <summary>
		/// (75)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA32I")]
		[NativeName(NativeNameType.Value, "76")]
		Rgba32I = unchecked(76),

		/// <summary>
		/// (76)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA32U")]
		[NativeName(NativeNameType.Value, "77")]
		Rgba32U = unchecked(77),

		/// <summary>
		/// (77)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA32F")]
		[NativeName(NativeNameType.Value, "78")]
		Rgba32F = unchecked(78),

		/// <summary>
		/// (78)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_B5G6R5")]
		[NativeName(NativeNameType.Value, "79")]
		B5G6R5 = unchecked(79),

		/// <summary>
		/// (79)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_R5G6B5")]
		[NativeName(NativeNameType.Value, "80")]
		R5G6B5 = unchecked(80),

		/// <summary>
		/// (80)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_BGRA4")]
		[NativeName(NativeNameType.Value, "81")]
		Bgra4 = unchecked(81),

		/// <summary>
		/// (81)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGBA4")]
		[NativeName(NativeNameType.Value, "82")]
		Rgba4 = unchecked(82),

		/// <summary>
		/// (82)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_BGR5A1")]
		[NativeName(NativeNameType.Value, "83")]
		Bgr5A1 = unchecked(83),

		/// <summary>
		/// (83)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGB5A1")]
		[NativeName(NativeNameType.Value, "84")]
		Rgb5A1 = unchecked(84),

		/// <summary>
		/// (84)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RGB10A2")]
		[NativeName(NativeNameType.Value, "85")]
		Rgb10A2 = unchecked(85),

		/// <summary>
		/// (85)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_RG11B10F")]
		[NativeName(NativeNameType.Value, "86")]
		Rg11B10F = unchecked(86),

		/// <summary>
		/// (86)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_UNKNOWNDEPTH")]
		[NativeName(NativeNameType.Value, "87")]
		Unknowndepth = unchecked(87),

		/// <summary>
		/// (87) Depth formats below.           <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_D16")]
		[NativeName(NativeNameType.Value, "88")]
		D16 = unchecked(88),

		/// <summary>
		/// (88)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_D24")]
		[NativeName(NativeNameType.Value, "89")]
		D24 = unchecked(89),

		/// <summary>
		/// (89)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_D24S8")]
		[NativeName(NativeNameType.Value, "90")]
		D24S8 = unchecked(90),

		/// <summary>
		/// (90)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_D32")]
		[NativeName(NativeNameType.Value, "91")]
		D32 = unchecked(91),

		/// <summary>
		/// (91)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_D16F")]
		[NativeName(NativeNameType.Value, "92")]
		D16F = unchecked(92),

		/// <summary>
		/// (92)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_D24F")]
		[NativeName(NativeNameType.Value, "93")]
		D24F = unchecked(93),

		/// <summary>
		/// (93)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_D32F")]
		[NativeName(NativeNameType.Value, "94")]
		D32F = unchecked(94),

		/// <summary>
		/// (94)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_D0S8")]
		[NativeName(NativeNameType.Value, "95")]
		D0S8 = unchecked(95),

		/// <summary>
		/// (95)                                <br/>
		/// </summary>
		[NativeName(NativeNameType.EnumItem, "BGFX_TEXTURE_FORMAT_COUNT")]
		[NativeName(NativeNameType.Value, "96")]
		Count = unchecked(96),
	}
}