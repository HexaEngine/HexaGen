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
	/// To be documented.
	/// </summary>
	[NativeName(NativeNameType.StructOrClass, "bgfx_allocator_vtbl_s")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct BgfxAllocatorVtbl
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "realloc")]
		[NativeName(NativeNameType.Type, "void* (*)(bgfx_allocator_interface_t* _this, void* _ptr, size_t _size, size_t _align, const char* _file, uint32_t _line)*")]
		public unsafe void* Realloc;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe BgfxAllocatorVtbl(delegate*<BgfxAllocatorInterface*, void*, ulong, ulong, byte*, uint, void*> realloc = default)
		{
			Realloc = (void*)realloc;
		}


	}

}