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

namespace Hexa.NET.ImGuiNET
{
	/// <summary>
	/// To be documented.
	/// </summary>
	[NativeName(NativeNameType.Typedef, "ImTextureID")]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly partial struct ImTextureID : IEquatable<ImTextureID>
	{
		public ImTextureID(nint handle) { Handle = handle; }
		public nint Handle { get; }
		public bool IsNull => Handle == 0;
		public static ImTextureID Null => new ImTextureID(0);
		public static implicit operator ImTextureID(nint handle) => new ImTextureID(handle);
		public static bool operator ==(ImTextureID left, ImTextureID right) => left.Handle == right.Handle;
		public static bool operator !=(ImTextureID left, ImTextureID right) => left.Handle != right.Handle;
		public static bool operator ==(ImTextureID left, nint right) => left.Handle == right;
		public static bool operator !=(ImTextureID left, nint right) => left.Handle != right;
		public bool Equals(ImTextureID other) => Handle == other.Handle;
		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is ImTextureID handle && Equals(handle);
		/// <inheritdoc/>
		public override int GetHashCode() => Handle.GetHashCode();
		private string DebuggerDisplay => string.Format("ImTextureID [0x{0}]", Handle.ToString("X"));
	}

	/// <summary>
	/// To be documented.
	/// </summary>
	[NativeName(NativeNameType.Typedef, "ImBitArrayPtr")]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly partial struct ImBitArrayPtr : IEquatable<ImBitArrayPtr>
	{
		public ImBitArrayPtr(nint handle) { Handle = handle; }
		public nint Handle { get; }
		public bool IsNull => Handle == 0;
		public static ImBitArrayPtr Null => new ImBitArrayPtr(0);
		public static implicit operator ImBitArrayPtr(nint handle) => new ImBitArrayPtr(handle);
		public static bool operator ==(ImBitArrayPtr left, ImBitArrayPtr right) => left.Handle == right.Handle;
		public static bool operator !=(ImBitArrayPtr left, ImBitArrayPtr right) => left.Handle != right.Handle;
		public static bool operator ==(ImBitArrayPtr left, nint right) => left.Handle == right;
		public static bool operator !=(ImBitArrayPtr left, nint right) => left.Handle != right;
		public bool Equals(ImBitArrayPtr other) => Handle == other.Handle;
		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is ImBitArrayPtr handle && Equals(handle);
		/// <inheritdoc/>
		public override int GetHashCode() => Handle.GetHashCode();
		private string DebuggerDisplay => string.Format("ImBitArrayPtr [0x{0}]", Handle.ToString("X"));
	}

	/// <summary>
	/// To be documented.
	/// </summary>
	[NativeName(NativeNameType.Typedef, "ImFileHandle")]
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public readonly partial struct ImFileHandle : IEquatable<ImFileHandle>
	{
		public ImFileHandle(nint handle) { Handle = handle; }
		public nint Handle { get; }
		public bool IsNull => Handle == 0;
		public static ImFileHandle Null => new ImFileHandle(0);
		public static implicit operator ImFileHandle(nint handle) => new ImFileHandle(handle);
		public static bool operator ==(ImFileHandle left, ImFileHandle right) => left.Handle == right.Handle;
		public static bool operator !=(ImFileHandle left, ImFileHandle right) => left.Handle != right.Handle;
		public static bool operator ==(ImFileHandle left, nint right) => left.Handle == right;
		public static bool operator !=(ImFileHandle left, nint right) => left.Handle != right;
		public bool Equals(ImFileHandle other) => Handle == other.Handle;
		/// <inheritdoc/>
		public override bool Equals(object obj) => obj is ImFileHandle handle && Equals(handle);
		/// <inheritdoc/>
		public override int GetHashCode() => Handle.GetHashCode();
		private string DebuggerDisplay => string.Format("ImFileHandle [0x{0}]", Handle.ToString("X"));
	}

}