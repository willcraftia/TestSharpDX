#region Using

using System;
using Libra.Graphics;
using Libra.PackedVector;

#endregion

namespace Libra.Samples.Particles3D
{
    public struct ParticleVertex
    {
        public Short2 Corner;

        public Vector3 Position;

        public Vector3 Velocity;

        public Color Random;

        public float Time;

        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration(
            new InputElement("POSITION",                    0, InputElementFormat.Short2),
            new InputElement("POSITION",                    1, InputElementFormat.Vector3),
            new InputElement(InputElement.SemanticNormal,   0, InputElementFormat.Vector3),
            new InputElement(InputElement.SemanticColor,    0, InputElementFormat.Color),
            new InputElement(InputElement.SemanticTexCoord, 0, InputElementFormat.Single)
            );

        //public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
        //(
        //    new VertexElement(0, VertexElementFormat.Short2,
        //                         VertexElementUsage.Position, 0),

        //    new VertexElement(4, VertexElementFormat.Vector3,
        //                         VertexElementUsage.Position, 1),

        //    new VertexElement(16, VertexElementFormat.Vector3,
        //                          VertexElementUsage.Normal, 0),

        //    new VertexElement(28, VertexElementFormat.Color,
        //                          VertexElementUsage.Color, 0),

        //    new VertexElement(32, VertexElementFormat.Single,
        //                          VertexElementUsage.TextureCoordinate, 0)
        //);

        //public const int SizeInBytes = 36;
    }
}
