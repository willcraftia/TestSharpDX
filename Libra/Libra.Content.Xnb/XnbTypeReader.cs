#region Using

using System;

#endregion

namespace Libra.Content.Xnb
{
    public abstract class XnbTypeReader
    {
        public Type TargetType { get; private set; }

        internal bool Initialized { get; private set; }

        public virtual bool CanDeserializeIntoExistingObject
        {
            get { return false; }
        }

        protected XnbTypeReader(Type targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            TargetType = targetType;
        }

        protected internal virtual void Initialize(XnbTypeReaderManager manager)
        {
            Initialized = true;
        }

        protected internal abstract object Read(XnbReader input, object existingInstance);
    }

    public abstract class XnbTypeReader<T> : XnbTypeReader
    {
        protected XnbTypeReader()
            : base(typeof(T))
        {
        }

        protected internal override object Read(XnbReader input, object existingInstance)
        {
            return (T) Read(input, (T) existingInstance);
        }

        protected internal abstract T Read(XnbReader input, T existingInstance);
    }
}
