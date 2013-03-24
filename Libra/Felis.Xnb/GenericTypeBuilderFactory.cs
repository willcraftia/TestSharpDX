#region Using

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Felis.Xnb
{
    public abstract class GenericTypeBuilderFactory
    {
        StringBuilder suffixBuilder;

        StringBuilder nameBuilder;

        public abstract string GenericTargetType { get; }

        public abstract Type ActualGenericTypeDefinition { get; }

        public GenericTypeBuilderFactory()
        {
            suffixBuilder = new StringBuilder(256);
            nameBuilder = new StringBuilder(384);
        }

        public GenericTypeBuilder CreateTypeBuilder(TypeReaderManager manager, IList<string> genericArguments)
        {
            suffixBuilder.Length = 0;
            suffixBuilder.Append('`');
            suffixBuilder.Append(genericArguments.Count);
            suffixBuilder.Append("[[");

            for (int i = 0; i < genericArguments.Count; i++)
            {
                suffixBuilder.Append(genericArguments[i]);

                if (i != genericArguments.Count - 1)
                {
                    suffixBuilder.Append("],[");
                }
            }

            suffixBuilder.Append("]]");
            var genericSuffix = suffixBuilder.ToString();

            nameBuilder.Length = 0;
            nameBuilder.Append(GenericTargetType);
            nameBuilder.Append(genericSuffix);
            var targetType = nameBuilder.ToString();

            var typeBuilder = CreateTypeBuilder();
            
            var actualGenericArguments = new Type[genericArguments.Count];
            for (int i = 0; i < actualGenericArguments.Length; i++)
            {
                actualGenericArguments[i] = manager.ResolveActualType(genericArguments[i]);
            }

            var actualType = ActualGenericTypeDefinition.MakeGenericType(actualGenericArguments);

            typeBuilder.Specialize(targetType, genericArguments, actualType, actualGenericArguments);
            
            return typeBuilder;
        }

        protected abstract GenericTypeBuilder CreateTypeBuilder();
    }

    public sealed class GenericTypeBuilderFactory<T> : GenericTypeBuilderFactory where T : GenericTypeBuilder, new()
    {
        GenericTypeBuilderAttribute attribute;

        public override string GenericTargetType
        {
            get { return attribute.GenericTargetType; }
        }

        public override Type ActualGenericTypeDefinition
        {
            get { return attribute.ActualGenericTypeDefinition; }
        }

        public GenericTypeBuilderFactory()
        {
            attribute = Attribute.GetCustomAttribute(typeof(T), typeof(GenericTypeBuilderAttribute)) as GenericTypeBuilderAttribute;
            if (attribute == null)
                throw new InvalidOperationException("GenericTypeReaderAttribute not specified: " + typeof(T));
        }

        protected override GenericTypeBuilder CreateTypeBuilder()
        {
            return new T();
        }
    }
}
