#region Using

using System;

#endregion

namespace Libra.Content.Xnb
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class XnbTypeReaderAttribute : Attribute
    {
        public string XnaTypeName { get; private set; }

        public XnbTypeReaderAttribute(string xnaTypeName)
        {
            if (xnaTypeName == null) throw new ArgumentNullException("xnaTypeName");

            XnaTypeName = xnaTypeName;
        }
    }
}
