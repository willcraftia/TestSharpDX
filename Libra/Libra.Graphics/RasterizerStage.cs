#region Using

using System;

#endregion

namespace Libra.Graphics
{
    public abstract class RasterizerStage
    {
        RasterizerState rasterizerState;

        Viewport viewport;

        Rectangle scissorRectangle;

        public RasterizerState RasterizerState
        {
            get { return rasterizerState; }
            set
            {
                if (rasterizerState == value) return;

                rasterizerState = value;

                OnRasterizerStateChanged();
            }
        }

        public Viewport Viewport
        {
            get { return viewport; }
            set
            {
                viewport = value;

                OnViewportChanged();
            }
        }

        public Rectangle ScissorRectangle
        {
            get { return scissorRectangle; }
            set
            {
                scissorRectangle = value;

                OnScissorRectangleChanged();
            }
        }

        protected RasterizerStage() { }

        protected abstract void OnRasterizerStateChanged();

        protected abstract void OnViewportChanged();

        protected abstract void OnScissorRectangleChanged();
    }
}
