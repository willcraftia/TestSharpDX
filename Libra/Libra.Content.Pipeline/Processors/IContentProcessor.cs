#region Using

using System;

#endregion

namespace Libra.Content.Pipeline.Processors
{
    public interface IContentProcessor
    {
        Type InputType { get; }

        Type OutputType { get; }

        object Process(object input);
    }
}
