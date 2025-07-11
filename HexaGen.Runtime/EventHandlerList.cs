namespace HexaGen.Runtime
{
    using System;
    using System.Collections;

    internal class EventHandlerList<T> : IEnumerable<T> where T : Delegate
    {
        private readonly List<T> delegates = [];
#if NET9_0_OR_GREATER
        private readonly Lock _lock = new();
#else
        private readonly object _lock = new();
#endif

        public void Add(T value)
        {
            lock (_lock)
            {
                delegates.Add(value);
            }
        }

        public void Remove(T value)
        {
            lock (_lock)
            {
                delegates.Remove(value);
            }
        }

        public void Invoke<TUserdata>(TUserdata userdata, Func<T, TUserdata, bool> action)
        {
            foreach (var item in delegates)
            {
                if (action.Invoke(item, userdata))
                {
                    break;
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return delegates.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}