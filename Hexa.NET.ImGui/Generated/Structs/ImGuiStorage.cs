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
	/// Helper: Key-&gt;Value storage<br/>
	/// Typically you don't have to worry about this since a storage is held within each Window.<br/>
	/// We use it to e.g. store collapse state for a tree (Int 01)<br/>
	/// This is optimized for efficient lookup (dichotomy into a contiguous buffer) and rare insertion (typically tied to user interactions aka max once a frame)<br/>
	/// You can use it as custom user storage for temporary values. Declare your own storage if, for example:<br/>
	/// - You want to manipulate the openclose state of a particular sub-tree in your interface (tree node uses Int 01 to store their state).<br/>
	/// - You want to store custom debug data easily without adding or editing structures in your code (probably not efficient, but convenient)<br/>
	/// Types are NOT stored, so it is up to you to make sure your Key don't collide with different types.<br/>
	/// </summary>
	[NativeName(NativeNameType.StructOrClass, "ImGuiStorage")]
	[StructLayout(LayoutKind.Sequential)]
	public partial struct ImGuiStorage
	{
		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Field, "Data")]
		[NativeName(NativeNameType.Type, "ImVector_ImGuiStoragePair")]
		public ImVectorImGuiStoragePair Data;


		/// <summary>
		/// To be documented.
		/// </summary>
		public unsafe ImGuiStorage(ImVectorImGuiStoragePair data = default)
		{
			Data = data;
		}


		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_BuildSortByKey")]
		[return: NativeName(NativeNameType.Type, "void")]
		public unsafe void BuildSortByKey()
		{
			fixed (ImGuiStorage* @this = &this)
			{
				ImGui.BuildSortByKeyNative(@this);
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_Clear")]
		[return: NativeName(NativeNameType.Type, "void")]
		public unsafe void Clear()
		{
			fixed (ImGuiStorage* @this = &this)
			{
				ImGui.ClearNative(@this);
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetBool")]
		[return: NativeName(NativeNameType.Type, "bool")]
		public unsafe bool GetBool([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key, [NativeName(NativeNameType.Param, "default_val")] [NativeName(NativeNameType.Type, "bool")] bool defaultVal)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				byte ret = ImGui.GetBoolNative(@this, key, defaultVal ? (byte)1 : (byte)0);
				return ret != 0;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetBool")]
		[return: NativeName(NativeNameType.Type, "bool")]
		public unsafe bool GetBool([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				byte ret = ImGui.GetBoolNative(@this, key, (byte)(0));
				return ret != 0;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetBoolRef")]
		[return: NativeName(NativeNameType.Type, "bool*")]
		public unsafe bool* GetBoolRef([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key, [NativeName(NativeNameType.Param, "default_val")] [NativeName(NativeNameType.Type, "bool")] bool defaultVal)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				bool* ret = ImGui.GetBoolRefNative(@this, key, defaultVal ? (byte)1 : (byte)0);
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetBoolRef")]
		[return: NativeName(NativeNameType.Type, "bool*")]
		public unsafe bool* GetBoolRef([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				bool* ret = ImGui.GetBoolRefNative(@this, key, (byte)(0));
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetFloat")]
		[return: NativeName(NativeNameType.Type, "float")]
		public unsafe float GetFloat([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key, [NativeName(NativeNameType.Param, "default_val")] [NativeName(NativeNameType.Type, "float")] float defaultVal)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				float ret = ImGui.GetFloatNative(@this, key, defaultVal);
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetFloat")]
		[return: NativeName(NativeNameType.Type, "float")]
		public unsafe float GetFloat([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				float ret = ImGui.GetFloatNative(@this, key, (float)(0.0f));
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetFloatRef")]
		[return: NativeName(NativeNameType.Type, "float*")]
		public unsafe float* GetFloatRef([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key, [NativeName(NativeNameType.Param, "default_val")] [NativeName(NativeNameType.Type, "float")] float defaultVal)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				float* ret = ImGui.GetFloatRefNative(@this, key, defaultVal);
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetFloatRef")]
		[return: NativeName(NativeNameType.Type, "float*")]
		public unsafe float* GetFloatRef([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				float* ret = ImGui.GetFloatRefNative(@this, key, (float)(0.0f));
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetInt")]
		[return: NativeName(NativeNameType.Type, "int")]
		public unsafe int GetInt([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key, [NativeName(NativeNameType.Param, "default_val")] [NativeName(NativeNameType.Type, "int")] int defaultVal)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				int ret = ImGui.GetIntNative(@this, key, defaultVal);
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetInt")]
		[return: NativeName(NativeNameType.Type, "int")]
		public unsafe int GetInt([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				int ret = ImGui.GetIntNative(@this, key, (int)(0));
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetIntRef")]
		[return: NativeName(NativeNameType.Type, "int*")]
		public unsafe int* GetIntRef([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key, [NativeName(NativeNameType.Param, "default_val")] [NativeName(NativeNameType.Type, "int")] int defaultVal)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				int* ret = ImGui.GetIntRefNative(@this, key, defaultVal);
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetIntRef")]
		[return: NativeName(NativeNameType.Type, "int*")]
		public unsafe int* GetIntRef([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				int* ret = ImGui.GetIntRefNative(@this, key, (int)(0));
				return ret;
			}
		}

		/// <summary>
		/// default_val is NULL<br/>
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetVoidPtr")]
		[return: NativeName(NativeNameType.Type, "void*")]
		public unsafe void* GetVoidPtr([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				void* ret = ImGui.GetVoidPtrNative(@this, key);
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetVoidPtrRef")]
		[return: NativeName(NativeNameType.Type, "void**")]
		public unsafe void** GetVoidPtrRef([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key, [NativeName(NativeNameType.Param, "default_val")] [NativeName(NativeNameType.Type, "void*")] void* defaultVal)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				void** ret = ImGui.GetVoidPtrRefNative(@this, key, defaultVal);
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_GetVoidPtrRef")]
		[return: NativeName(NativeNameType.Type, "void**")]
		public unsafe void** GetVoidPtrRef([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				void** ret = ImGui.GetVoidPtrRefNative(@this, key, (void*)(default));
				return ret;
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_SetAllInt")]
		[return: NativeName(NativeNameType.Type, "void")]
		public unsafe void SetAllInt([NativeName(NativeNameType.Param, "val")] [NativeName(NativeNameType.Type, "int")] int val)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				ImGui.SetAllIntNative(@this, val);
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_SetBool")]
		[return: NativeName(NativeNameType.Type, "void")]
		public unsafe void SetBool([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key, [NativeName(NativeNameType.Param, "val")] [NativeName(NativeNameType.Type, "bool")] bool val)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				ImGui.SetBoolNative(@this, key, val ? (byte)1 : (byte)0);
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_SetFloat")]
		[return: NativeName(NativeNameType.Type, "void")]
		public unsafe void SetFloat([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key, [NativeName(NativeNameType.Param, "val")] [NativeName(NativeNameType.Type, "float")] float val)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				ImGui.SetFloatNative(@this, key, val);
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_SetInt")]
		[return: NativeName(NativeNameType.Type, "void")]
		public unsafe void SetInt([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key, [NativeName(NativeNameType.Param, "val")] [NativeName(NativeNameType.Type, "int")] int val)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				ImGui.SetIntNative(@this, key, val);
			}
		}

		/// <summary>
		/// To be documented.
		/// </summary>
		[NativeName(NativeNameType.Func, "ImGuiStorage_SetVoidPtr")]
		[return: NativeName(NativeNameType.Type, "void")]
		public unsafe void SetVoidPtr([NativeName(NativeNameType.Param, "key")] [NativeName(NativeNameType.Type, "ImGuiID")] uint key, [NativeName(NativeNameType.Param, "val")] [NativeName(NativeNameType.Type, "void*")] void* val)
		{
			fixed (ImGuiStorage* @this = &this)
			{
				ImGui.SetVoidPtrNative(@this, key, val);
			}
		}

	}

}