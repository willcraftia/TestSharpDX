﻿#region Using

using System;
using System.Collections.Generic;
using Libra.Games;
using Libra.Games.SharpDX;
using Libra.Graphics;
using Libra.Input;
using Libra.Xnb;

#endregion

namespace Libra.Samples.Primitives3D
{
    public sealed class MainGame : Game
    {
        IGamePlatform platform;

        GraphicsManager graphicsManager;

        SpriteBatch spriteBatch;

        SpriteFont spriteFont;

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
            platform = new SdxFormGamePlatform(this)
            {
                DirectInputEnabled = true
            };
            graphicsManager = new GraphicsManager(this);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Device.ImmediateContext);

            var xnbManager = new XnbManager(Services);
            xnbManager.RootDirectory = "Content";

            spriteFont = xnbManager.Load<SpriteFont>("hudFont");

            primitives.Add(new CubePrimitive(Device));
            primitives.Add(new SpherePrimitive(Device));
            primitives.Add(new CylinderPrimitive(Device));
            primitives.Add(new TorusPrimitive(Device));
            primitives.Add(new TeapotPrimitive(Device));

            keyboard = platform.CreateKeyboard();
            mouse = platform.CreateMouse();
            joystick = platform.CreateJoystick();

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
                context.RasterizerState = RasterizerState.Wireframe;
            }
            else
            {
                context.RasterizerState = RasterizerState.CullBack;
            }

            float time = (float) gameTime.TotalGameTime.TotalSeconds;
            float yaw = time * 0.4f;
            float pitch = time * 0.7f;
            float roll = time * 1.1f;

            var cameraPosition = new Vector3(0, 0, 2.5f);

            var aspect = context.Viewport.AspectRatio;

            var world = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            var view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 10);

            var currentPrimitive = primitives[currentPrimitiveIndex];
            var color = colors[currentColorIndex];

            currentPrimitive.Draw(context, world, view, projection, color);

            context.RasterizerState = RasterizerState.CullBack;

            var text = "A or tap top of screen = Change primitive\n" +
                       "B or tap bottom left of screen = Change color\n" +
                       "Y or tap bottom right of screen = Toggle wireframe";

            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, text, new Vector2(48, 48), Color.White);
            spriteBatch.End();

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

            if (IsPressed(Keys.Escape, Buttons.Back))
            {
                Exit();
            }

            Viewport viewport = Device.ImmediateContext.Viewport;
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
