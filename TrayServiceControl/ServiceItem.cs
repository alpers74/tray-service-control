using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Windows;
using System.Windows.Input;
using TrayServiceControl.Debugging;
using TrayServiceControl.MVVM;
using TrayServiceControl.Service;


namespace TrayServiceControl
{
    public class ServiceItem : ObservableObject
    {
        private string _attachName;
        private string _startName;
        private ServiceController Service { get; set; }
        private int? _pid;
        private IVisibilitySettings _settings;

        // Avoid calling on UI thread.
        public ServiceItem(ServiceController service, IVisibilitySettings settings)
        {
            _settings = settings;
            Service = service;

            DisplayName = service.DisplayName;
            ServiceName = service.ServiceName;
            Status = service.Status.ToString();

            AttachName = "Attach";// TODO: attached or not, TODO part of the command?

            var process = ServiceControl.ServiceToProcess(Service);

            AttachCommand = new ActionCommand(() => { },
                Service.Status == ServiceControllerStatus.Running &&
                VsEnvironment.Debuggers.Any());

            var debugger = VsEnvironment.Debuggers.FirstOrDefault(p => p.IsAttachedTo(process.Id));

            DetachCommand = new ActionCommand(() => { Detach(debugger); },
                Service.Status == ServiceControllerStatus.Running &&
                VsEnvironment.Debuggers.Any());

            StartName = Service.Status == ServiceControllerStatus.Running ? "Stop" : "Start"; // TODO: intermediate states
            StartServiceCommand = new ActionCommand(Start);
            StopServiceCommand = new ActionCommand(Stop);

            SetPid();
            if (Service.Status == ServiceControllerStatus.Running)
            {
                StartVisibility = System.Windows.Visibility.Collapsed;
                StopVisibility = System.Windows.Visibility.Visible;
                
                AttachVisibility = (debugger == null) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                DetachVisibility = (debugger == null) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }
            else
            {
                StartVisibility = System.Windows.Visibility.Visible;
                StopVisibility = System.Windows.Visibility.Collapsed;
                
                AttachVisibility = System.Windows.Visibility.Visible;
                DetachVisibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Stop()
        {

            if (Service.CanStop)
            {
                // Perhaps use yield and observable to make this async
                Service.Stop();
                // TODO: This could take time - make async
                Service.WaitForStatus(ServiceControllerStatus.Stopped);

                StartVisibility = System.Windows.Visibility.Visible;
                StopVisibility = System.Windows.Visibility.Collapsed;

                AttachVisibility = System.Windows.Visibility.Visible;
                DetachVisibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void Start()
        {
            if (Service.Status == ServiceControllerStatus.Stopped)
            {
                Service.Start();
                // TODO: This could take time - make async
                Service.WaitForStatus(ServiceControllerStatus.Running);

                StartVisibility = System.Windows.Visibility.Collapsed;
                StopVisibility = System.Windows.Visibility.Visible;

                AttachVisibility = System.Windows.Visibility.Visible;
                DetachVisibility = System.Windows.Visibility.Collapsed;
            }
        }

        public string DisplayName { get; private set; }
        public string ServiceName { get; private set; }
        public string Status { get; private set; }

        public string Pid
        {
            get
            {
                return _pid.HasValue ? _pid.ToString() : "";
            }
        }

        private void SetPid()
        {
            var process = ServiceControl.ServiceToProcess(Service);
            if (process != null && process.Id != 0)
                _pid = process.Id;
            else
                _pid = null;
        }

        public ActionCommand AttachCommand { get; private set; }
        public ActionCommand StartServiceCommand { get; private set; }
        public ActionCommand DetachCommand { get; private set; }
        public ActionCommand StopServiceCommand { get; private set; }

        // TODO: Remove not required.
        public bool Visibility
        {
            get { return _settings[Service.DisplayName]; }
            set
            {
                _settings[Service.DisplayName] = value;
                _settings.Save();
            }
        }

        private Visibility _startVisibility;
        public Visibility StartVisibility
        {
            get { return _startVisibility; }
            set { _startVisibility = value; OnPropertyChanged("StartVisibility"); }
        }        
        
        private Visibility _stopVisibility;
        public Visibility StopVisibility
        {
            get { return _stopVisibility; }
            set { _stopVisibility = value; OnPropertyChanged("StopVisibility"); }
        }

        private Visibility _attachVisibility;
        public Visibility AttachVisibility
        {
            get { return _attachVisibility; }
            set { _attachVisibility = value; OnPropertyChanged("AttachVisibility"); }
        }
        private Visibility _detachVisibility;
        public Visibility DetachVisibility
        {
            get { return _detachVisibility; }
            set { _detachVisibility = value; OnPropertyChanged("DetachVisibility"); }
        }

        public string AttachName
        {
            get { return _attachName; }
            set { _attachName = value; OnPropertyChanged("AttachName"); }
        }

        public string StartName
        {
            get { return _startName; }
            set { _startName = value; OnPropertyChanged("StartName"); }
        }

        private Process Process
        {
            get
            {
                return (Service.Status == ServiceControllerStatus.Running) ?
                    ServiceControl.ServiceToProcess(Service) : null;
            }
        }

        public class DebuggerContextMenuItem
        {
            public string DebuggerName { get; set; }
            public ICommand Attach { get; set; }
        }

        ObservableCollection<DebuggerContextMenuItem> _col = new ObservableCollection<DebuggerContextMenuItem>();
        public ObservableCollection<DebuggerContextMenuItem> DebuggerContextMenu {
            get {

                var dbg = VsEnvironment.Debuggers.Select(
                    s => new DebuggerContextMenuItem
                    {
                        DebuggerName = s.Solution + " - " + s.Name + " " + s.Version,
                        Attach = new ActionCommand(() =>
                            {
                                
                                Attach(s);
                                AttachName = "Detach";
                            }),
                    });
                _col.Clear();
                foreach(var d in dbg)
                {
                    _col.Add(d);
                }
                return _col;
            }
        }
        private void Attach(IVsDebugger debugger)
        {
            var process = Process;
            if (process != null)
            {
                if (!VsEnvironment.IsDebuggerPresent(process))
                {
                    try
                    {
                        debugger.Attach(process.Id);
                        AttachVisibility = System.Windows.Visibility.Collapsed;
                        DetachVisibility = System.Windows.Visibility.Visible;

                        DetachCommand = new ActionCommand(() => { Detach(debugger); },
                            Service.Status == ServiceControllerStatus.Running &&
                            VsEnvironment.Debuggers.Any());
                        OnPropertyChanged("DetachCommand");
                    }
                    catch (COMException e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
            ((ActionCommand)AttachCommand).Enabled = 
                Service.Status == ServiceControllerStatus.Running &&
                VsEnvironment.Debuggers.Any();
        }

        private void Detach(IVsDebugger debugger)
        {
            var process = Process;
            if (process != null)
            {
                if (VsEnvironment.IsDebuggerPresent(process))
                {
                    try
                    {
                        debugger.Detach(process.Id);
                        AttachVisibility = System.Windows.Visibility.Visible;
                        DetachVisibility = System.Windows.Visibility.Collapsed;
                    }
                    catch (COMException e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
            ((ActionCommand)AttachCommand).Enabled = 
                Service.Status == ServiceControllerStatus.Running &&
                VsEnvironment.Debuggers.Any();
        }

    }
}
