#region Using

using System;

#endregion

namespace Felis.Xnb
{
    public abstract class TypeReader
    {
        public abstract string TargetType { get; }

        public abstract string ReaderName { get; }

        public virtual bool IsValueType
        {
            get { return false; }
        }

        public bool Initialized { get; private set; }

        protected TypeReader() { }

        protected internal virtual void Initialize(TypeReaderManager manager)
        {
            Initialized = true;
        }

        protected internal abstract object Read(ContentReader input);
    }
}
