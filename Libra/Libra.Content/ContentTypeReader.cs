#region Using

using System;

#endregion

namespace Libra.Content
{
    public abstract class ContentTypeReader
    {
        public Type TargetType { get; private set; }

        public virtual bool CanDeserializeIntoExistingObject
        {
            get { return false; }
        }

        internal bool Initialized { get; private set; }

        protected ContentTypeReader(Type targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            TargetType = targetType;
        }

        protected internal virtual void Initialize(ContentTypeReaderManager manager)
        {
            Initialized = true;
        }

        protected internal abstract object Read(ContentReader input, object existingInstance);
    }

    public abstract class ContentTypeReader<T> : ContentTypeReader
    {
        protected ContentTypeReader()
            : base(typeof(T))
        {
        }

        protected internal override object Read(ContentReader input, object existingInstance)
        {
            return (T) Read(input, (T) existingInstance);
        }

        protected internal abstract T Read(ContentReader input, T existingInstance);
    }
}
