#region Using

using System;
using System.IO;
using Felis.Xnb;

#endregion

namespace Felis.Samples.ReadXnb
{
    class Program
    {
        static void Main(string[] args)
        {
            var typeReaderManager = new TypeReaderManager();
            typeReaderManager.RegisterStandardTypeReaders();

            typeReaderManager.RegisterTypeBuilder<Vector3Builder>();
            typeReaderManager.RegisterTypeBuilder<RectangleBuilder>();
            typeReaderManager.RegisterTypeBuilder<MatrixBuilder>();
            typeReaderManager.RegisterTypeBuilder<VertexBufferBuilder>();
            typeReaderManager.RegisterTypeBuilder<VertexDeclarationBuilder>();
            typeReaderManager.RegisterTypeBuilder<IndexBufferBuilder>();
            typeReaderManager.RegisterTypeBuilder<BasicEffectBuilder>();
            typeReaderManager.RegisterTypeBuilder<ModelBuilder>();
            typeReaderManager.RegisterTypeBuilder<Texture2DBuilder>();
            typeReaderManager.RegisterTypeBuilder<SpriteFontBuilder>();

            using (var stream = File.OpenRead("../../Content/dude.xnb"))
            {
                using (var reader = new XnbReader(stream, "dude", typeReaderManager, null))
                {
                    var instance = reader.ReadXnb();

                    Console.WriteLine(instance);
                }
            }

            using (var stream = File.OpenRead("../../Content/SpriteFont.xnb"))
            {
                using (var reader = new XnbReader(stream, "SpriteFont", typeReaderManager, null))
                {
                    var instance = reader.ReadXnb();

                    Console.WriteLine(instance);
                }
            }

            Console.ReadLine();
        }
    }
}
