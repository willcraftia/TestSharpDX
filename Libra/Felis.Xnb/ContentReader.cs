#region Using

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#endregion

namespace Felis.Xnb
{
    public class ContentReader : BinaryReader
    {
        Action<IDisposable> recordDisposableObject;

        string[] originalTypeReaderNames;

        TypeReader[] typeReaders;

        int sharedResourceCount;

        Dictionary<int, List<Delegate>> fixupListMap;

        public ContentManager ContentManager { get; private set; }

        public object DeviceContext { get; private set; }

        public char TargetPlatform { get; private set; }

        public byte FormatVersion { get; private set; }

        public bool IsHiDefProfile { get; private set; }

        public bool IsCompressed { get; private set; }

        public uint CompressedFileSize { get; private set; }

        public uint DecompressedFileSize { get; private set; }

        public ContentReader(Stream stream, string assetName,
            ContentManager contentManager, object deviceContext, Action<IDisposable> recordDisposableObject)
            : base(stream)
        {
            if (contentManager == null) throw new ArgumentNullException("contentManager");
            if (assetName == null) throw new ArgumentNullException("assetName");

            ContentManager = contentManager;
            DeviceContext = deviceContext;
            this.recordDisposableObject = recordDisposableObject;

            fixupListMap = new Dictionary<int, List<Delegate>>();
        }

        public object ReadXnb()
        {
            ReadHeader();
            ReadTypeManifest();
            ReadSharedResourceCount();

            var result = ReadObject();

            ReadSharedResourceData();

            return result;
        }

        void ReadHeader()
        {
            // Format identifier (magic number).
            byte magic1 = ReadByte();
            byte magic2 = ReadByte();
            byte magic3 = ReadByte();
            if (magic1 != 'X' || magic2 != 'N' || magic3 != 'B')
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append((char) magic1);
                stringBuilder.Append((char) magic2);
                stringBuilder.Append((char) magic3);
                throw new InvalidOperationException("Invalid XNB magic numbers: " + stringBuilder);
            }

            // Target platform.
            TargetPlatform = (char) ReadByte();

            // Format version.
            FormatVersion = ReadByte();
            if (FormatVersion != 5)
            {
                throw new NotSupportedException(
                    string.Format("Format version '{0}' not supported ", FormatVersion));
            }

            // Flag bits.
            int flagBits = (int) ReadByte();
            IsHiDefProfile = (flagBits & 1) != 0;

            IsCompressed = (flagBits & 0x80) != 0;

            // Compressed file size.
            CompressedFileSize = ReadUInt32();

            if (IsCompressed)
            {
                // Decompressed data size.
                DecompressedFileSize = ReadUInt32();

                throw new NotSupportedException("Compressed XNB not supported.");
            }
        }

        void ReadTypeManifest()
        {
            // Type reader count.
            int typeReaderCount = Read7BitEncodedInt();

            originalTypeReaderNames = new string[typeReaderCount];
            typeReaders = new TypeReader[typeReaderCount];

            // Repeat <type reader count>.
            for (int i = 0; i < typeReaderCount; i++)
            {
                string readerName = ReadString();
                int readerVersion = ReadInt32();

                originalTypeReaderNames[i] = readerName;

                // Actual type reader.
                typeReaders[i] = ContentManager.TypeReaderManager.GetTypeReaderByReaderName(readerName);
            }
        }

        void ReadSharedResourceCount()
        {
            sharedResourceCount = Read7BitEncodedInt();
        }

        void ReadSharedResourceData()
        {
            for (int i = 0; i < sharedResourceCount; i++)
            {
                var sharedResource = ReadObject();

                var resourceId = i + 1;

                List<Delegate> fixupList;
                if (fixupListMap.TryGetValue(resourceId, out fixupList))
                {
                    for (int j = 0; j < fixupList.Count; j++)
                    {
                        var fixup = fixupList[j];
                        fixup.Method.Invoke(fixup.Target, new[] { sharedResource });
                    }
                }
            }
        }

        public object ReadObject()
        {
            // typeId
            var typeId = Read7BitEncodedInt();

            if (typeId == 0)
            {
                return null;
            }

            // 対応する型リーダの取得。
            var typeReader = typeReaders[typeId - 1];

            // 読み込み。
            var obj = ReadObject(typeReader);

            // 破棄の記録。
            var disposable = obj as IDisposable;
            if (disposable != null)
                RecordDisposableObject(disposable);

            return obj;
        }

        public object ReadObject(TypeReader typeReader)
        {
            return typeReader.Read(this);
        }

        public int ReadSharedResource(Action<object> fixup)
        {
            var resourceId = Read7BitEncodedInt();

            if (fixup != null)
            {
                if (resourceId == 0)
                {
                    fixup(null);
                }
                else
                {
                    List<Delegate> fixupList;
                    if (!fixupListMap.TryGetValue(resourceId, out fixupList))
                    {
                        fixupList = new List<Delegate>();
                        fixupListMap[resourceId] = fixupList;
                    }

                    fixupList.Add(fixup);
                }
            }

            return resourceId;
        }

        void RecordDisposableObject(IDisposable disposable)
        {
            if (recordDisposableObject != null)
            {
                recordDisposableObject(disposable);
            }
            else
            {
                ContentManager.RecordDisposableObject(disposable);
            }
        }
    }
}
