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
    }
}
