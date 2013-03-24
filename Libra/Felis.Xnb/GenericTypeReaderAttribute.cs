#region Using

using System;

#endregion

namespace Felis.Xnb
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GenericTypeReaderAttribute : Attribute
    {
        public string GenericTargetType { get; private set; }

        public string GenericReaderName { get; private set; }

        public GenericTypeReaderAttribute(string genericTargetType, string genericReaderName)
        {
            if (genericTargetType == null) throw new ArgumentNullException("genericTargetType");
            if (genericReaderName == null) throw new ArgumentNullException("genericReaderName");

            GenericTargetType = genericTargetType;
            GenericReaderName = genericReaderName;
        }
    }
}
