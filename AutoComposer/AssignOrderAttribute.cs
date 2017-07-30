using System;

namespace AutoComposer
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AssignOrderAttribute : Attribute
    {
        public AssignOrderAttribute(Int32 order)
        {
            Order = order;
        }

        public Int32 Order { get; }
    }
}
