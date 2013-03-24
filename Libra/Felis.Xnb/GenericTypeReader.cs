#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Felis.Xnb
{
    public abstract class GenericTypeReader : TypeReader
    {
        string targetType;

        string readerName;

        IList<string> genericArguments;

        public override string TargetType
        {
            get { return targetType; }
        }

        public override string ReaderName
        {
            get { return readerName; }
        }

        public IList<string> GenericArguments
        {
            get { return genericArguments; }
        }

        protected internal void Specialize(string targetType, string readerName, IList<string> genericArguments)
        {
            this.targetType = targetType;
            this.readerName = readerName;
            this.genericArguments = genericArguments;
        }
    }
}
