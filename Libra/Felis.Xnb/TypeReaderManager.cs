#region Using

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

#endregion

namespace Felis.Xnb
{
    public sealed class TypeReaderManager
    {
        List<TypeBuilder> typeBuilders;

        List<GenericTypeBuilderFactory> genericTypeBuilderFactories;

        List<TypeReader> typeReaders;

        List<GenericTypeReaderFactory> genericTypeReaderFactories;

        public TypeReaderManager()
        {
            typeBuilders = new List<TypeBuilder>();
            genericTypeBuilderFactories = new List<GenericTypeBuilderFactory>();
            typeReaders = new List<TypeReader>();
            genericTypeReaderFactories = new List<GenericTypeReaderFactory>();
        }

        public void RegisterTypeBuilder<T>() where T : TypeBuilder, new()
        {
            typeBuilders.Add(new T());
        }

        public void RegisterGenericTypeBuilder<T>() where T : GenericTypeBuilder, new()
        {
            genericTypeBuilderFactories.Add(new GenericTypeBuilderFactory<T>());
        }

        public void RegisterTypeReader<T>() where T : TypeReader, new()
        {
            typeReaders.Add(new T());
        }

        public void RegisterGenericTypeReader<T>() where T : GenericTypeReader, new()
        {
            genericTypeReaderFactories.Add(new GenericTypeReaderFactory<T>());
        }

        public void RegisterStandardTypeReaders()
        {
            // Primitive types
            RegisterTypeReader<CharReader>();
            RegisterTypeReader<StringReader>();

            // System types
            RegisterGenericTypeReader<ListReader>();

            // Math types
            RegisterTypeReader<Vector3Reader>();
            RegisterTypeReader<RectangleReader>();
            RegisterTypeReader<MatrixReader>();

            // Graphics types
            RegisterTypeReader<Texture2DReader>();
            RegisterTypeReader<IndexBufferReader>();
            RegisterTypeReader<VertexBufferReader>();
            RegisterTypeReader<VertexDeclarationReader>();
            RegisterTypeReader<BasicEffectReader>();
            RegisterTypeReader<SpriteFontReader>();
            RegisterTypeReader<ModelReader>();

            // 読み込み型が明確となるリーダに対するビルダを同時に登録。
            RegisterGenericTypeBuilder<ListBuilder>();
        }

        public Type ResolveActualType(string targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            var wanted = StripAssemblyVersion(targetType);

            foreach (var builder in typeBuilders)
            {
                if (builder.TargetType == wanted)
                {
                    return builder.ActualType;
                }
            }

            string genericTargetType;
            List<string> genericArguments;

            if (SplitGenericTypeName(wanted, out genericTargetType, out genericArguments))
            {
                foreach (var factory in genericTypeBuilderFactories)
                {
                    if (factory.GenericTargetType == genericTargetType)
                    {
                        var builder = factory.CreateTypeBuilder(this, genericArguments);

                        typeBuilders.Add(builder);

                        return builder.ActualType;
                    }
                }
            }

            return Type.GetType(targetType);
        }

        public TypeBuilder GetTypeBuilderByTargetType(string targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            var wanted = StripAssemblyVersion(targetType);

            foreach (var builder in typeBuilders)
            {
                if (builder.TargetType == wanted)
                {
                    return builder;
                }
            }

            string genericTargetType;
            List<string> genericArguments;

            if (SplitGenericTypeName(wanted, out genericTargetType, out genericArguments))
            {
                foreach (var factory in genericTypeBuilderFactories)
                {
                    if (factory.GenericTargetType == genericTargetType)
                    {
                        var builder = factory.CreateTypeBuilder(this, genericArguments);

                        typeBuilders.Add(builder);

                        return builder;
                    }
                }
            }

            throw new ArgumentException("TypeBuilder not found: targetType=" + targetType, "targetType");
        }

        public TypeReader GetTypeReaderByReaderName(string readerName)
        {
            if (readerName == null) throw new ArgumentNullException("readerName");

            var wanted = StripAssemblyVersion(readerName);

            foreach (var reader in typeReaders)
            {
                if (reader.ReaderName == wanted)
                {
                    if (!reader.Initialized) reader.Initialize(this);
                    return reader;
                }
            }

            string genericReaderName;
            List<string> genericArguments;

            if (SplitGenericTypeName(wanted, out genericReaderName, out genericArguments))
            {
                foreach (var factory in genericTypeReaderFactories)
                {
                    if (factory.GenericReaderName == genericReaderName)
                    {
                        var reader = factory.CreateTypeReader(genericArguments);

                        typeReaders.Add(reader);

                        if (!reader.Initialized) reader.Initialize(this);
                        return reader;
                    }
                }
            }

            throw new ArgumentException("TypeReader not found: readerName=" + readerName, "readerName");
        }

        public TypeReader GetTypeReaderByTargetType(string targetType)
        {
            if (targetType == null) throw new ArgumentNullException("targetType");

            var wanted = StripAssemblyVersion(targetType);

            foreach (var reader in typeReaders)
            {
                if (reader.TargetType == wanted)
                {
                    if (!reader.Initialized) reader.Initialize(this);
                    return reader;
                }
            }

            throw new ArgumentException("TypeReader not found: targetType=" + targetType, "targetType");
        }

        string StripAssemblyVersion(string typeName)
        {
            int commaIndex = 0;

            while (0 <= (commaIndex = typeName.IndexOf(',', commaIndex)))
            {
                if (commaIndex + 1 < (typeName.Length - 1) && typeName[commaIndex + 1] == '[')
                {
                    commaIndex++;
                }
                else
                {
                    int closeBraket = typeName.IndexOf(']', commaIndex);

                    if (0 <= closeBraket)
                    {
                        typeName = typeName.Remove(commaIndex, closeBraket - commaIndex);
                    }
                    else
                    {
                        typeName = typeName.Remove(commaIndex);
                    }
                }
            }

            return typeName;
        }

        bool SplitGenericTypeName(string typeName, out string genericName, out List<string> genericArguments)
        {
            var pos = typeName.IndexOf('`');

            if (pos < 0)
            {
                genericName = null;
                genericArguments = null;
                return false;
            }

            genericArguments = new List<string>();

            genericName = typeName.Substring(0, pos);

            pos++;

            while (pos < typeName.Length && char.IsDigit(typeName[pos]))
                pos++;

            while (pos < typeName.Length && typeName[pos] == '[')
                pos++;

            while (pos < typeName.Length && typeName[pos] != ']')
            {
                int nesting = 0;
                int end;

                for (end = pos; end < typeName.Length; end++)
                {
                    if (typeName[end] == '[')
                    {
                        nesting++;
                    }
                    else if (typeName[end] == ']')
                    {
                        if (0 < nesting)
                            nesting--;
                        else
                            break;
                    }
                }

                genericArguments.Add(typeName.Substring(pos, end - pos));

                pos = end;

                if (pos < typeName.Length && typeName[pos] == ']')
                    pos++;

                if (pos < typeName.Length && typeName[pos] == ',')
                    pos++;
                
                if (pos < typeName.Length && typeName[pos] == '[')
                    pos++;
            }

            return true;
        }
    }
}
