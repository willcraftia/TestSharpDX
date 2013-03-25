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
            new InputElement("POSITION", 0, InputElementFormat.Vector3),
            new InputElement("POSITION", 1, InputElementFormat.Short2),
            new InputElement("NORMAL",   0, InputElementFormat.Vector3),
            new InputElement("COLOR",    0, InputElementFormat.Color),
            new InputElement("TEXCOORD", 0, InputElementFormat.Single)
            );
    }
}
