using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Input;
using TrayServiceControl.MVVM;

namespace TrayServiceControl
{
    class MainViewModel : ObservableObject
    {
        private ICommand _quitCommand;
        private IVisibilitySettings _settings;
        Options _options;

        public MainViewModel(IVisibilitySettings settings)
        {
            _settings = settings;
            _quitCommand = new ActionCommand(() =>
                {
                    var evt = QuitEvent;
                    if (evt != null) evt();
                });

            Services = new ObservableCollection<ServiceItem>();
            FilteredServices = new ObservableCollection<ServiceItem>();
            OptionsCommand = new ActionCommand(() =>
                {
                    if (_options == null || _options.IsClosed)
                        _options = new Options(new OptionsViewModel(_settings));

                    if (!_options.IsVisible)
                        _options.Show();
                    else
                        _options.BringIntoView();
                });

            ThreadPool.QueueUserWorkItem(cb =>
                {
                    var services = ServiceController.GetServices();

                    // Force creation of items with ToArray()
                    var items = services.Select(
                        service => new ServiceItem(service, settings)).ToArray();

                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            foreach (var item in items)
                            {
                                Services.Add(item);
                                if (item.Visibility)
                                    FilteredServices.Add(item);
                            }
                            ServicesPopulated = true;
                        }));
                });
        }


        public ICommand QuitCommand
        {
            get { return _quitCommand; }
        }

        public event Action QuitEvent;

        public ObservableCollection<ServiceItem> Services { get; private set; }

        public ObservableCollection<ServiceItem> FilteredServices { get; private set; }

        public ActionCommand OptionsCommand { get; set; }

        internal void RefreshMenu()
        {
            FilteredServices.Clear();
            var items = Services.Where(s => s.Visibility);
            foreach (var s in items)
                FilteredServices.Add(s);
            
        }

        public bool ServicesPopulated { get; set; }
    }
}
