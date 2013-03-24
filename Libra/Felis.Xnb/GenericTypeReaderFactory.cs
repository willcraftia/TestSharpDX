#region Using

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Felis.Xnb
{
    public abstract class GenericTypeReaderFactory
    {
        StringBuilder suffixBuilder;

        StringBuilder nameBuilder;

        public abstract string GenericTargetType { get; }

        public abstract string GenericReaderName { get; }

        public GenericTypeReaderFactory()
        {
            suffixBuilder = new StringBuilder(256);
            nameBuilder = new StringBuilder(384);
        }

        public GenericTypeReader CreateTypeReader(IList<string> genericArguments)
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

            nameBuilder.Length = 0;
            nameBuilder.Append(GenericReaderName);
            nameBuilder.Append(genericSuffix);
            var readerName = nameBuilder.ToString();

            var reader = CreateTypeReader();
            
            reader.Specialize(targetType, readerName, genericArguments);
            
            return reader;
        }

        protected abstract GenericTypeReader CreateTypeReader();
    }

    public sealed class GenericTypeReaderFactory<T> : GenericTypeReaderFactory where T : GenericTypeReader, new()
    {
        GenericTypeReaderAttribute attribute;

        public override string GenericTargetType
        {
            get { return attribute.GenericTargetType; }
        }

        public override string GenericReaderName
        {
            get { return attribute.GenericReaderName; }
        }

        public GenericTypeReaderFactory()
        {
            attribute = Attribute.GetCustomAttribute(typeof(T), typeof(GenericTypeReaderAttribute)) as GenericTypeReaderAttribute;
            if (attribute == null)
                throw new InvalidOperationException("GenericTypeReaderAttribute not specified: " + typeof(T));
        }

        protected override GenericTypeReader CreateTypeReader()
        {
            return new T();
        }
    }
}
