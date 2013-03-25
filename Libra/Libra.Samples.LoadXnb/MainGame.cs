#region Using

using System;
using System.IO;
using Felis.Xnb;
using Libra.Games;
using Libra.Games.SharpDX;
using Libra.Graphics;
using Libra.Input;
using Libra.Xnb;

#endregion

namespace Libra.Samples.LoadXnb
{
    public sealed class MainGame : Game
    {
        IGamePlatform platform;

        GraphicsManager graphicsManager;

        IKeyboard keyboard;

        KeyboardState currentKeyboardState;

        Model gridModel;

        Model dudeModel;

        float rotateDude = 0.0f;

        Vector3 cameraPosition = new Vector3(0, 70, 100);
        
        Vector3 cameraForward = new Vector3(0, -0.4472136f, -0.8944272f);

        Matrix world;

        Matrix view;
        
        Matrix projection;

        public MainGame()
        {
            platform = new SdxFormGamePlatform(this)
            {
                DirectInputEnabled = true
            };
            graphicsManager = new GraphicsManager(this);
        }

        protected override void Initialize()
        {
            var exists = System.IO.File.Exists("Content/dude.xnb");

            base.Initialize();
        }

        const string XnaModelReaderName = "";

        protected override void LoadContent()
        {
            var contentManager = new ContentManager(Device);
            contentManager.TypeReaderManager.RegisterStandardTypeReaders();
            contentManager.TypeReaderManager.RegisterTypeBuilder<Vector3Builder>();
            contentManager.TypeReaderManager.RegisterTypeBuilder<RectangleBuilder>();
            contentManager.TypeReaderManager.RegisterTypeBuilder<MatrixBuilder>();
            contentManager.TypeReaderManager.RegisterTypeBuilder<BoundingSphereBuilder>();
            contentManager.TypeReaderManager.RegisterTypeBuilder<VertexBufferBuilder>();
            contentManager.TypeReaderManager.RegisterTypeBuilder<VertexDeclarationBuilder>();
            contentManager.TypeReaderManager.RegisterTypeBuilder<IndexBufferBuilder>();
            contentManager.TypeReaderManager.RegisterTypeBuilder<BasicEffectBuilder>();
            contentManager.TypeReaderManager.RegisterTypeBuilder<ModelBuilder>();
            contentManager.TypeReaderManager.RegisterTypeBuilder<Texture2DBuilder>();
            contentManager.TypeReaderManager.RegisterTypeBuilder<SpriteFontBuilder>();
            contentManager.RootDirectory = "Content";

            gridModel = contentManager.Load<Model>("grid");

            dudeModel = contentManager.Load<Model>("dude");

            var viewport = Device.ImmediateContext.Viewport;

            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f), (float) viewport.AspectRatio, 1.0f, 1000.0f);

            InitializeModel(gridModel);
            InitializeModel(dudeModel);

            keyboard = platform.CreateKeyboard();

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);

            UpdateCamera(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Device.ImmediateContext.Clear(Color.CornflowerBlue);

            world = Matrix.Identity;
            DrawModel(gridModel);

            world = Matrix.CreateRotationY(MathHelper.ToRadians(rotateDude));
            DrawModel(dudeModel);

            base.Draw(gameTime);
        }

        void InitializeModel(Model model)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                }
            }
        }

        void DrawModel(Model model)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }

                mesh.Draw(Device.ImmediateContext);
            }
        }

        void HandleInput(GameTime gameTime)
        {
            float time = (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            currentKeyboardState = keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Q))
                rotateDude -= time * 0.2f;
            if (currentKeyboardState.IsKeyDown(Keys.E))
                rotateDude += time * 0.2f;

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit();
        }

        void UpdateCamera(GameTime gameTime)
        {
            float time = (float) gameTime.ElapsedGameTime.TotalMilliseconds;

            float pitch = 0;
            float turn = 0;

            if (currentKeyboardState.IsKeyDown(Keys.Up))
                pitch += time * 0.001f;

            if (currentKeyboardState.IsKeyDown(Keys.Down))
                pitch -= time * 0.001f;

            if (currentKeyboardState.IsKeyDown(Keys.Left))
                turn += time * 0.001f;

            if (currentKeyboardState.IsKeyDown(Keys.Right))
                turn -= time * 0.001f;

            Vector3 cameraRight = Vector3.Cross(Vector3.Up, cameraForward);
            Vector3 flatFront = Vector3.Cross(cameraRight, Vector3.Up);

            Matrix pitchMatrix = Matrix.CreateFromAxisAngle(cameraRight, pitch);
            Matrix turnMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, turn);

            Vector3 tiltedFront = Vector3.TransformNormal(cameraForward, pitchMatrix *
                                                          turnMatrix);

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

            if (currentKeyboardState.IsKeyDown(Keys.R))
            {
                cameraPosition = new Vector3(0, 50, 50);
                cameraForward = new Vector3(0, 0, -1);
            }

            cameraForward.Normalize();

            view = Matrix.CreateLookAt(cameraPosition,
                                       cameraPosition + cameraForward,
                                       Vector3.Up);
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
