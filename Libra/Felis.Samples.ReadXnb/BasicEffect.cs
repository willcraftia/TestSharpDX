#region Using

using System;

#endregion

namespace Felis.Samples.ReadXnb
{
    public sealed class BasicEffect : Effect
    {
        public string Texture;

        public Vector3 DiffuseColor;

        public Vector3 EmissiveColor;

        public Vector3 SpecularColor;

        public float SpecularPower;

        public float Alpha;

        public bool VertexColorEnabled;
    }
}
