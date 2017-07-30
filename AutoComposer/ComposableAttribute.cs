using System;

namespace AutoComposer
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ComposableAttribute : Attribute
    {
        public ComposableAttribute()
        {
            Order = 0;
        }

        public ComposableAttribute(Int32 order)
        {
            Order = order;
        }

        public Int32 Order { get; }
    }
}
