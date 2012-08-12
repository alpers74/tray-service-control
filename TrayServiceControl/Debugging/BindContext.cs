using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;


namespace TrayServiceControl.Debugging
{
    internal class BindContext : IDisposable
    {
        IBindCtx _bindCtx;
        public BindContext()
        {
            if (Ole32Methods.CreateBindCtx(0, out _bindCtx) != 0)
                throw new InvalidOperationException();
            // TODO: throw?
        }

        public IBindCtx Value { get { return _bindCtx; } }

        public void Dispose()
        {
            Marshal.ReleaseComObject(_bindCtx);
        }
    }

}
