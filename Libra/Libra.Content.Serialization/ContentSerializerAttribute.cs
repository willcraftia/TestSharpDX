#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Libra.Content.Serialization
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ContentSerializerAttribute : Attribute
    {
    }
}
