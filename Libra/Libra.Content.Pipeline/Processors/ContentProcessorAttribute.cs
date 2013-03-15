#region Using

using System;

#endregion

namespace Libra.Content.Pipeline.Processors
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ContentProcessorAttribute : Attribute
    {
    }
}
