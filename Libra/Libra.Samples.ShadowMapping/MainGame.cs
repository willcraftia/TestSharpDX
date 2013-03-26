#region Using

using System;
using Libra.Games;
using Libra.Games.SharpDX;
using Libra.Graphics;
using Libra.Input;
using Libra.Xnb;

#endregion

namespace Libra.Samples.ShadowMapping
{
    public sealed class MainGame : Game
    {
        const int shadowMapWidthHeight = 2048;

        const int windowWidth = 800;

        const int windowHeight = 480;

        IGamePlatform platform;

        GraphicsManager graphicsManager;

        XnbManager content;

        SpriteBatch spriteBatch;

        Vector3 cameraPosition = new Vector3(0, 70, 100);
        
        Vector3 cameraForward = new Vector3(0, -0.4472136f, -0.8944272f);
        
        BoundingFrustum cameraFrustum = new BoundingFrustum(Matrix.Identity);
 
        Vector3 lightDir = new Vector3(-0.3333333f, 0.6666667f, 0.6666667f);

        IKeyboard keyboard;

        IJoystick joystick;

        KeyboardState currentKeyboardState;
        
        JoystickState currentJoystickState;

        Model gridModel;

        Model dudeModel;

        float rotateDude = 0.0f;

        RenderTarget shadowRenderTarget;

        RenderTargetView shadowRenderTargetView;

        Matrix world;
        
        Matrix view;
        
        Matrix projection;

        Matrix lightViewProjection;

        public MainGame()
        {
            platform = new SdxFormGamePlatform(this)
            {
                DirectInputEnabled = true
            };
            graphicsManager = new GraphicsManager(this);

            content = new XnbManager(Services, "Content");

            graphicsManager.PreferredBackBufferWidth = windowWidth;
            graphicsManager.PreferredBackBufferHeight = windowHeight;

            var aspectRatio = (float) windowWidth / (float) windowHeight;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, aspectRatio,  1.0f, 1000.0f);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Device.ImmediateContext);

            gridModel = content.Load<Model>("grid");
            dudeModel = content.Load<Model>("dude");

            shadowRenderTarget = Device.CreateRenderTarget();
            shadowRenderTarget.Width = shadowMapWidthHeight;
            shadowRenderTarget.Height = shadowMapWidthHeight;
            shadowRenderTarget.MipLevels = 1;
            shadowRenderTarget.Format = SurfaceFormat.Single;
            shadowRenderTarget.DepthFormat = DepthFormat.Depth24Stencil8;
            shadowRenderTarget.Initialize();

            shadowRenderTargetView = Device.CreateRenderTargetView();
            shadowRenderTargetView.Initialize(shadowRenderTarget);

            keyboard = platform.CreateKeyboard();
            joystick = platform.CreateJoystick();
        }

        protected override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);

            UpdateCamera(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var context = Device.ImmediateContext;

            lightViewProjection = CreateLightViewProjectionMatrix();

            context.BlendState = BlendState.Opaque;
            context.DepthStencilState = DepthStencilState.Default;

            CreateShadowMap();

            DrawWithShadowMap();

            DrawShadowMapToScreen();

            base.Draw(gameTime);
        }

        Matrix CreateLightViewProjectionMatrix()
        {
            var lightRotation = Matrix.CreateLookAt(Vector3.Zero, -lightDir, Vector3.Up);

            var frustumCorners = cameraFrustum.GetCorners();

            for (int i = 0; i < frustumCorners.Length; i++)
            {
                frustumCorners[i] = Vector3.Transform(frustumCorners[i], lightRotation);
            }

            var lightBox = BoundingBox.CreateFromPoints(frustumCorners);

            var boxSize = lightBox.Max - lightBox.Min;
            var halfBoxSize = boxSize * 0.5f;

            var lightPosition = lightBox.Min + halfBoxSize;
            lightPosition.Z = lightBox.Min.Z;

            lightPosition = Vector3.Transform(lightPosition, Matrix.Invert(lightRotation));

            var lightView = Matrix.CreateLookAt(lightPosition, lightPosition - lightDir, Vector3.Up);
            
            var lightProjection = Matrix.CreateOrthographic(boxSize.X, boxSize.Y, -boxSize.Z, boxSize.Z);

            return lightView * lightProjection;
        }

        void CreateShadowMap()
        {
            var context = Device.ImmediateContext;

            context.SetRenderTarget(shadowRenderTargetView);

            context.Clear(Color.White);

            world = Matrix.CreateRotationY(MathHelper.ToRadians(rotateDude));
            DrawModel(dudeModel, true);

            context.SetRenderTarget(null);
        }

        void DrawWithShadowMap()
        {
            var context = Device.ImmediateContext;

            context.Clear(Color.CornflowerBlue);

            context.PixelShaderSamplers[1] = SamplerState.PointClamp;

            world = Matrix.Identity;
            DrawModel(gridModel, false);

            world = Matrix.CreateRotationY(MathHelper.ToRadians(rotateDude));
            DrawModel(dudeModel, false);
        }

        void DrawModel(Model model, bool createShadowMap)
        {
            string techniqueName = createShadowMap ? "CreateShadowMap" : "DrawWithShadowMap";

            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            //foreach (var mesh in model.Meshes)
            //{
            //    foreach (Effect effect in mesh.Effects)
            //    {
            //        effect.CurrentTechnique = effect.Techniques[techniqueName];
            //        effect.Parameters["World"].SetValue(world);
            //        effect.Parameters["View"].SetValue(view);
            //        effect.Parameters["Projection"].SetValue(projection);
            //        effect.Parameters["LightDirection"].SetValue(lightDir);
            //        effect.Parameters["LightViewProj"].SetValue(lightViewProjection);

            //        if (!createShadowMap)
            //            effect.Parameters["ShadowMap"].SetValue(shadowRenderTarget);
            //    }
            //    mesh.Draw();
            //}
        }

        void DrawShadowMapToScreen()
        {
            spriteBatch.Begin(0, BlendState.Opaque, SamplerState.PointClamp, null, null);
            spriteBatch.Draw(shadowRenderTargetView, new Rectangle(0, 0, 128, 128), Color.White);
            spriteBatch.End();

            var context = Device.ImmediateContext;

            context.PixelShaderResources[0] = null;
            context.PixelShaderSamplers[0] = SamplerState.LinearWrap;
        }

        /// </summary>
        void HandleInput(GameTime gameTime)
        {
            float time = (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            currentKeyboardState = keyboard.GetState();
            currentJoystickState = joystick.GetState();

            rotateDude += currentJoystickState.Triggers.Right * time * 0.2f;
            rotateDude -= currentJoystickState.Triggers.Left * time * 0.2f;
            
            if (currentKeyboardState.IsKeyDown(Keys.Q))
                rotateDude -= time * 0.2f;
            if (currentKeyboardState.IsKeyDown(Keys.E))
                rotateDude += time * 0.2f;

            if (currentKeyboardState.IsKeyDown(Keys.Escape) ||
                currentJoystickState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }
        }

        void UpdateCamera(GameTime gameTime)
        {
            float time = (float) gameTime.ElapsedGameTime.TotalMilliseconds;
 
            float pitch = -currentJoystickState.ThumbSticks.Right.Y * time * 0.001f;
            float turn = -currentJoystickState.ThumbSticks.Right.X * time * 0.001f;

            if (currentKeyboardState.IsKeyDown(Keys.Up))
                pitch += time * 0.001f;

            if (currentKeyboardState.IsKeyDown(Keys.Down))
                pitch -= time * 0.001f;

            if (currentKeyboardState.IsKeyDown(Keys.Left))
                turn += time * 0.001f;

            if (currentKeyboardState.IsKeyDown(Keys.Right))
                turn -= time * 0.001f;

            var cameraRight = Vector3.Cross(Vector3.Up, cameraForward);
            var flatFront = Vector3.Cross(cameraRight, Vector3.Up);

            var pitchMatrix = Matrix.CreateFromAxisAngle(cameraRight, pitch);
            var turnMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, turn);

            var tiltedFront = Vector3.TransformNormal(cameraForward, pitchMatrix * turnMatrix);

            if (Vector3.Dot(tiltedFront, flatFront) > 0.001f)
            {
                cameraForward = Vector3.Normalize(tiltedFront);
            }

            if (currentKeyboardState.IsKeyDown(Keys.W))
                cameraPosition += cameraForward * time * 0.1f;

            if (currentKeyboardState.IsKeyDown(Keys.S))
                cameraPosition -= cameraForward * time * 0.1f;

            if (currentKeyboardState.IsKeyDown(Keys.A))
                cameraPosition += cameraRight * time * 0.1f;

            if (currentKeyboardState.IsKeyDown(Keys.D))
                cameraPosition -= cameraRight * time * 0.1f;

            cameraPosition += cameraForward * currentJoystickState.ThumbSticks.Left.Y * time * 0.1f;
            cameraPosition -= cameraRight * currentJoystickState.ThumbSticks.Left.X * time * 0.1f;

            if (currentJoystickState.Buttons.RightStick == ButtonState.Pressed ||
                currentKeyboardState.IsKeyDown(Keys.R))
            {
                cameraPosition = new Vector3(0, 50, 50);
                cameraForward = new Vector3(0, 0, -1);
            }

            cameraForward.Normalize();

            view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraForward, Vector3.Up);

            cameraFrustum.Matrix = view * projection;
        }
    }

    #region Program

    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MainGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
