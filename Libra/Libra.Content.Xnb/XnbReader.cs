#region Using

using System;
using System.Collections.Generic;
using System.IO;
using Libra.Graphics;

#endregion

namespace Libra.Content.Xnb
{
    public sealed class XnbReader : BinaryReader
    {
        XnbTypeReaderManager typeReaderManager;

        string[] xnbTypeReaderTypeNamesById;

        XnbTypeReader[] typeReaders;

        int sharedResourceCount;

        Dictionary<int, List<Delegate>> fixupListMap;

        public IDevice Device { get; private set; }

        // テクスチャ等の読み込みにて使用する DeviceContext。
        // どのような文脈にて読み込みを行うかを明示するために必要。
        // コンストラクタで null を指定した場合は暗黙的に ImmidiateContext。

        public DeviceContext DeviceContext { get; private set; }

        public XnbReader(Stream stream, XnbTypeReaderManager typeReaderManager, IDevice device, DeviceContext context = null)
            : base(stream)
        {
            if (typeReaderManager == null) throw new ArgumentNullException("typeReaderManager");
            if (device == null) throw new ArgumentNullException("device");

            this.typeReaderManager = typeReaderManager;
            Device = device;
            DeviceContext = context ?? device.ImmediateContext;

            fixupListMap = new Dictionary<int, List<Delegate>>();
        }

        public T ReadXnb<T>()
        {
            ReadHeader();
            ReadTypeManifest();
            ReadSharedResourceCount();

            var result = ReadObject<T>();

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
                throw new InvalidOperationException("Not an XNB file.");

            // Target platform.
            char targetPlatform = (char) ReadByte();
            switch (targetPlatform)
            {
                case 'w':
                    LogWriteLine("Target platform: Windows");
                    break;
                case 'm':
                    LogWriteLine("Target platform: Windows Phone");
                    break;
                case 'x':
                    LogWriteLine("Target platform: Xbox 360");
                    break;
                default:
                    LogWriteLine("Unknown target platform: {0}", targetPlatform);
                    break;
            }

            // Format version.
            byte formatVersion = ReadByte();
            if (formatVersion != 5)
            {
                LogWriteLine("Warning: not an XNA Game Studio version 4.0 XNB file. Parsing may fail unexpectedly.");
            }

            // Flag bits.
            // HiDef/Reach の判定は不要。
            // 圧縮は非対応とする (フィールドを読み込むのみ)。
            int flagBits = (int) ReadByte();
            if ((flagBits & 1) != 0)
            {
                LogWriteLine("Graphics profile: HiDef");
            }
            else
            {
                LogWriteLine("Graphics profile: Reach");
            }

            bool isCompressed = (flagBits & 0x80) != 0;

            // Compressed file size.
            // Decompressed data size.
            // 圧縮は非対応とする (フィールドを読み込むのみ)。
            // 非圧縮の場合、Decompressed data size はヘッダに含まれない。
            uint compressedFileSize = ReadUInt32();
            //Console.WriteLine(compressedFileSize);

            if (isCompressed)
            {
                uint decompressedFileSize = ReadUInt32();

                throw new NotSupportedException("Don't support reading the contents of compressed XNB files.");
            }
        }

        void ReadTypeManifest()
        {
            // Type reader count.
            int typeReaderCount = Read7BitEncodedInt();

            xnbTypeReaderTypeNamesById = new string[typeReaderCount];
            typeReaders = new XnbTypeReader[typeReaderCount];

            // Repeat <type reader count>.
            for (int i = 0; i < typeReaderCount; i++)
            {
                string name = ReadString();
                int version = ReadInt32();

                LogWriteLine("{0} (version {1})", name, version);

                xnbTypeReaderTypeNamesById[i] = name;

                // 型リーダのインスタンス化。
                typeReaders[i] = typeReaderManager.GetTypeReaderByXnaTypeName(name);
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
                var sharedResource = ReadObject<object>();

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

        public T ReadObject<T>()
        {
            // typeId
            var typeId = Read7BitEncodedInt();

            if (typeId == 0)
            {
                LogWriteLine("Type: null");
                return default(T);
            }

            LogWriteLine("Type: {0}", xnbTypeReaderTypeNamesById[typeId - 1]);

            // 対応する型リーダの取得。
            var typeReader = typeReaders[typeId - 1];

            // 読み込み。
            return (T) ReadObject(typeReader, default(T));
        }

        public T ReadObject<T>(T existingInstance)
        {
            // typeId
            var typeId = Read7BitEncodedInt();
            LogWriteLine("Type: {0}", xnbTypeReaderTypeNamesById[typeId - 1]);

            // 対応する型リーダの取得。
            var typeReader = typeReaders[typeId - 1];

            // 読み込み。
            return (T) ReadObject(typeReader, existingInstance);
        }

        public T ReadObject<T>(XnbTypeReader typeReader)
        {
            return (T) ReadObject(typeReader, default(T));
        }

        public T ReadObject<T>(XnbTypeReader typeReader, T existingInstance)
        {
            return (T) typeReader.Read(this, existingInstance);
        }

        public void ReadSharedResource<T>(Action<T> fixup)
        {
            if (fixup == null) throw new ArgumentNullException("fixup");

            var resourceId = Read7BitEncodedInt();

            if (resourceId == 0)
            {
                fixup(default(T));
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

        public Vector2 ReadVector2()
        {
            var result = new Vector2();
            result.X = ReadSingle();
            result.Y = ReadSingle();
            return result;
        }

        public Vector3 ReadVector3()
        {
            var result = new Vector3();
            result.X = ReadSingle();
            result.Y = ReadSingle();
            result.Z = ReadSingle();
            return result;
        }

        public Vector4 ReadVector4()
        {
            var result = new Vector4();
            result.X = ReadSingle();
            result.Y = ReadSingle();
            result.Z = ReadSingle();
            result.W = ReadSingle();
            return result;
        }

        public Matrix ReadMatrix()
        {
            var result = new Matrix();
            result.M11 = ReadSingle();
            result.M12 = ReadSingle();
            result.M13 = ReadSingle();
            result.M14 = ReadSingle();
            result.M21 = ReadSingle();
            result.M22 = ReadSingle();
            result.M23 = ReadSingle();
            result.M24 = ReadSingle();
            result.M31 = ReadSingle();
            result.M32 = ReadSingle();
            result.M33 = ReadSingle();
            result.M34 = ReadSingle();
            result.M41 = ReadSingle();
            result.M42 = ReadSingle();
            result.M43 = ReadSingle();
            result.M44 = ReadSingle();
            return result;
        }

        public Quaternion ReadQuaternion()
        {
            var result = new Quaternion();
            result.X = ReadSingle();
            result.Y = ReadSingle();
            result.Z = ReadSingle();
            result.W = ReadSingle();
            return result;
        }

        public Color ReadColor()
        {
            var result = new Color();
            result.R = ReadByte();
            result.G = ReadByte();
            result.B = ReadByte();
            result.A = ReadByte();
            return result;
        }

        public BoundingSphere ReadBoundingSphere()
        {
            var result = new BoundingSphere();
            result.Center = ReadVector3();
            result.Radius = ReadSingle();
            return result;
        }

        void LogWriteLine(object value)
        {
            Console.WriteLine(value);
        }

        void LogWriteLine(string value)
        {
            Console.WriteLine(value);
        }

        void LogWriteLine(string format, params object[] arg)
        {
            Console.WriteLine(format, arg);
        }
    }
}
