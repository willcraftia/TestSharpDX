#region Using

using System;
using System.Collections.Generic;
using Libra.Games;
using Libra.Games.SharpDX;
using Libra.Graphics;
using Libra.Input;

using SDXWRenderForm = SharpDX.Windows.RenderForm;

#endregion

namespace Libra.Samples.Primitives3D
{
    public sealed class MainGame : Game
    {
        IGamePlatform platform;

        GraphicsManager graphicsManager;

        IKeyboard keyboard;

        IMouse mouse;

        IJoystick joystick;

        KeyboardState currentKeyboardState;

        KeyboardState lastKeyboardState;

        JoystickState currentJoystickState;

        JoystickState lastJoystickState;

        MouseState currentMouseState;

        MouseState lastMouseState;

        List<GeometricPrimitive> primitives = new List<GeometricPrimitive>();

        int currentPrimitiveIndex = 0;

        RasterizerState wireFrameState;

        List<Color> colors = new List<Color>
        {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.White,
            Color.Black,
        };

        int currentColorIndex = 0;

        bool isWireframe;

        public MainGame()
        {
            platform = new SdxFormGamePlatform(this, new SDXWRenderForm());
            graphicsManager = new GraphicsManager(this);
        }

        protected override void LoadContent()
        {
            primitives.Add(new CubePrimitive(Device));
            primitives.Add(new SpherePrimitive(Device));
            primitives.Add(new CylinderPrimitive(Device));
            primitives.Add(new TorusPrimitive(Device));
            primitives.Add(new TeapotPrimitive(Device));

            wireFrameState = new RasterizerState()
            {
                FillMode = FillMode.Wireframe,
                CullMode = CullMode.None,
            };

            keyboard = platform.CreateKeyboard();
            mouse = platform.CreateMouse();

            // TODO
            joystick = Libra.Input.SharpDX.SdxJoystick.Create();

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            HandleInput();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var context = Device.ImmediateContext;

            context.Clear(Color.CornflowerBlue);

            if (isWireframe)
            {
                context.RasterizerStage.RasterizerState = wireFrameState;
            }
            else
            {
                context.RasterizerStage.RasterizerState = RasterizerState.CullBack;
            }

            float time = (float) gameTime.TotalGameTime.TotalSeconds;
            float yaw = time * 0.4f;
            float pitch = time * 0.7f;
            float roll = time * 1.1f;

            Vector3 cameraPosition = new Vector3(0, 0, 2.5f);

            float aspect = context.RasterizerStage.Viewport.AspectRatio;

            var world = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            var view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 10);

            var currentPrimitive = primitives[currentPrimitiveIndex];
            var color = colors[currentColorIndex];

            currentPrimitive.Draw(context, world, view, projection, color);

            context.RasterizerStage.RasterizerState = RasterizerState.CullBack;

            base.Draw(gameTime);
        }

        void HandleInput()
        {
            lastKeyboardState = currentKeyboardState;
            lastJoystickState = currentJoystickState;
            lastMouseState = currentMouseState;

            currentKeyboardState = keyboard.GetState();
            currentJoystickState = joystick.GetState();
            currentMouseState = mouse.GetState();

            var test = joystick.GetState();

            if (IsPressed(Keys.Escape, Buttons.Back))
            {
                Exit();
            }

            Viewport viewport = Device.ImmediateContext.RasterizerStage.Viewport;
            int halfWidth = (int) viewport.Width / 2;
            int halfHeight = (int) viewport.Height / 2;
            Rectangle topOfScreen = new Rectangle(0, 0, (int) viewport.Width, (int) halfHeight);
            if (IsPressed(Keys.A, Buttons.A) || LeftMouseIsPressed(topOfScreen))
            {
                currentPrimitiveIndex = (currentPrimitiveIndex + 1) % primitives.Count;
            }

            Rectangle botLeftOfScreen = new Rectangle(0, halfHeight, halfWidth, halfHeight);
            if (IsPressed(Keys.B, Buttons.B) || LeftMouseIsPressed(botLeftOfScreen))
            {
                currentColorIndex = (currentColorIndex + 1) % colors.Count;
            }

            Rectangle botRightOfScreen = new Rectangle(halfWidth, halfHeight, halfWidth, halfHeight);
            if (IsPressed(Keys.Y, Buttons.Y) || LeftMouseIsPressed(botRightOfScreen))
            {
                isWireframe = !isWireframe;
            }
        }

        bool IsPressed(Keys key, Buttons button)
        {
            return currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key) ||
                   currentJoystickState.IsButtonDown(button) && lastJoystickState.IsButtonUp(button);
        }

        bool LeftMouseIsPressed(Rectangle rect)
        {
            return currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton != ButtonState.Pressed &&
                rect.Contains(currentMouseState.X, currentMouseState.Y);
        }
    }

    #region Program

    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
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
