#region Using

using System;

#endregion

namespace Felis.Xnb
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GenericTypeBuilderAttribute : Attribute
    {
        public string GenericTargetType { get; private set; }

        public Type ActualGenericTypeDefinition { get; private set; }

        public GenericTypeBuilderAttribute(string genericTargetType, Type actualGenericTypeDefinition)
        {
            if (genericTargetType == null) throw new ArgumentNullException("genericTargetType");
            if (actualGenericTypeDefinition == null) throw new ArgumentNullException("actualGenericTypeDefinition");

            GenericTargetType = genericTargetType;
            ActualGenericTypeDefinition = actualGenericTypeDefinition;
        }
    }
}
