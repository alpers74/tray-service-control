using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
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
                try
                {
                    var processes = _dte.Debugger.LocalProcesses.OfType<Process>().ToArray();
                    var process = processes.SingleOrDefault(x => x.ProcessID == pid);
                    if (process != null)
                    {
                        process.Attach();
                    }
                }
                catch (COMException)
                {
                }
            }
        }

        public void Detach(int pid)
        {
            using (new MessageFilter())
            {
                try
                {
                    if (_dte.Debugger.DebuggedProcesses == null)
                        return; 
                    var processes = _dte.Debugger.DebuggedProcesses.OfType<Process>().ToArray();
                    var process = processes.SingleOrDefault(x => x.ProcessID == pid);
                    if (process != null)
                    {
                        process.Detach(false);
                    }

                //_dte.Debugger.DetachAll();
                }
                catch (COMException)
                {
                }
        }
        }

        public bool IsAttachedTo(int pid)
        {
            using (new MessageFilter())
            {
                try
                {
                    if (_dte.Debugger.DebuggedProcesses == null)
                        return false;
                    var processes = _dte.Debugger.DebuggedProcesses.OfType<Process>().ToArray();
                    return processes.Any(x => x.ProcessID == pid);
                }
                catch (COMException)
                {
                    return false;
                }
            }
        }
        
        public string Version 
        { 
            get 
            {
                try
                {

                    using (new MessageFilter())
                    {
                        return _dte.Version;
                    }
                }
                catch (COMException)
                {
                    return "";
                }
            } 
        }

        public string Solution 
        { 
            get 
            {
                try
                {
                    using (new MessageFilter())
                    {
                        return _dte.Solution.IsOpen ? Path.GetFileNameWithoutExtension(_dte.Solution.FullName) : "No Solution";
                    }
                }
                catch (COMException)
                {
                    return "";
                }
            } 
        }

        public string Name 
        { 
            get
            {
                try
                {
                    using (new MessageFilter())
                    {
                        return _dte.Name;
                    }
                }
                catch(COMException)
                {
                    return "";
                }
            } 
        }
    }
}