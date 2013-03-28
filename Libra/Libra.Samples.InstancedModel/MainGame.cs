#region Using

using System;
using System.Collections.Generic;
using Libra.Games;
using Libra.Games.SharpDX;
using Libra.Graphics;
using Libra.Input;
using Libra.Xnb;

#endregion

namespace Libra.Samples.InstancedModel
{
    public sealed class MainGame : Game
    {
        public enum InstancingTechnique
        {
            HardwareInstancing,
            NoInstancing,
            NoInstancingOrStateBatching
        }

        IGamePlatform platform;

        GraphicsManager graphics;

        XnbManager content;

        SpriteBatch spriteBatch;

        SpriteFont spriteFont;

        InstancingTechnique instancingTechnique = InstancingTechnique.HardwareInstancing;

        const int InitialInstanceCount = 1000;

        List<SpinningInstance> instances;
        
        Matrix[] instanceTransforms;
        
        Model instancedModel;
        
        Matrix[] instancedModelBones;
        
        VertexBuffer instanceVertexBuffer;

        static VertexDeclaration instanceVertexDeclaration = new VertexDeclaration(
            new InputElement("TRANSFORM", InputElementFormat.Vector4, 0,  0, true, 1),
            new InputElement("TRANSFORM", InputElementFormat.Vector4, 1, 16, true, 1),
            new InputElement("TRANSFORM", InputElementFormat.Vector4, 2, 32, true, 1),
            new InputElement("TRANSFORM", InputElementFormat.Vector4, 3, 48, true, 1)
            );

        int frameRate;
        
        int frameCounter;
        
        TimeSpan elapsedTime;

        IKeyboard keyboard;

        IJoystick joystick;

        KeyboardState lastKeyboardState;
        
        JoystickState lastGamePadState;
        
        KeyboardState currentKeyboardState;
        
        JoystickState currentGamePadState;


        public MainGame()
        {
            platform = new SdxFormGamePlatform(this)
            {
                DirectInputEnabled = true
            };
            graphics = new GraphicsManager(this);

            content = new XnbManager(Services, "Content");

            IsFixedTimeStep = false;

            graphics.SynchronizeWithVerticalRetrace = false;

            instances = new List<SpinningInstance>();

            for (int i = 0; i < InitialInstanceCount; i++)
                instances.Add(new SpinningInstance());
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(Device.ImmediateContext);
            
            spriteFont = content.Load<SpriteFont>("Font");

            instancedModel = content.Load<Model>("Cats");
            instancedModelBones = new Matrix[instancedModel.Bones.Count];
            instancedModel.CopyAbsoluteBoneTransformsTo(instancedModelBones);

            keyboard = platform.CreateKeyboard();
            joystick = platform.CreateJoystick();
        }

        protected override void Update(GameTime gameTime)
        {
            HandleInput();

            foreach (var instance in instances)
            {
                instance.Update(gameTime);
            }

            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            } 
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            var context = Device.ImmediateContext;

            context.Clear(Color.CornflowerBlue);

            var view = Matrix.CreateLookAt(new Vector3(0, 0, 15),
                                              Vector3.Zero, Vector3.Up);

            var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, context.Viewport.AspectRatio, 1,  100);

            context.BlendState = BlendState.Opaque;
            context.DepthStencilState = DepthStencilState.Default;

            Array.Resize(ref instanceTransforms, instances.Count);

            for (int i = 0; i < instances.Count; i++)
            {
                instanceTransforms[i] = instances[i].Transform;
            }

            switch (instancingTechnique)
            {
                case InstancingTechnique.HardwareInstancing:
                    DrawModelHardwareInstancing(instancedModel, instancedModelBones, instanceTransforms, view, projection);
                    break;

                case InstancingTechnique.NoInstancing:
                    DrawModelNoInstancing(instancedModel, instancedModelBones, instanceTransforms, view, projection);
                    break;

                case InstancingTechnique.NoInstancingOrStateBatching:
                    DrawModelNoInstancingOrStateBatching(instancedModel, instancedModelBones, instanceTransforms, view, projection);
                    break;
            }

            DrawOverlayText();
            
            frameCounter++;

            base.Draw(gameTime);
        }

        void DrawModelHardwareInstancing(Model model, Matrix[] modelBones, Matrix[] instances, Matrix view, Matrix projection)
        {
            var context = Device.ImmediateContext;

            if (instances.Length == 0)
                return;

            if ((instanceVertexBuffer == null) ||
                (instances.Length > instanceVertexBuffer.VertexCount))
            {
                if (instanceVertexBuffer != null)
                    instanceVertexBuffer.Dispose();

                instanceVertexBuffer = Device.CreateVertexBuffer();
                instanceVertexBuffer.Usage = ResourceUsage.Dynamic;
                instanceVertexBuffer.Initialize(instanceVertexDeclaration, instances.Length);
            }

            instanceVertexBuffer.SetData(context, instances, 0, instances.Length, SetDataOptions.Discard);

            foreach (var mesh in model.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    //GraphicsDevice.SetVertexBuffers(
                    //    new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                    //    new VertexBufferBinding(instanceVertexBuffer, 0, 1)
                    //);

                    context.IndexBuffer = meshPart.IndexBuffer;

                    //Effect effect = meshPart.Effect;

                    //effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];

                    //effect.Parameters["World"].SetValue(modelBones[mesh.ParentBone.Index]);
                    //effect.Parameters["View"].SetValue(view);
                    //effect.Parameters["Projection"].SetValue(projection);

                    //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    //{
                    //    pass.Apply();

                    //    GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                    //                                           meshPart.NumVertices, meshPart.StartIndex,
                    //                                           meshPart.PrimitiveCount, instances.Length);
                    //}
                }
            }
        }

        void DrawModelNoInstancing(Model model, Matrix[] modelBones, Matrix[] instances, Matrix view, Matrix projection)
        {
            var context = Device.ImmediateContext;

            foreach (var mesh in model.Meshes)
            {
                foreach (var meshPart in mesh.MeshParts)
                {
                    //context.SetVertexBuffer(meshPart.VertexBuffer, meshPart.VertexOffset);
                    context.IndexBuffer = meshPart.IndexBuffer;

                    //Effect effect = meshPart.Effect;

                    //effect.CurrentTechnique = effect.Techniques["NoInstancing"];

                    //effect.Parameters["View"].SetValue(view);
                    //effect.Parameters["Projection"].SetValue(projection);

                    //EffectParameter transformParameter = effect.Parameters["World"];

                    //for (int i = 0; i < instances.Length; i++)
                    //{
                    //    transformParameter.SetValue(modelBones[mesh.ParentBone.Index] * instances[i]);

                    //    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    //    {
                    //        pass.Apply();

                    //        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0,
                    //                                             meshPart.NumVertices, meshPart.StartIndex,
                    //                                             meshPart.PrimitiveCount);
                    //    }
                    //}
                }
            }
        }

        void DrawModelNoInstancingOrStateBatching(Model model, Matrix[] modelBones, Matrix[] instances, Matrix view, Matrix projection)
        {
            for (int i = 0; i < instances.Length; i++)
            {
                //foreach (var mesh in model.Meshes)
                //{
                //    foreach (Effect effect in mesh.Effects)
                //    {
                //        effect.CurrentTechnique = effect.Techniques["NoInstancing"];

                //        effect.Parameters["World"].SetValue(modelBones[mesh.ParentBone.Index] * instances[i]);
                //        effect.Parameters["View"].SetValue(view);
                //        effect.Parameters["Projection"].SetValue(projection);
                //    }

                //    mesh.Draw();
                //}
            }
        }

        void DrawOverlayText()
        {
            string text = string.Format("Frames per second: {0}\n" +
                                        "Instances: {1}\n" +
                                        "Technique: {2}\n\n" +
                                        "A = Change technique\n" +
                                        "X = Add instances\n" +
                                        "Y = Remove instances\n",
                                        frameRate,
                                        instances.Count,
                                        instancingTechnique);

            spriteBatch.Begin();

            spriteBatch.DrawString(spriteFont, text, new Vector2(65, 65), Color.Black);
            spriteBatch.DrawString(spriteFont, text, new Vector2(64, 64), Color.White);

            spriteBatch.End();
        }

        void HandleInput()
        {
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;

            currentKeyboardState = keyboard.GetState();
            currentGamePadState = joystick.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape) ||
                currentGamePadState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            int instanceChangeRate = Math.Max(instances.Count / 100, 1);

            if (currentKeyboardState.IsKeyDown(Keys.X) ||
                currentGamePadState.Buttons.X == ButtonState.Pressed)
            {
                for (int i = 0; i < instanceChangeRate; i++)
                {
                    instances.Add(new SpinningInstance());
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.Y) ||
                currentGamePadState.Buttons.Y == ButtonState.Pressed)
            {
                for (int i = 0; i < instanceChangeRate; i++)
                {
                    if (instances.Count == 0)
                        break;

                    instances.RemoveAt(instances.Count - 1);
                }
            }

            if ((currentKeyboardState.IsKeyDown(Keys.A) &&
                 lastKeyboardState.IsKeyUp(Keys.A)) ||
                (currentGamePadState.Buttons.A == ButtonState.Pressed &&
                 lastGamePadState.Buttons.A == ButtonState.Released))
            {
                instancingTechnique++;

                if (instancingTechnique > InstancingTechnique.NoInstancingOrStateBatching)
                    instancingTechnique = 0;
            }
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
