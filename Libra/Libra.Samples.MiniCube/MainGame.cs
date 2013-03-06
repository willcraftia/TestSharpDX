#region Using

using System;
using Libra.Games;
using Libra.Games.SharpDX;
using Libra.Graphics;
using Libra.Compiler;

using SDXWRenderForm = SharpDX.Windows.RenderForm;

#endregion

namespace Libra.Samples.MiniCube
{
    public sealed class MainGame : Game
    {
        IGamePlatform platform;

        GraphicsManager graphicsManager;

        VertexShader vertexShader;

        PixelShader pixelShader;

        InputLayout inputLayout;

        VertexBuffer vertexBuffer;

        ConstantBuffer constantBuffer;

        public MainGame()
        {
            platform = new SdxFormGamePlatform(this, new SDXWRenderForm());
            graphicsManager = new GraphicsManager(this);
        }

        protected override void LoadContent()
        {
            // ここではテストのために行優先でコンパイル。
            var compiler = new ShaderCompiler();
            compiler.RootPath = "Shaders";
            compiler.PackMatrixRowMajor = true;

            var vsBytecode = compiler.CompileFromFile("MiniCube.fx", "VS", VertexShaderProfile.vs_4_0);
            var psBytecode = compiler.CompileFromFile("MiniCube.fx", "PS", PixelShaderProfile.ps_4_0);

            vertexShader = Device.CreateVertexShader();
            vertexShader.Initialize(vsBytecode);

            pixelShader = Device.CreatePixelShader();
            pixelShader.Initialize(psBytecode);
            
            inputLayout = Device.CreateInputLayout();
            inputLayout.Initialize<InputPositionColor>(vsBytecode);

            vertexBuffer = Device.CreateVertexBuffer();
            vertexBuffer.Usage = ResourceUsage.Immutable;
            vertexBuffer.Initialize(new []
                {
                    new InputPositionColor { Position = new Vector3(-1, -1,  1), Color = new Color(255, 0, 0, 255) },
                    new InputPositionColor { Position = new Vector3(-1,  1,  1), Color = new Color(255, 0, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1,  1), Color = new Color(255, 0, 0, 255) },
                    new InputPositionColor { Position = new Vector3(-1, -1,  1), Color = new Color(255, 0, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1,  1), Color = new Color(255, 0, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1, -1,  1), Color = new Color(255, 0, 0, 255) },

                    new InputPositionColor { Position = new Vector3(-1, -1, -1), Color = new Color(0, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1, -1), Color = new Color(0, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3(-1,  1, -1), Color = new Color(0, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3(-1, -1, -1), Color = new Color(0, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1, -1, -1), Color = new Color(0, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1, -1), Color = new Color(0, 255, 0, 255) },

                    new InputPositionColor { Position = new Vector3(-1,  1, -1), Color = new Color(0, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1, -1), Color = new Color(0, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1,  1), Color = new Color(0, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3(-1,  1, -1), Color = new Color(0, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1,  1), Color = new Color(0, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3(-1,  1,  1), Color = new Color(0, 0, 255, 255) },

                    new InputPositionColor { Position = new Vector3(-1, -1, -1), Color = new Color(255, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3(-1, -1,  1), Color = new Color(255, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1, -1,  1), Color = new Color(255, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3(-1, -1, -1), Color = new Color(255, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1, -1,  1), Color = new Color(255, 255, 0, 255) },
                    new InputPositionColor { Position = new Vector3( 1, -1, -1), Color = new Color(255, 255, 0, 255) },

                    new InputPositionColor { Position = new Vector3(-1, -1, -1), Color = new Color(255, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3(-1,  1, -1), Color = new Color(255, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3(-1,  1,  1), Color = new Color(255, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3(-1, -1, -1), Color = new Color(255, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3(-1,  1,  1), Color = new Color(255, 0, 255, 255) },
                    new InputPositionColor { Position = new Vector3(-1, -1,  1), Color = new Color(255, 0, 255, 255) },

                    new InputPositionColor { Position = new Vector3( 1, -1, -1), Color = new Color(0, 255, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1, -1,  1), Color = new Color(0, 255, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1,  1), Color = new Color(0, 255, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1, -1, -1), Color = new Color(0, 255, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1,  1), Color = new Color(0, 255, 255, 255) },
                    new InputPositionColor { Position = new Vector3( 1,  1, -1), Color = new Color(0, 255, 255, 255) },
                });

            constantBuffer = Device.CreateConstantBuffer();
            constantBuffer.Initialize<Matrix>();

            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            var context = Device.ImmediateContext;

            context.Clear(Color.CornflowerBlue);

            context.InputAssemblerStage.InputLayout = inputLayout;
            context.InputAssemblerStage.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssemblerStage.SetVertexBuffer(0, vertexBuffer);

            context.VertexShaderStage.VertexShader = vertexShader;
            context.VertexShaderStage.SetConstantBuffer(0, constantBuffer);
            context.PixelShaderStage.PixelShader = pixelShader;

            float aspect = context.RasterizerStage.Viewport.AspectRatio;
            float time = (float) gameTime.TotalGameTime.TotalSeconds;

            var world = Matrix.CreateRotationX(time) * Matrix.CreateRotationY(time * 2) * Matrix.CreateRotationZ(time * .7f);
            var view = Matrix.CreateLookAt(new Vector3(0, 0, -5), Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspect, 0.1f, 100.0f);

            // メモ
            //
            // HLSL デフォルトは列優先 (column_major)。
            // http://msdn.microsoft.com/en-us/library/bb509706(v=vs.85).aspx
            //
            // Matrix 構造体は、SharpDX も XNA も自作 Matrix も、
            // フィールドの並びが行優先 (HLSL では row_major)。
            //
            // このため、HLSL デフォルトでは列優先に対応させる必要があり、
            // このためには行列を転置させてから設定すれば良い。
            //
            // あるいは、シェーダをコンパイルする際に、オプション指定により
            // 行優先と列優先を設定できる。
            //
            // なお、列優先の方が効率的であるらしい。
            // http://maverickproj.web.fc2.com/d3d11_02.html
            //
            // 基本的には、デフォルトに従う事を重視し、
            // 転置してから設定という手順を踏むが、
            // ここではテストのためにコンパイル時に行優先としている。

            var worldViewProjection = world * view * projection;
            //worldViewProjection.Transpose();
            constantBuffer.SetData(context, worldViewProjection);

            context.Draw(36);

            base.Draw(gameTime);
        }
    }
}
