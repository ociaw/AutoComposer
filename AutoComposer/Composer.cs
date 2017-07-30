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
            
            List<PropertyInfo> unassignedProperties = GetComposableProperties(topType).ToList();

            while (unassignedProperties.Any())
            {
                Object obj = Compose(objects);
                if (obj == null)
                    throw new ArgumentException($"No object to compose for type {topType}");

                Type type = obj.GetType();
                PropertyInfo property = unassignedProperties.FirstOrDefault(p => p.PropertyType == type);
                if (property == null)
                    throw new ArgumentException($"Type {topType} has no property of type {type}.");
                unassignedProperties.Remove(property);
                property.SetValue(top, obj);
            }
            return top;
        }

        private PropertyInfo[] GetComposableProperties(Type type)
        {
            if (_typePropertyMaps.TryGetValue(type, out PropertyInfo[] properties))
                return properties;

            var assignableProperties = type.GetRuntimeProperties()
                .Where(p => p.GetCustomAttribute<ComposableAttribute>() != null)
                .OrderBy(p => p.GetCustomAttribute<AssignOrderAttribute>()?.Order ?? 0);
            properties = assignableProperties.ToArray();
            _typePropertyMaps.TryAdd(type, properties);
            return properties;
        }

        private Object Compose(Type t, Object[] objects)
        {
            Object composee = objects[0];

            Object[] toBeMapped = new Object[objects.Length - 1];
            Array.Copy(objects, 1, toBeMapped, 0, toBeMapped.Length);


            if (!_typePropertyMaps.ContainsKey(t))
                PopulateMap(t, toBeMapped);

            PropertyInfo[] properties = _typePropertyMaps[t];
            if (toBeMapped.Length != properties.Length)
                throw new ArgumentException("Incorrect number of objects.");

            for (Int32 i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                property.SetValue(composee, toBeMapped[i]);
            }
            return composee;
        }

        private void PopulateMap(Type t, Object[] objects)
        {
            var assignableProperties = t.GetRuntimeProperties()
                .Where(p => p.GetCustomAttribute<ComposableAttribute>() != null)
                .OrderBy(p => p.GetCustomAttribute<AssignOrderAttribute>().Order);
            List<PropertyInfo> unassignedProperties = assignableProperties.ToList();
            List<PropertyInfo> properties = new List<PropertyInfo>(unassignedProperties.Count);
            foreach (var obj in objects)
            {
                Type type = obj.GetType();
                PropertyInfo matched = unassignedProperties.FirstOrDefault(p => p.PropertyType == type);
                if (matched == null)
                    throw new ArgumentException("Object type not found.", nameof(objects));

                unassignedProperties.Remove(matched);
                properties.Add(matched);
            }
            _typePropertyMaps.TryAdd(t, properties.ToArray());
        }
    }
}
