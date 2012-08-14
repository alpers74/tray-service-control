using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrayServiceControl.Debugging
{
    public interface IVsDebugger
    {
        void Attach(int pid);
        void Detach(int pid);
        bool IsAttachedTo(int pid);

        string Version { get; }
        string Solution { get; }
        string Name { get; }
    }
}
