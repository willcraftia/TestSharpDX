#region Using

using System;

#endregion

namespace Libra.Content
{
    public abstract class ContentTypeReader
    {
        public Type TargetType { get; private set; }

        internal bool Initialized { get; private set; }

        protected ContentTypeReader(Type targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            TargetType = targetType;
        }

        protected internal abstract object Read(ContentReader input);

        protected virtual void Initialize(ContentTypeReaderManager manager) { }

        internal void InternalInitialize(ContentTypeReaderManager manager)
        {
            if (Initialized) return;

            Initialize(manager);

            Initialized = true;
        }
    }
}
