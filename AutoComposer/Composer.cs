using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoComposer
{
    public class Composer
    {
        private readonly ConcurrentDictionary<Type, PropertyInfo[]> _typePropertyMaps = new ConcurrentDictionary<Type, PropertyInfo[]>();

        public T Compose<T> (Object[] objects) where T : class
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            var enumerator = ((IEnumerable<Object>) objects).GetEnumerator();
            T ret = (T) Compose(enumerator);
            enumerator.Dispose();
            return ret;
        }

        public T Compose<T>(IEnumerator<Object> objects) where T : class
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            return (T) Compose(objects);
        }

        private Object Compose(IEnumerator<Object> objects)
        {
            if (!objects.MoveNext())
                return null;
            Object top = objects.Current;
            if (top == null)
                return null;
            Type topType = top.GetType();
            
            PropertyInfo[] properties = GetComposableProperties(topType);
            Int32 offset = 0;
            while (offset < properties.Length)
            {
                Object obj = Compose(objects);
                if (obj == null)
                    throw new ArgumentException($"No object to compose for type {topType}");

                properties[offset].SetValue(top, obj);
                offset++;
            }
            return top;
        }

        private PropertyInfo[] GetComposableProperties(Type type)
        {
            if (_typePropertyMaps.TryGetValue(type, out PropertyInfo[] properties))
                return properties;

            var assignableProperties = type.GetRuntimeProperties()
                .Where(p => p.GetCustomAttribute<ComposableAttribute>() != null)
                .OrderBy(p => p.GetCustomAttribute<ComposableAttribute>().Order);
            properties = assignableProperties.ToArray();
            _typePropertyMaps.TryAdd(type, properties);
            return properties;
        }
    }
}
