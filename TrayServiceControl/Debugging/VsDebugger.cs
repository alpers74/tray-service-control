using System.IO;
using System.Linq;
using EnvDTE;

namespace TrayServiceControl.Debugging
{
    internal class VsDebugger : IVsDebugger
    {
        DTE _dte;
        public VsDebugger(DTE dte)
        {
            _dte = dte;
        }

        public void Attach(int pid)
        {
            using (new MessageFilter())
            {
                var processes = _dte.Debugger.LocalProcesses.OfType<Process>().ToArray();
                var process = processes.SingleOrDefault(x => x.ProcessID == pid);
                if (process != null)
                    process.Attach();
            }
        }

        public void Detach(int pid)
        {
            using (new MessageFilter())
            {
                var processes = _dte.Debugger.DebuggedProcesses.OfType<Process>().ToArray();
                var process = processes.SingleOrDefault(x => x.ProcessID == pid);
                if (process != null)
                    process.Detach();

                //_dte.Debugger.DetachAll();
            }
        }

        public bool IsAttachedTo(int pid)
        {
            using (new MessageFilter())
            {
                var processes = _dte.Debugger.DebuggedProcesses.OfType<Process>().ToArray();
                return processes.Any(x => x.ProcessID == pid);
            }
        }
        
        public string Version { get { using (new MessageFilter()) { return _dte.Version; } } }
        public string Solution { get { using (new MessageFilter()) { return 
            _dte.Solution.IsOpen 
            ? Path.GetFileNameWithoutExtension(_dte.Solution.FullName) : "No Solution"; } } }
        public string Name { get { using (new MessageFilter()) { return _dte.Name; } } }
    }

}
