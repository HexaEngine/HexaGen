namespace HexaGen.Core.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class CollectionNormalizer
    {
        public static void Normalize<T>(T obj)
        {
            if (obj == null) return;

            var props = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p =>
                    p.CanRead && p.CanWrite &&
                    p.PropertyType != typeof(string) &&
                    typeof(IEnumerable).IsAssignableFrom(p.PropertyType)
                );

            foreach (var prop in props)
            {
                var current = prop.GetValue(obj);
                if (current == null)
                {
                    var type = prop.PropertyType;

                    // Handle common collection types
                    if (type.IsGenericType)
                    {
                        Type genericDef = type.GetGenericTypeDefinition();

                        if (genericDef == typeof(List<>) ||
                            genericDef == typeof(HashSet<>) ||
                            genericDef == typeof(Dictionary<,>))
                        {
                            try
                            {
                                var instance = Activator.CreateInstance(type);
                                prop.SetValue(obj, instance);
                            }
                            catch
                            {
                                // Ignore non-instantiable types
                            }
                        }
                    }
                }
            }
        }
    }
}