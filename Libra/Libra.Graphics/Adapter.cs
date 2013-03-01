#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using D3D11Device = SharpDX.Direct3D11.Device;
using D3D11DeviceCreationFlags = SharpDX.Direct3D11.DeviceCreationFlags;
using D3DFeatureLevel = SharpDX.Direct3D.FeatureLevel;
using DXGIAdapter1 = SharpDX.DXGI.Adapter1;
using DXGIFactory1 = SharpDX.DXGI.Factory1;
using SDXSharpDXException = SharpDX.SharpDXException;

#endregion

namespace Libra.Graphics
{
    public sealed class Adapter
    {
        #region OutputCollection

        public sealed class OutputCollection : IEnumerable<Output>
        {
            List<Output> items;

            public Output this[int index]
            {
                get { return items[index]; }
            }

            internal OutputCollection(int size)
            {
                items = new List<Output>();
            }

            internal void Add(Output item)
            {
                items.Add(item);
            }

            public IEnumerator<Output> GetEnumerator()
            {
                return items.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        #endregion

        public static ReadOnlyCollection<Adapter> Adapters { get; private set; }

        public static Adapter DefaultAdapter { get; private set; }

        static Adapter()
        {
            using (var factory = new DXGIFactory1())
            {
                var count = factory.GetAdapterCount1();
                var adapters = new List<Adapter>(count);

                for (int i = 0; i < count; i++)
                {
                    var adapter = new Adapter(factory.GetAdapter1(i));
                    adapters.Add(adapter);
                }

                DefaultAdapter = adapters[0];
                DefaultAdapter.IsDefaultAdapter = true;

                Adapters = new ReadOnlyCollection<Adapter>(adapters);
            }
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// DXGI_ADAPTER_DESC.Description。
        /// </remarks>
        public string Description { get; private set; }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// DXGI_ADAPTER_DESC.VendorId。
        /// </remarks>
        public int VendorId { get; private set; }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// DXGI_ADAPTER_DESC.DeviceId。
        /// </remarks>
        public int DeviceId { get; private set; }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// DXGI_ADAPTER_DESC.SubSysId。
        /// </remarks>
        public int SubSystemId { get; private set; }

        /// <summary>
        /// </summary>
        /// <remarks>
        /// DXGI_ADAPTER_DESC.Revision。
        /// </remarks>
        public int Revision { get; private set; }

        public OutputCollection Outputs { get; private set; }

        public Output PrimaryOutput { get; private set; }

        public bool IsDefaultAdapter { get; private set; }

        internal DXGIAdapter1 DXGIAdapter { get; private set; }

        internal DXGIFactory1 DXGIFactory
        {
            get { return DXGIAdapter.GetParent<DXGIFactory1>(); }
        }

        Adapter(DXGIAdapter1 dxgiAdapter)
        {
            DXGIAdapter = dxgiAdapter;

            Outputs = new OutputCollection(dxgiAdapter.Outputs.Length);

            var adapterDescription = dxgiAdapter.Description1;

            Description = adapterDescription.Description;
            VendorId = adapterDescription.VendorId;
            DeviceId = adapterDescription.DeviceId;
            SubSystemId = adapterDescription.SubsystemId;
            Revision = adapterDescription.Revision;

            for (int i = 0; i < dxgiAdapter.Outputs.Length; i++)
            {
                var output = new Output(dxgiAdapter.Outputs[i]);
                Outputs.Add(output);
            }

            if (0 < dxgiAdapter.Outputs.Length)
                PrimaryOutput = Outputs[0];
        }

        public bool IsProfileSupported(DeviceProfile profile)
        {
            D3D11Device d3d11Device = null;
            try
            {
                d3d11Device = new D3D11Device(DXGIAdapter, D3D11DeviceCreationFlags.None, (D3DFeatureLevel) profile);
                return true;
            }
            catch (SDXSharpDXException)
            {
                return false;
            }
            finally
            {
                if (d3d11Device != null)
                    d3d11Device.Dispose();
            }
        }

        #region ToString

        public override string ToString()
        {
            return Description;
        }

        #endregion
    }
}
