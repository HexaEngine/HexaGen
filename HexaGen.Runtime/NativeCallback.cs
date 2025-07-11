namespace HexaGen.Runtime
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a native callback that can be passed to interop functions requiring a callback to C# code.
    /// Ensures that the callback remains valid beyond the current scope by holding a GCHandle.
    /// </summary>
    /// <typeparam name="T">The delegate type of the callback.</typeparam>
    public struct NativeCallback<T> : IDisposable, IEquatable<NativeCallback<T>> where T : Delegate
    {
        /// <summary>
        /// The managed delegate representing the callback.
        /// </summary>
        public T? Callback;

        /// <summary>
        /// The GCHandle that ensures the callback remains allocated in memory.
        /// </summary>
        public GCHandle Handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeCallback{T}"/> struct.
        /// </summary>
        /// <param name="callback">The delegate to be used as the callback.</param>
        public NativeCallback(T? callback)
        {
            Callback = callback;
            if (callback != null)
            {
                Handle = GCHandle.Alloc(callback);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the callback is null.
        /// </summary>
        public readonly bool IsNull => Callback == null;

        /// <summary>
        /// Gets a value indicating whether the GCHandle is allocated.
        /// </summary>
        public readonly bool IsAllocated => Handle.IsAllocated;

        /// <summary>
        /// Gets a value indicating whether the instance has been disposed.
        /// </summary>
        public readonly bool IsDisposed => Handle == default;

        /// <summary>
        /// Releases the GCHandle and clears the callback.
        /// </summary>
        public void Dispose()
        {
            if (Handle.IsAllocated)
            {
                Handle.Free();
                Handle = default;
                Callback = default!;
            }
        }

        /// <inheritdoc/>
        public override readonly bool Equals(object? obj)
        {
            return obj is NativeCallback<T> callback && Equals(callback);
        }

        /// <inheritdoc/>
        public readonly bool Equals(NativeCallback<T> other)
        {
            return ReferenceEquals(Callback, other.Callback);
        }

        /// <inheritdoc/>
        public override readonly int GetHashCode()
        {
            return Callback?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// Determines whether two <see cref="NativeCallback{T}"/> instances are equal.
        /// </summary>
        public static bool operator ==(NativeCallback<T> left, NativeCallback<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="NativeCallback{T}"/> instances are not equal.
        /// </summary>
        public static bool operator !=(NativeCallback<T> left, NativeCallback<T> right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Implicitly converts a <see cref="NativeCallback{T}"/> instance to its underlying delegate type.
        /// </summary>
        public static implicit operator T?(NativeCallback<T> callback) => callback.Callback;
    }
}