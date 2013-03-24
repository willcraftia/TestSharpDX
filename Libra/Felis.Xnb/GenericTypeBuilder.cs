#region Using

using System;
using System.Collections.Generic;

#endregion

namespace Felis.Xnb
{
    public abstract class GenericTypeBuilder : TypeBuilder
    {
        string targetType;

        IList<string> genericArguments;

        Type actualType;

        Type[] actualGenericArguments;

        public override string TargetType
        {
            get { return targetType; }
        }

        public IList<string> GenericArguments
        {
            get { return genericArguments; }
        }

        public override Type ActualType
        {
            get { return actualType; }
        }

        public Type[] ActualGenericArguments
        {
            get { return actualGenericArguments;}
        }

        protected internal virtual void Specialize(string targetType, IList<string> genericArguments,
            Type actualType, Type[] actualGenericArguments)
        {
            this.targetType = targetType;
            this.genericArguments = genericArguments;
            this.actualType = actualType;
            this.actualGenericArguments = actualGenericArguments;
        }
    }
}
