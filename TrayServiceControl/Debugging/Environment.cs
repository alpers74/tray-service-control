using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using System.IO;

namespace TrayServiceControl.Debugging
{
    public class VsEnvironment
    {
        public static bool IsDebuggerPresent(System.Diagnostics.Process proc)
        {
            bool result;
            Kernel32Methods.CheckRemoteDebuggerPresent(proc.Handle, out result);
            return result;
        }

        public static IEnumerable<IVsDebugger> Debuggers
        {
            get
            {
                List<IVsDebugger> result = new List<IVsDebugger>();
                using (var rot = RunningObjectTable.Get())
                {
                    rot.VisitRunning(moniker =>
                    {
                        string displayName;
                        using (var ctx = new BindContext())
                        {
                            moniker.GetDisplayName(ctx.Value, null, out displayName);
                        }
                        if (displayName.StartsWith("!VisualStudio."))
                        {
                            var dte = rot.GetObject(moniker) as DTE;
                            if (dte != null)
                                result.Add(new VsDebugger(dte));
                        }
                    });
                }
                return result;
            }
        }
    }
}
