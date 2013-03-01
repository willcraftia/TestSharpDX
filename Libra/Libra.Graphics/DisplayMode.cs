#region Using

using System;

using DXGIDisplayModeScaling = SharpDX.DXGI.DisplayModeScaling;
using DXGIDisplayModeScanlineOrder = SharpDX.DXGI.DisplayModeScanlineOrder;
using DXGIFormat = SharpDX.DXGI.Format;
using DXGIModeDescription = SharpDX.DXGI.ModeDescription;

#endregion

namespace Libra.Graphics
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// DXGI_MODE_DESC。
    /// </remarks>
    public struct DisplayMode : IEquatable<DisplayMode>
    {
        public int Width;

        public int Height;

        public RefreshRate RefreshRate;

        public SurfaceFormat Format;

        public DisplayModeScanlineOrder ScanlineOrdering;

        public DisplayModeScaling Scaling;

        public DisplayMode(int width, int height, RefreshRate refreshRate, SurfaceFormat format)
        {
            Width = width;
            Height = height;
            RefreshRate = refreshRate;
            Format = format;
            ScanlineOrdering = DisplayModeScanlineOrder.Unspecified;
            Scaling = DisplayModeScaling.Unspecified;
        }

        internal static void FromDXGIModeDescription(ref DXGIModeDescription dxgiMode, out DisplayMode result)
        {
            result = new DisplayMode
            {
                Width = dxgiMode.Width,
                Height = dxgiMode.Height,
                RefreshRate =
                {
                    Numerator = dxgiMode.RefreshRate.Numerator,
                    Denominator = dxgiMode.RefreshRate.Denominator
                },
                Format = (SurfaceFormat) dxgiMode.Format,
                ScanlineOrdering = (DisplayModeScanlineOrder) dxgiMode.ScanlineOrdering,
                Scaling = (DisplayModeScaling) dxgiMode.Scaling
            };
        }

        internal void ToDXGIModeDescription(out DXGIModeDescription result)
        {
            result = new DXGIModeDescription
            {
                Width = Width,
                Height = Height,
                RefreshRate =
                {
                    Numerator = RefreshRate.Numerator,
                    Denominator = RefreshRate.Denominator
                },
                Format = (DXGIFormat) Format,
                ScanlineOrdering = (DXGIDisplayModeScanlineOrder) ScanlineOrdering,
                Scaling = (DXGIDisplayModeScaling) Scaling
            };
        }

        #region Equatable

        public static bool operator ==(DisplayMode value1, DisplayMode value2)
        {
            return value1.Equals(value2);
        }

        public static bool operator !=(DisplayMode value1, DisplayMode value2)
        {
            return !value1.Equals(value2);
        }

        public bool Equals(DisplayMode other)
        {
            return Width == other.Width && Height == other.Height &&
                RefreshRate == other.RefreshRate && Format == other.Format;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            return Equals((DisplayMode) obj);
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode() ^
                RefreshRate.GetHashCode() ^ Format.GetHashCode();
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return "{Width:" + Width + " Height:" + Height +
                " RefreshRate:" + RefreshRate + " Format:" + Format +
                " ScanlineOrdering:" + ScanlineOrdering +
                " Scaling:" + Scaling + "}";
        }

        #endregion
    }
}
