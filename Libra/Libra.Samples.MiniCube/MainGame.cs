#region Using

using System;
using Libra.Games;
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

        public MainGame()
        {
            platform = new FormGamePlatform(this, new SDXWRenderForm());
            graphicsManager = new GraphicsManager(this);
            //graphicsManager.PreferredBackBufferWidth = 0;
            //graphicsManager.PreferredBackBufferHeight = 0;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        VertexShader vertexShader;

        InputLayout inputLayout;

        PixelShader pixelShader;

        VertexBuffer vertexBuffer;

        ConstantBuffer constantBuffer;

        protected override void LoadContent()
        {
            // ここではテストのために行優先でコンパイル。
            var compiler = new ShaderCompiler();
            compiler.RootPath = "Shader";
            compiler.PackMatrixRowMajor = true;

            var vsBytecode = compiler.CompileFromFile("MiniCube.fx", "VS", VertexShaderProfile.vs_4_0);
            var psBytecode = compiler.CompileFromFile("MiniCube.fx", "PS", PixelShaderProfile.ps_4_0);

            vertexShader = new VertexShader(Device, vsBytecode);
            inputLayout = new InputLayout(Device, vsBytecode, typeof(InputPositionColor));
            pixelShader = new PixelShader(Device, psBytecode);

            // メモ
            //
            // 左手系 (DirectX 標準) と右手系 (ここでは XNA に倣って右手) の差異により、
            // SharpDX とは頂点座標 (すなわち法線であり外積の方向) が異なる。
            // なお、面カリングは頂点から定まる法線を基準とするのみであることから、
            // 法線方向さえ正しければ左手右手での差異は無い。

            vertexBuffer = VertexBuffer.Create(Device, new []
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

            constantBuffer = ConstantBuffer.Create<Matrix>(Device);

            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            var context = Device.ImmediateContext;

            var backBufferWidth = graphicsManager.SwapChain.BackBufferWidth;
            var backBufferHeight = graphicsManager.SwapChain.BackBufferHeight;
            var viewport = new Viewport(0, 0, backBufferWidth, backBufferHeight);

            context.InputAssemblerStage.InputLayout = inputLayout;
            context.InputAssemblerStage.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssemblerStage.SetVertexBuffer<InputPositionColor>(0, vertexBuffer);

            context.VertexShaderStage.Shader = vertexShader;
            context.VertexShaderStage.SetConstantBuffer(0, constantBuffer);

            context.RasterizerStage.Viewport = viewport;

            context.PixelShaderStage.Shader = pixelShader;

            context.OutputMergerStage.Clear(ClearOptions.Target | ClearOptions.Depth, Color.CornflowerBlue);

            float time = (float) gameTime.TotalGameTime.TotalSeconds;
            var world = Matrix.CreateRotationX(time) * Matrix.CreateRotationY(time * 2) * Matrix.CreateRotationZ(time * .7f);
            var view = Matrix.CreateLookAt(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, viewport.AspectRatio, 0.1f, 100.0f);

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
