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
using HexaGen.Runtime.COM;

namespace Hexa.NET.DXC
{
	/// <summary>
	/// To be documented.
	/// </summary>
	[NativeName(NativeNameType.Delegate, "DxcCreateInstanceProc")]
	[return: NativeName(NativeNameType.Type, "HRESULT")]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate HResult DxcCreateInstanceProc([NativeName(NativeNameType.Param, "rclsid")] [NativeName(NativeNameType.Type, "const IID&")] Guid* rclsid, [NativeName(NativeNameType.Param, "riid")] [NativeName(NativeNameType.Type, "const IID&")] Guid* riid, [NativeName(NativeNameType.Param, "ppv")] [NativeName(NativeNameType.Type, "LPVOID*")] void** ppv);

	/// <summary>
	/// To be documented.
	/// </summary>
	[NativeName(NativeNameType.Delegate, "DxcCreateInstance2Proc")]
	[return: NativeName(NativeNameType.Type, "HRESULT")]
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public unsafe delegate HResult DxcCreateInstance2Proc([NativeName(NativeNameType.Param, "pMalloc")] [NativeName(NativeNameType.Type, "IMalloc*")] IMalloc* pMalloc, [NativeName(NativeNameType.Param, "rclsid")] [NativeName(NativeNameType.Type, "const IID&")] Guid* rclsid, [NativeName(NativeNameType.Param, "riid")] [NativeName(NativeNameType.Type, "const IID&")] Guid* riid, [NativeName(NativeNameType.Param, "ppv")] [NativeName(NativeNameType.Type, "LPVOID*")] void** ppv);

}