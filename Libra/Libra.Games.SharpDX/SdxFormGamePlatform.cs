#region Using

using System;
using System.Windows.Forms;
using Libra.Graphics;
using Libra.Graphics.SharpDX;
using Libra.Input;
using Libra.Input.SharpDX;

using SDXWRenderLoop = SharpDX.Windows.RenderLoop;

#endregion

namespace Libra.Games.SharpDX
{
    public sealed class SdxFormGamePlatform : IGamePlatform, IDisposable
    {
        #region FormGameWindow

        sealed class FormGameWindow : GameWindow
        {
            bool allowUserResizing;

            public override bool AllowUserResizing
            {
                get { return allowUserResizing; }
                set
                {
                    if (allowUserResizing == value)
                        return;

                    allowUserResizing = value;

                    if (allowUserResizing)
                    {
                        Form.FormBorderStyle = FormBorderStyle.Sizable;
                    }
                    else
                    {
                        Form.FormBorderStyle = FormBorderStyle.FixedSingle;
                    }
                }
            }

            public override Rectangle ClientBounds
            {
                get
                {
                    var size = Form.ClientSize;
                    return new Rectangle(0, 0, size.Width, size.Height);
                }
            }

            public override IntPtr Handle
            {
                get { return Form.Handle; }
            }

            internal Form Form { get; private set; }

            internal FormGameWindow(Form form)
            {
                Form = form;
                Form.ClientSizeChanged += OnClientSizeChanged;

                // デフォルトの振る舞いとしてフォームをサイズ変更不可に初期化。
                Form.FormBorderStyle = FormBorderStyle.FixedSingle;

                Title = form.Text;
            }

            protected override void SetTitle(string title)
            {
                Form.Text = Title;
            }

            void OnClientSizeChanged(object sender, EventArgs e)
            {
                OnClientSizeChanged();
            }
        }

        #endregion

        public event EventHandler Activated;

        public event EventHandler Deactivated;

        public event EventHandler Exiting;

        Game game;

        public GameWindow Window { get; private set; }

        public IGameTimer GameTimer { get; private set; }

        public IGraphicsFactory GraphicsFactory { get; private set; }

        public IInputFactory InputFactory { get; private set; }

        public Form Form { get; private set; }

        public SdxFormGamePlatform(Game game, Form form)
        {
            if (game == null) throw new ArgumentNullException("game");
            if (form == null) throw new ArgumentNullException("form");

            this.game = game;
            Form = form;
            Form.Activated += OnActivated;
            Form.Deactivate += OnDeactivated;
            Form.FormClosing += OnClosing;

            game.Services.AddService<IGamePlatform>(this);
        }

        public void Initialize()
        {
            if (Window != null)
                throw new InvalidOperationException("GameWindow already exists.");

            Window = new FormGameWindow(Form);
            GameTimer = new SdxGameTimer();
            GraphicsFactory = new SdxGraphicsFactory();
            InputFactory = new SdxInputFactory();
        }

        public void Run(TickCallback tick)
        {
            SDXWRenderLoop.Run(Form, new SDXWRenderLoop.RenderCallback(tick));
        }

        public void Exit()
        {
            Form.Close();
        }

        void OnActivated(object sender, EventArgs e)
        {
            if (Activated != null)
                Activated(this, EventArgs.Empty);
        }

        void OnDeactivated(object sender, EventArgs e)
        {
            if (Deactivated != null)
                Deactivated(this, EventArgs.Empty);
        }

        void OnClosing(object sender, FormClosingEventArgs e)
        {
            if (Exiting != null)
                Exiting(this, EventArgs.Empty);
        }

        #region IDisposable

        bool disposed;

        ~SdxFormGamePlatform()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                if (InputFactory != null)
                    (InputFactory as SdxInputFactory).Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
