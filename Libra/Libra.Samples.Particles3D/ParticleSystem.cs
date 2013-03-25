#region Using

using System;
using System.Runtime.InteropServices;
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

        [StructLayout(LayoutKind.Explicit, Size = 96)]
        struct CBSettings
        {
            [FieldOffset(0)]
            public float Duration;

            [FieldOffset(4)]
            public float DurationRandomness;

            //public float Dummy0;

            //public float Dummy1;

            [FieldOffset(16)]
            public Vector3 Gravity;

            [FieldOffset(28)]
            public float EndVelocity;

            [FieldOffset(32)]
            public Vector4 MinColor;

            [FieldOffset(48)]
            public Vector4 MaxColor;

            [FieldOffset(64)]
            public Vector2 RotateSpeed;

            //public float Dummy2;

            //public float Dummy3;

            [FieldOffset(80)]
            public Vector2 StartSize;

            [FieldOffset(88)]
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

                //context.RasterizerState = RasterizerState.Wireframe;

                context.PrimitiveTopology = PrimitiveTopology.TriangleList;
                
                context.VertexShaderConstantBuffers[0] = settingsConstantBuffer;
                context.VertexShaderConstantBuffers[1] = constantBuffer;

                context.PixelShaderResources[0] = textureView;
                context.PixelShaderSamplers[0] = SamplerState.LinearClamp;

                context.SetVertexBuffer(0, vertexBuffer);
                context.IndexBuffer = indexBuffer;

                if (firstActiveParticle < firstFreeParticle)
                {
                    context.DrawIndexed(
                        (firstFreeParticle - firstActiveParticle) * 6,
                        firstActiveParticle * 6,
                        firstActiveParticle * 4);
                }
                else
                {
                    context.DrawIndexed(
                        (settings.MaxParticles - firstActiveParticle) * 6,
                        firstActiveParticle * 6,
                        firstActiveParticle * 4);

                    if (firstFreeParticle > 0)
                    {
                        context.DrawIndexed(
                            firstFreeParticle * 6,
                            0,
                            0);
                    }
                }

                context.DepthStencilState = DepthStencilState.Default;
            }

            drawCounter++;
        }

        void AddNewParticlesToVertexBuffer()
        {
            int stride = ParticleVertex.VertexDeclaration.Stride;

            if (firstNewParticle < firstFreeParticle)
            {
                vertexBuffer.SetData(Device.ImmediateContext,
                    firstNewParticle * stride * 4,
                    particles,
                    firstNewParticle * 4,
                    (firstFreeParticle - firstNewParticle) * 4,
                    SetDataOptions.NoOverwrite);

                //ParticleVertex[] temp = new ParticleVertex[(firstFreeParticle - firstNewParticle) * 4];
                //vertexBuffer.GetData(Device.ImmediateContext, temp);

                //Console.WriteLine("" + temp.GetType());
            }
            else
            {
                vertexBuffer.SetData(Device.ImmediateContext,
                    firstNewParticle * stride * 4,
                    particles,
                    firstNewParticle * 4,
                    (settings.MaxParticles - firstNewParticle) * 4,
                    SetDataOptions.NoOverwrite);

                if (firstFreeParticle > 0)
                {
                    vertexBuffer.SetData(Device.ImmediateContext,
                        firstNewParticle * stride * 4,
                        particles,
                        0,
                        firstFreeParticle * 4,
                        SetDataOptions.NoOverwrite);
                }
            }

            firstNewParticle = firstFreeParticle;
        }

        public void SetCamera(Matrix view, Matrix projection)
        {
            Matrix.Transpose(ref view, out constantBufferParameters.View);
            Matrix.Transpose(ref projection, out constantBufferParameters.Projection);
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
