using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace TrayServiceControl.Debugging
{
    internal class RunningObjectTable : IDisposable
    {
        private IRunningObjectTable _rot;

        public static RunningObjectTable Get()
        {
            IRunningObjectTable rot;
            int retVal = Ole32Methods.GetRunningObjectTable(0, out rot);
            if (retVal != 0)
                throw new InvalidOperationException("Failed to obtain ROT from Ole32");
            return new RunningObjectTable(rot);
        }

        private RunningObjectTable(IRunningObjectTable rot)
        {
            _rot = rot;
        }

        public void Dispose()
        {
            // TODO: pattern?
            Marshal.ReleaseComObject(_rot);
        }

        public void VisitRunning(Action<IMoniker> visitor)
        {
            // TODO: null check
            IEnumMoniker enumMoniker;
            _rot.EnumRunning(out enumMoniker);

            IntPtr fetched = IntPtr.Zero;
            IMoniker[] moniker = new IMoniker[1];
            while (enumMoniker.Next(1, moniker, fetched) == 0)
            {
                visitor(moniker[0]);
            }
        }

        public object GetObject(IMoniker moniker)
        {
            // null check
            object result;
            _rot.GetObject(moniker, out result);
            // TODO: error checkin?
            return result;
        }
    }

}
