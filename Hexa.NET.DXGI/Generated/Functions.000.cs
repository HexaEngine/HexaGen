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
using HexaGen.Runtime.COM;

namespace Hexa.NET.DXGI
{
	public unsafe partial class DXGI
	{
		internal const string LibName = "dxgi";

		/// <summary>
		/// To be documented.
		/// </summary>
		[LibraryImport(LibName, EntryPoint = "CreateDXGIFactory")]
		[UnmanagedCallConv(CallConvs = new Type[] {typeof(System.Runtime.CompilerServices.CallConvCdecl)})]
		internal static partial int CreateDXGIFactoryNative(Guid* riid, void** ppFactory);

		/// <summary>/// To be documented./// </summary>		public static int CreateDXGIFactory(Guid* riid, void** ppFactory) 
		{
			int ret = CreateDXGIFactoryNative(riid, ppFactory);
			return ret;
		}

		/// <summary>/// To be documented./// </summary>		public static int CreateDXGIFactory<T>(out ComPtr<T> ppFactory) where T : unmanaged, IComObject, IComObject<T>
		{
			ppFactory = default;
			int ret = CreateDXGIFactoryNative((Guid*)(ComUtils.GuidPtrOf<T>()), (void**)ppFactory.GetAddressOf());
			return ret;
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[LibraryImport(LibName, EntryPoint = "CreateDXGIFactory1")]
		[UnmanagedCallConv(CallConvs = new Type[] {typeof(System.Runtime.CompilerServices.CallConvCdecl)})]
		internal static partial int CreateDXGIFactory1Native(Guid* riid, void** ppFactory);

		/// <summary>/// To be documented./// </summary>		public static int CreateDXGIFactory1(Guid* riid, void** ppFactory) 
		{
			int ret = CreateDXGIFactory1Native(riid, ppFactory);
			return ret;
		}

		/// <summary>/// To be documented./// </summary>		public static int CreateDXGIFactory1<T>(out ComPtr<T> ppFactory) where T : unmanaged, IComObject, IComObject<T>
		{
			ppFactory = default;
			int ret = CreateDXGIFactory1Native((Guid*)(ComUtils.GuidPtrOf<T>()), (void**)ppFactory.GetAddressOf());
			return ret;
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[LibraryImport(LibName, EntryPoint = "CreateDXGIFactory2")]
		[UnmanagedCallConv(CallConvs = new Type[] {typeof(System.Runtime.CompilerServices.CallConvCdecl)})]
		internal static partial int CreateDXGIFactory2Native(uint flags, Guid* riid, void** ppFactory);

		/// <summary>/// To be documented./// </summary>		public static int CreateDXGIFactory2(uint flags, Guid* riid, void** ppFactory) 
		{
			int ret = CreateDXGIFactory2Native(flags, riid, ppFactory);
			return ret;
		}

		/// <summary>/// To be documented./// </summary>		public static int CreateDXGIFactory2(uint flags, ref Guid riid, void** ppFactory) 
		{
			fixed (Guid* priid = &riid)
			{
				int ret = CreateDXGIFactory2Native(flags, (Guid*)priid, ppFactory);
				return ret;
			}
		}

		/// <summary>/// To be documented./// </summary>		public static int CreateDXGIFactory2<T>(uint flags, out ComPtr<T> ppFactory) where T : unmanaged, IComObject, IComObject<T>
		{
			ppFactory = default;
			int ret = CreateDXGIFactory2Native(flags, (Guid*)(ComUtils.GuidPtrOf<T>()), (void**)ppFactory.GetAddressOf());
			return ret;
		}

		/// <summary>/// To be documented./// </summary>		public static int CreateDXGIFactory2<T>(uint flags, ref Guid riid, out ComPtr<T> ppFactory) where T : unmanaged, IComObject, IComObject<T>
		{
			fixed (Guid* priid = &riid)
			{
				ppFactory = default;
				int ret = CreateDXGIFactory2Native(flags, (Guid*)priid, (void**)ppFactory.GetAddressOf());
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[LibraryImport(LibName, EntryPoint = "DXGIGetDebugInterface1")]
		[UnmanagedCallConv(CallConvs = new Type[] {typeof(System.Runtime.CompilerServices.CallConvCdecl)})]
		internal static partial int DXGIGetDebugInterface1Native(uint flags, Guid* riid, void** pDebug);

		/// <summary>/// To be documented./// </summary>		public static int DXGIGetDebugInterface1(uint flags, Guid* riid, void** pDebug) 
		{
			int ret = DXGIGetDebugInterface1Native(flags, riid, pDebug);
			return ret;
		}

		/// <summary>/// To be documented./// </summary>		public static int DXGIGetDebugInterface1(uint flags, ref Guid riid, void** pDebug) 
		{
			fixed (Guid* priid = &riid)
			{
				int ret = DXGIGetDebugInterface1Native(flags, (Guid*)priid, pDebug);
				return ret;
			}
		}

		/// <summary>/// To be documented./// </summary>		public static int DXGIGetDebugInterface1<T>(uint flags, out ComPtr<T> pDebug) where T : unmanaged, IComObject, IComObject<T>
		{
			pDebug = default;
			int ret = DXGIGetDebugInterface1Native(flags, (Guid*)(ComUtils.GuidPtrOf<T>()), (void**)pDebug.GetAddressOf());
			return ret;
		}

		/// <summary>/// To be documented./// </summary>		public static int DXGIGetDebugInterface1<T>(uint flags, ref Guid riid, out ComPtr<T> pDebug) where T : unmanaged, IComObject, IComObject<T>
		{
			fixed (Guid* priid = &riid)
			{
				pDebug = default;
				int ret = DXGIGetDebugInterface1Native(flags, (Guid*)priid, (void**)pDebug.GetAddressOf());
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[LibraryImport(LibName, EntryPoint = "DXGIDeclareAdapterRemovalSupport")]
		[UnmanagedCallConv(CallConvs = new Type[] {typeof(System.Runtime.CompilerServices.CallConvCdecl)})]
		internal static partial int DXGIDeclareAdapterRemovalSupportNative();

		/// <summary>/// To be documented./// </summary>		public static int DXGIDeclareAdapterRemovalSupport() 
		{
			int ret = DXGIDeclareAdapterRemovalSupportNative();
			return ret;
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[LibraryImport(LibName, EntryPoint = "DXGIDisableVBlankVirtualization")]
		[UnmanagedCallConv(CallConvs = new Type[] {typeof(System.Runtime.CompilerServices.CallConvCdecl)})]
		internal static partial int DXGIDisableVBlankVirtualizationNative();

		/// <summary>/// To be documented./// </summary>		public static int DXGIDisableVBlankVirtualization() 
		{
			int ret = DXGIDisableVBlankVirtualizationNative();
			return ret;
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[LibraryImport(LibName, EntryPoint = "DXGIGetDebugInterface")]
		[UnmanagedCallConv(CallConvs = new Type[] {typeof(System.Runtime.CompilerServices.CallConvCdecl)})]
		internal static partial int DXGIGetDebugInterfaceNative(Guid* riid, void** ppDebug);

		/// <summary>/// To be documented./// </summary>		public static int DXGIGetDebugInterface(Guid* riid, void** ppDebug) 
		{
			int ret = DXGIGetDebugInterfaceNative(riid, ppDebug);
			return ret;
		}

		/// <summary>/// To be documented./// </summary>		public static int DXGIGetDebugInterface<T>(out ComPtr<T> ppDebug) where T : unmanaged, IComObject, IComObject<T>
		{
			ppDebug = default;
			int ret = DXGIGetDebugInterfaceNative((Guid*)(ComUtils.GuidPtrOf<T>()), (void**)ppDebug.GetAddressOf());
			return ret;
		}

	}
}
