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
using System.Runtime.InteropServices;
using HexaGen.Runtime;
using System.Numerics;

namespace Hexa.NET.PhysX
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly partial struct CUcontext : IEquatable<CUcontext>
	{
		public CUcontext(nint handle) { Handle = handle; }
		public nint Handle { get; }
		public bool IsNull => Handle == 0;
		public static CUcontext Null => new CUcontext(0);
		public static implicit operator CUcontext(nint handle) => new CUcontext(handle);
		public static bool operator ==(CUcontext left, CUcontext right) => left.Handle == right.Handle;
		public static bool operator !=(CUcontext left, CUcontext right) => left.Handle != right.Handle;
		public static bool operator ==(CUcontext left, nint right) => left.Handle == right;
		public static bool operator !=(CUcontext left, nint right) => left.Handle != right;
		public bool Equals(CUcontext other) => Handle == other.Handle;
		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is CUcontext handle && Equals(handle);
		/// <inheritdoc/>
		public override int GetHashCode() => Handle.GetHashCode();
		private string DebuggerDisplay => string.Format("CUcontext [0x{0}]", Handle.ToString("X"));
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly partial struct CUmodule : IEquatable<CUmodule>
	{
		public CUmodule(nint handle) { Handle = handle; }
		public nint Handle { get; }
		public bool IsNull => Handle == 0;
		public static CUmodule Null => new CUmodule(0);
		public static implicit operator CUmodule(nint handle) => new CUmodule(handle);
		public static bool operator ==(CUmodule left, CUmodule right) => left.Handle == right.Handle;
		public static bool operator !=(CUmodule left, CUmodule right) => left.Handle != right.Handle;
		public static bool operator ==(CUmodule left, nint right) => left.Handle == right;
		public static bool operator !=(CUmodule left, nint right) => left.Handle != right;
		public bool Equals(CUmodule other) => Handle == other.Handle;
		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is CUmodule handle && Equals(handle);
		/// <inheritdoc/>
		public override int GetHashCode() => Handle.GetHashCode();
		private string DebuggerDisplay => string.Format("CUmodule [0x{0}]", Handle.ToString("X"));
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly partial struct CUfunction : IEquatable<CUfunction>
	{
		public CUfunction(nint handle) { Handle = handle; }
		public nint Handle { get; }
		public bool IsNull => Handle == 0;
		public static CUfunction Null => new CUfunction(0);
		public static implicit operator CUfunction(nint handle) => new CUfunction(handle);
		public static bool operator ==(CUfunction left, CUfunction right) => left.Handle == right.Handle;
		public static bool operator !=(CUfunction left, CUfunction right) => left.Handle != right.Handle;
		public static bool operator ==(CUfunction left, nint right) => left.Handle == right;
		public static bool operator !=(CUfunction left, nint right) => left.Handle != right;
		public bool Equals(CUfunction other) => Handle == other.Handle;
		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is CUfunction handle && Equals(handle);
		/// <inheritdoc/>
		public override int GetHashCode() => Handle.GetHashCode();
		private string DebuggerDisplay => string.Format("CUfunction [0x{0}]", Handle.ToString("X"));
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly partial struct CUstream : IEquatable<CUstream>
	{
		public CUstream(nint handle) { Handle = handle; }
		public nint Handle { get; }
		public bool IsNull => Handle == 0;
		public static CUstream Null => new CUstream(0);
		public static implicit operator CUstream(nint handle) => new CUstream(handle);
		public static bool operator ==(CUstream left, CUstream right) => left.Handle == right.Handle;
		public static bool operator !=(CUstream left, CUstream right) => left.Handle != right.Handle;
		public static bool operator ==(CUstream left, nint right) => left.Handle == right;
		public static bool operator !=(CUstream left, nint right) => left.Handle != right;
		public bool Equals(CUstream other) => Handle == other.Handle;
		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is CUstream handle && Equals(handle);
		/// <inheritdoc/>
		public override int GetHashCode() => Handle.GetHashCode();
		private string DebuggerDisplay => string.Format("CUstream [0x{0}]", Handle.ToString("X"));
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly partial struct CUevent : IEquatable<CUevent>
	{
		public CUevent(nint handle) { Handle = handle; }
		public nint Handle { get; }
		public bool IsNull => Handle == 0;
		public static CUevent Null => new CUevent(0);
		public static implicit operator CUevent(nint handle) => new CUevent(handle);
		public static bool operator ==(CUevent left, CUevent right) => left.Handle == right.Handle;
		public static bool operator !=(CUevent left, CUevent right) => left.Handle != right.Handle;
		public static bool operator ==(CUevent left, nint right) => left.Handle == right;
		public static bool operator !=(CUevent left, nint right) => left.Handle != right;
		public bool Equals(CUevent other) => Handle == other.Handle;
		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is CUevent handle && Equals(handle);
		/// <inheritdoc/>
		public override int GetHashCode() => Handle.GetHashCode();
		private string DebuggerDisplay => string.Format("CUevent [0x{0}]", Handle.ToString("X"));
	}

	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly partial struct CUgraphicsResource : IEquatable<CUgraphicsResource>
	{
		public CUgraphicsResource(nint handle) { Handle = handle; }
		public nint Handle { get; }
		public bool IsNull => Handle == 0;
		public static CUgraphicsResource Null => new CUgraphicsResource(0);
		public static implicit operator CUgraphicsResource(nint handle) => new CUgraphicsResource(handle);
		public static bool operator ==(CUgraphicsResource left, CUgraphicsResource right) => left.Handle == right.Handle;
		public static bool operator !=(CUgraphicsResource left, CUgraphicsResource right) => left.Handle != right.Handle;
		public static bool operator ==(CUgraphicsResource left, nint right) => left.Handle == right;
		public static bool operator !=(CUgraphicsResource left, nint right) => left.Handle != right;
		public bool Equals(CUgraphicsResource other) => Handle == other.Handle;
		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is CUgraphicsResource handle && Equals(handle);
		/// <inheritdoc/>
		public override int GetHashCode() => Handle.GetHashCode();
		private string DebuggerDisplay => string.Format("CUgraphicsResource [0x{0}]", Handle.ToString("X"));
	}

}