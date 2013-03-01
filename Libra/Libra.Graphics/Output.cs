#region Using

using System;
using System.Collections.Generic;

using DXGIDisplayModeEnumerationFlags = SharpDX.DXGI.DisplayModeEnumerationFlags;
using DXGIDisplayModeScanlineOrder = SharpDX.DXGI.DisplayModeScanlineOrder;
using DXGIFormat = SharpDX.DXGI.Format;
using DXGIModeDescription = SharpDX.DXGI.ModeDescription;
using DXGIOutput = SharpDX.DXGI.Output;

#endregion

namespace Libra.Graphics
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Output
    {
        public string DeviceName { get; private set; }

        public Rectangle DesktopCoordinates { get; private set; }

        public bool AttachedToDesktop { get; private set; }

        public IntPtr Monitor { get; private set; }

        DXGIOutput dxgiOutput;

        internal Output(DXGIOutput dxgiOutput)
        {
            this.dxgiOutput = dxgiOutput;

            var outpuDescription = dxgiOutput.Description;

            DeviceName = outpuDescription.DeviceName;
            DesktopCoordinates = new Rectangle
            {
                Left = outpuDescription.DesktopBounds.Left,
                Top = outpuDescription.DesktopBounds.Top,
                Right = outpuDescription.DesktopBounds.Right,
                Bottom = outpuDescription.DesktopBounds.Bottom
            };
            AttachedToDesktop = outpuDescription.IsAttachedToDesktop;
            Monitor = outpuDescription.MonitorHandle;
        }

        // メモ
        //
        // 表示モードの列挙が必要な場合にのみ、それらを列挙するものとし、
        // XNA の GraphicsAdapter のような事前列挙による保持はしない。
        // 殆どの場合において、GetClosestMatchingMode による最適表示モードの検索で十分であり、
        // 事前列挙の必要性が少ない。

        public DisplayMode[] GetModes(SurfaceFormat format, EnumerateDisplayModes flags)
        {
            var dxgiModes = dxgiOutput.GetDisplayModeList((DXGIFormat) format, (DXGIDisplayModeEnumerationFlags) flags);

            var result = new DisplayMode[dxgiModes.Length];
            for (int i = 0; i < dxgiModes.Length; i++)
                DisplayMode.FromDXGIModeDescription(ref dxgiModes[i], out result[i]);

            return result;
        }

        public void GetClosestMatchingMode(Device device, ref DisplayMode preferredMode, out DisplayMode result)
        {
            DXGIModeDescription dxgiModeToMatch;
            preferredMode.ToDXGIModeDescription(out dxgiModeToMatch);

            DXGIModeDescription dxgiResult;
            dxgiOutput.GetClosestMatchingMode(device.D3D11Device, dxgiModeToMatch, out dxgiResult);

            DisplayMode.FromDXGIModeDescription(ref dxgiResult, out result);
        }
    }
}
