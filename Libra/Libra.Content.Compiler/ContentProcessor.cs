#region Using

using System;

#endregion

namespace Libra.Content.Compiler
{
    public abstract class ContentProcessor<TInput, TOutput> : IContentProcessor
    {
        public Type InputType
        {
            get { return typeof(TInput); }
        }

        public Type OutputType
        {
            get { return typeof(TOutput); }
        }

        public object Process(object input)
        {
            return Process((TInput) input);
        }

        protected abstract TOutput Process(TInput input);
    }
}
