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
        private readonly ConcurrentDictionary<Type, Type[]> _typeTypeMaps = new ConcurrentDictionary<Type, Type[]>();

        /// <summary>
        /// Composes a single <typeparamref name="T"/> object from the objects within the provided array.
        /// </summary>
        /// <typeparam name="T">The type of object to compose.</typeparam>
        /// <param name="objects">An array of objects containing the objects the composed object depends upon.
        /// The type of the first element must match <typeparamref name="T"/>.</param>
        /// <returns>A fully composed object or null if the first element is null.</returns>
        public T Compose<T> (Object[] objects)
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            using (var enumerator = ((IEnumerable<Object>)objects).GetEnumerator())
            {
                T ret = (T)ComposeInternal(enumerator);
                return ret;
            }
        }

        /// <summary>
        /// Composes a single <typeparamref name="T"/> object from the objects provided by the enumerator.
        /// </summary>
        /// <typeparam name="T">The type of object to compose.</typeparam>
        /// <param name="objects">An enumerator of objects containing the objects the composed object depends upon.
        /// The type of the first element must match <typeparamref name="T"/>.</param>
        /// <returns>A fully composed object or null if the first element is null.</returns>
        public T Compose<T>(IEnumerator<Object> objects)
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            return (T) ComposeInternal(objects);
        }

        /// <summary>
        /// Composes a single object from the objects provided by the enumerator.
        /// </summary>
        /// <param name="objects">An enumerator of objects containing the objects the composed object depends upon.</param>
        /// <returns>A fully composed object with the same type as the first element of the enumerator.</returns>
        public Object Compose(IEnumerator<Object> objects)
        {
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

        /// <summary>
        /// Recursively flattens a type into an array of it's composable dependencies.
        /// </summary>
        /// <typeparam name="T">The type to flatten.</typeparam>
        /// <returns>An array of all the composable children of the type.</returns>
        public Type[] FlattenComposableType<T>()
        {
            return FlattenComposableTypeInternal(typeof(T)).ToArray();
        }

        /// <summary>
        /// Recursively flattens a type into an array of it's composable dependencies.
        /// </summary>
        /// <param name="type">The type to flatten.</param>
        /// <returns>An array of all the composable children of the type.</returns>
        public Type[] FlattenComposableType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return FlattenComposableTypeInternal(type).ToArray();
        }

        private Type[] FlattenComposableTypeInternal(Type type)
        {
            if (_typeTypeMaps.TryGetValue(type, out Type[] types))
                return types;

            var properties = GetComposableProperties(type);
            List<Type> list = new List<Type> { type };
            foreach (var property in properties)
            {
                list.AddRange(FlattenComposableTypeInternal(property.PropertyType));
            }
            types = list.ToArray();
            _typeTypeMaps.TryAdd(type, types);

            return types;
        }
    }
}
