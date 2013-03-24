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

        public void SetCount(uint value)
        {
            list = Activator.CreateInstance(ActualType, (int) value);
        }

        public void Add(object value)
        {
            addMethod.Invoke(list, new[] { value });
        }

        public override object End()
        {
            return list;
        }
    }
}
