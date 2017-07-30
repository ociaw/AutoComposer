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
            T ret = (T) ComposeInternal(enumerator);
            enumerator.Dispose();
            return ret;
        }

        public T Compose<T>(IEnumerator<Object> objects) where T : class
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            return (T) ComposeInternal(objects);
        }

        public Object Compose(Type type, IEnumerator<Object> objects)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            return ComposeInternal(objects);
        }

        private Object ComposeInternal(IEnumerator<Object> objects)
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
                Object obj = ComposeInternal(objects);
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

        public Type[] FlattenComposableType<T>()
        {
            return FlattenComposableTypeInternal(typeof(T)).ToArray();
        }

        public Type[] FlattenComposableType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return FlattenComposableTypeInternal(type).ToArray();
        }

        private List<Type> FlattenComposableTypeInternal(Type type)
        {
            var properties = GetComposableProperties(type);
            List<Type> types = new List<Type> { type };
            foreach (var property in properties)
            {
                types.AddRange(FlattenComposableTypeInternal(property.PropertyType));
            }
            return types;
        }
    }
}
