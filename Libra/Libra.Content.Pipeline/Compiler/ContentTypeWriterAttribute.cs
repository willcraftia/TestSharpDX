#region Using

using System;

#endregion

namespace Libra.Content.Pipeline.Compiler
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ContentTypeWriterAttribute : Attribute
    {
    }
}
