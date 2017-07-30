using System;

namespace AutoComposer
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ComposableAttribute : Attribute
    {
    }
}
