#region Using

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace Felis.Xnb
{
    [GenericTypeBuilder("System.Collections.Generic.List", typeof(List<>))]
    public sealed class ListBuilder : GenericTypeBuilder
    {
        object list;

        MethodInfo addMethod;

        protected internal override void Specialize(
            string targetType, IList<string> genericArguments, Type actualType, Type[] actualGenericArguments)
        {
            addMethod = actualType.GetMethod("Add");

            base.Specialize(targetType, genericArguments, actualType, actualGenericArguments);
        }

        protected internal void SetCount(uint value)
        {
            list = Activator.CreateInstance(ActualType, (int) value);
        }

        protected internal void Add(object value)
        {
            addMethod.Invoke(list, new[] { value });
        }

        protected internal override void Begin(object deviceContext) { }

        protected internal override object End()
        {
            return list;
        }
    }
}
