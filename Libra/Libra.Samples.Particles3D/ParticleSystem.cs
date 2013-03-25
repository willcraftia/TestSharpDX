#region Using

using System;
using Libra.Games;
using Libra.Graphics;
using Libra.Graphics.Compiler;
using Libra.PackedVector;
using Libra.Xnb;

#endregion

namespace Libra.Samples.Particles3D
{
    public abstract class ParticleSystem : DrawableGameComponent
    {
        #region CBSettings

        struct CBSettings
        {
            public float Duration;

            public float DurationRandomness;

            public float Dummy0;

            public float Dummy1;

            public Vector3 Gravity;

            public float EndVelocity;

            public Vector4 MinColor;

            public Vector4 MaxColor;

            public Vector2 RotateSpeed;

            public float Dummy2;

            public float Dummy3;

            public Vector2 StartSize;

            public Vector2 EndSize;
        }

        #endregion

        #region CBParameters

        struct CBParameters
        {
            public Matrix View;

            public Matrix Projection;

            public Vector2 ViewportScale;

            public float CurrentTime;

            // 16 の倍数とするためのダミー。
            public float Dummy0;
        }

        #endregion

        ParticleSettings settings = new ParticleSettings();

        XnbManager content;

        VertexShader vertexShader;

        PixelShader pixelShader;

        ConstantBuffer settingsConstantBuffer;

        ConstantBuffer constantBuffer;

        ShaderResourceView textureView;

        ParticleVertex[] particles;

        VertexBuffer vertexBuffer;

        //DynamicVertexBuffer vertexBuffer;

        IndexBuffer indexBuffer;

        CBParameters constantBufferParameters;

        int firstActiveParticle;
        
        int firstNewParticle;
        
        int firstFreeParticle;
        
        int firstRetiredParticle;

        float currentTime;

        int drawCounter;

        static Random random = new Random();

        protected ParticleSystem(Game game, XnbManager content)
            : base(game)
        {
            this.content = content;
        }

        public override void Initialize()
        {
            InitializeSettings(settings);

            particles = new ParticleVertex[settings.MaxParticles * 4];

            for (int i = 0; i < settings.MaxParticles; i++)
            {
                particles[i * 4 + 0].Corner = new Short2(-1, -1);
                particles[i * 4 + 1].Corner = new Short2(1, -1);
                particles[i * 4 + 2].Corner = new Short2(1, 1);
                particles[i * 4 + 3].Corner = new Short2(-1, 1);
            }

            base.Initialize();
        }

        protected abstract void InitializeSettings(ParticleSettings settings);

        protected override void LoadContent()
        {
            LoadParticleEffect();

            vertexBuffer = Device.CreateVertexBuffer();
            vertexBuffer.Usage = ResourceUsage.Dynamic;
            vertexBuffer.Initialize(ParticleVertex.VertexDeclaration, settings.MaxParticles * 4);

            //vertexBuffer = new DynamicVertexBuffer(Device, ParticleVertex.VertexDeclaration,
            //                                       settings.MaxParticles * 4, BufferUsage.WriteOnly);

            ushort[] indices = new ushort[settings.MaxParticles * 6];

            for (int i = 0; i < settings.MaxParticles; i++)
            {
                indices[i * 6 + 0] = (ushort) (i * 4 + 0);
                indices[i * 6 + 1] = (ushort) (i * 4 + 1);
                indices[i * 6 + 2] = (ushort) (i * 4 + 2);

                indices[i * 6 + 3] = (ushort) (i * 4 + 0);
                indices[i * 6 + 4] = (ushort) (i * 4 + 2);
                indices[i * 6 + 5] = (ushort) (i * 4 + 3);
            }

            indexBuffer = Device.CreateIndexBuffer();
            indexBuffer.Usage = ResourceUsage.Immutable;
            indexBuffer.Initialize(indices);
        }

        void LoadParticleEffect()
        {
            // TODO
            //
            // シェーダは全てのパーティクル システムで共通であるため、
            // 外部から指定するか、あるいは、共有リソースとして定義すべき。

            var compiler = new ShaderCompiler();
            compiler.RootPath = "Shaders";

            var vsBytecode = compiler.CompileFromFile("ParticleEffect.fx", "ParticleVertexShader", VertexShaderProfile.vs_4_0);
            var psBytecode = compiler.CompileFromFile("ParticleEffect.fx", "ParticlePixelShader", PixelShaderProfile.ps_4_0);

            vertexShader = Device.CreateVertexShader();
            vertexShader.Initialize(vsBytecode);

            pixelShader = Device.CreatePixelShader();
            pixelShader.Initialize(psBytecode);

            var cbSettings = new CBSettings
            {
                Duration = (float) settings.Duration.TotalSeconds,
                DurationRandomness = settings.DurationRandomness,
                Gravity = settings.Gravity,
                EndVelocity = settings.EndVelocity,
                MinColor = settings.MinColor.ToVector4(),
                MaxColor = settings.MaxColor.ToVector4(),
                RotateSpeed = new Vector2(settings.MinRotateSpeed, settings.MaxRotateSpeed),
                StartSize = new Vector2(settings.MinStartSize, settings.MaxStartSize),
                EndSize = new Vector2(settings.MinEndSize, settings.MaxEndSize)
            };

            settingsConstantBuffer = Device.CreateConstantBuffer();
            settingsConstantBuffer.Usage = ResourceUsage.Immutable;
            settingsConstantBuffer.Initialize(cbSettings);

            constantBuffer = Device.CreateConstantBuffer();
            constantBuffer.Initialize<CBParameters>();

            var texture = content.Load<Texture2D>(settings.TextureName);
            textureView = Device.CreateShaderResourceView();
            textureView.Initialize(texture);
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            currentTime += (float) gameTime.ElapsedGameTime.TotalSeconds;

            RetireActiveParticles();
            FreeRetiredParticles();

            if (firstActiveParticle == firstFreeParticle)
                currentTime = 0;

            if (firstRetiredParticle == firstActiveParticle)
                drawCounter = 0;
        }

        void RetireActiveParticles()
        {
            float particleDuration = (float) settings.Duration.TotalSeconds;

            while (firstActiveParticle != firstNewParticle)
            {
                float particleAge = currentTime - particles[firstActiveParticle * 4].Time;

                if (particleAge < particleDuration)
                    break;

                particles[firstActiveParticle * 4].Time = drawCounter;

                firstActiveParticle++;

                if (firstActiveParticle >= settings.MaxParticles)
                    firstActiveParticle = 0;
            }
        }

        void FreeRetiredParticles()
        {
            while (firstRetiredParticle != firstActiveParticle)
            {
                int age = drawCounter - (int) particles[firstRetiredParticle * 4].Time;

                if (age < 3)
                    break;

                firstRetiredParticle++;

                if (firstRetiredParticle >= settings.MaxParticles)
                    firstRetiredParticle = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var context = Device.ImmediateContext;

            if (firstNewParticle != firstFreeParticle)
            {
                AddNewParticlesToVertexBuffer();
            }

            if (firstActiveParticle != firstFreeParticle)
            {
                context.BlendState = settings.BlendState;
                context.DepthStencilState = DepthStencilState.DepthRead;

                constantBufferParameters.ViewportScale = new Vector2(0.5f / context.Viewport.AspectRatio, -0.5f);
                constantBufferParameters.CurrentTime = currentTime;
                constantBuffer.SetData(Device.ImmediateContext, constantBufferParameters);

                context.VertexShader = vertexShader;
                context.PixelShader = pixelShader;

                context.PixelShaderSamplers[0] = SamplerState.LinearClamp;

                context.PrimitiveTopology = PrimitiveTopology.TriangleList;
                
                context.VertexShaderConstantBuffers[0] = settingsConstantBuffer;
                context.VertexShaderConstantBuffers[1] = constantBuffer;

                context.PixelShaderResources[0] = textureView;

                context.SetVertexBuffer(0, vertexBuffer);
                context.IndexBuffer = indexBuffer;

                if (firstActiveParticle < firstFreeParticle)
                {
                    context.DrawIndexed(
                        (firstFreeParticle - firstActiveParticle) * 2 * 3,
                        firstActiveParticle * 6,
                        firstActiveParticle * 4);
                }
                else
                {
                    context.DrawIndexed(
                        (firstFreeParticle - firstActiveParticle) * 2 * 3,
                        firstActiveParticle * 6,
                        firstActiveParticle * 4);

                    if (firstFreeParticle > 0)
                    {
                        context.DrawIndexed(
                            firstFreeParticle * 2 * 3,
                            0,
                            0);
                    }
                }

                //foreach (EffectPass pass in particleEffect.CurrentTechnique.Passes)
                //{
                //    pass.Apply();

                //    if (firstActiveParticle < firstFreeParticle)
                //    {

                // baseVertex = 0
                // minVertexIndex = firstActiveParticle * 4
                // numVertices = (firstFreeParticle - firstActiveParticle) * 4
                // startIndex = firstActiveParticle * 6
                // primitiveCount = (firstFreeParticle - firstActiveParticle) * 2

                //        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                //                                     firstActiveParticle * 4, (firstFreeParticle - firstActiveParticle) * 4,
                //                                     firstActiveParticle * 6, (firstFreeParticle - firstActiveParticle) * 2);
                //    }
                //    else
                //    {
                //        device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                //                                     firstActiveParticle * 4, (settings.MaxParticles - firstActiveParticle) * 4,
                //                                     firstActiveParticle * 6, (settings.MaxParticles - firstActiveParticle) * 2);

                //        if (firstFreeParticle > 0)
                //        {
                //            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0,
                //                                         0, firstFreeParticle * 4,
                //                                         0, firstFreeParticle * 2);
                //        }
                //    }
                //}

                context.DepthStencilState = DepthStencilState.Default;
            }

            drawCounter++;
        }

        void AddNewParticlesToVertexBuffer()
        {
            //int stride = ParticleVertex.VertexDeclaration.Stride;

            if (firstNewParticle < firstFreeParticle)
            {
                vertexBuffer.SetData(Device.ImmediateContext,
                    particles,
                    firstNewParticle * 4,
                    (firstFreeParticle - firstNewParticle) * 4,
                    firstNewParticle * 4,
                    SetDataOptions.NoOverwrite);
            }
            else
            {
                vertexBuffer.SetData(Device.ImmediateContext,
                    particles,
                    firstNewParticle * 4,
                    (firstFreeParticle - firstNewParticle) * 4,
                    firstNewParticle * 4,
                    SetDataOptions.NoOverwrite);

                if (firstFreeParticle > 0)
                {
                    vertexBuffer.SetData(Device.ImmediateContext,
                        particles,
                        0,
                        firstFreeParticle * 4,
                        0,
                        SetDataOptions.NoOverwrite);
                }
            }

            //if (firstNewParticle < firstFreeParticle)
            //{
            //    vertexBuffer.SetData(firstNewParticle * stride * 4, particles,
            //                         firstNewParticle * 4,
            //                         (firstFreeParticle - firstNewParticle) * 4,
            //                         stride, SetDataOptions.NoOverwrite);
            //}
            //else
            //{
            //    vertexBuffer.SetData(firstNewParticle * stride * 4, particles,
            //                         firstNewParticle * 4,
            //                         (settings.MaxParticles - firstNewParticle) * 4,
            //                         stride, SetDataOptions.NoOverwrite);

            //    if (firstFreeParticle > 0)
            //    {
            //        vertexBuffer.SetData(0, particles,
            //                             0, firstFreeParticle * 4,
            //                             stride, SetDataOptions.NoOverwrite);
            //    }
            //}

            firstNewParticle = firstFreeParticle;
        }

        public void SetCamera(Matrix view, Matrix projection)
        {
            constantBufferParameters.View = view;
            constantBufferParameters.Projection = projection;
        }

        public void AddParticle(Vector3 position, Vector3 velocity)
        {
            int nextFreeParticle = firstFreeParticle + 1;

            if (nextFreeParticle >= settings.MaxParticles)
                nextFreeParticle = 0;

            if (nextFreeParticle == firstRetiredParticle)
                return;

            velocity *= settings.EmitterVelocitySensitivity;

            float horizontalVelocity = MathHelper.Lerp(settings.MinHorizontalVelocity,
                                                       settings.MaxHorizontalVelocity,
                                                       (float) random.NextDouble());

            double horizontalAngle = random.NextDouble() * MathHelper.TwoPi;

            velocity.X += horizontalVelocity * (float) Math.Cos(horizontalAngle);
            velocity.Z += horizontalVelocity * (float) Math.Sin(horizontalAngle);

            velocity.Y += MathHelper.Lerp(settings.MinVerticalVelocity,
                                          settings.MaxVerticalVelocity,
                                          (float) random.NextDouble());

            Color randomValues = new Color((byte) random.Next(255),
                                           (byte) random.Next(255),
                                           (byte) random.Next(255),
                                           (byte) random.Next(255));

            for (int i = 0; i < 4; i++)
            {
                particles[firstFreeParticle * 4 + i].Position = position;
                particles[firstFreeParticle * 4 + i].Velocity = velocity;
                particles[firstFreeParticle * 4 + i].Random = randomValues;
                particles[firstFreeParticle * 4 + i].Time = currentTime;
            }

            firstFreeParticle = nextFreeParticle;
        }
    }
}
