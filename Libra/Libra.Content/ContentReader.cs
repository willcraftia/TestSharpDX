#region Using

using System;
using System.IO;
using Libra.Graphics;

#endregion

namespace Libra.Content
{
    public sealed class ContentReader : BinaryReader
    {
        ContentTypeReaderManager manager;

        public IDevice Device { get; private set; }

        // テクスチャ等の読み込みにて使用する DeviceContext。
        // どのような文脈にて読み込みを行うかを明示するために必要。
        // コンストラクタで null を指定した場合は暗黙的に ImmidiateContext。

        public DeviceContext DeviceContext { get; private set; }

        public ContentReader(Stream stream, ContentTypeReaderManager manager, IDevice device, DeviceContext context = null)
            : base(stream)
        {
            if (manager == null) throw new ArgumentNullException("manager");
            if (device == null) throw new ArgumentNullException("device");

            this.manager = manager;
            Device = device;
            DeviceContext = context ?? device.ImmediateContext;
        }

        public T ReadObject<T>()
        {
            return (T) ReadObject(default(T));
        }

        public T ReadObject<T>(T existingInstance)
        {
            var typeReader = manager[typeof(T)];
            return (T) ReadObject(typeReader, existingInstance);
        }

        public T ReadObject<T>(ContentTypeReader typeReader)
        {
            return (T) ReadObject(typeReader, default(T));
        }

        public T ReadObject<T>(ContentTypeReader typeReader, T existingInstance)
        {
            return (T) typeReader.Read(this, existingInstance);
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
    }
}
