using System.Collections.ObjectModel;
using System.Linq;
using TrayServiceControl.Service;

namespace TrayServiceControl
{
    public class OptionsViewModel
    {
        public OptionsViewModel(IVisibilitySettings settings)
        {
            var control = new ServiceControl();
            var items = control.Services.Select(item => new ServiceSelectionItem(settings) 
            { 
                DisplayName = item.DisplayName, 
                ServiceName = item.ServiceName 
            });
            Services = new ObservableCollection<ServiceSelectionItem>(items);
        }

        public class ServiceSelectionItem
        {
            private IVisibilitySettings _settings;
            public ServiceSelectionItem(IVisibilitySettings settings)
            {
                _settings = settings;
            }

            public string DisplayName { get; set; }
            public string ServiceName { get; set; }


            public bool ShowInMenu
            {
                get { return _settings[DisplayName]; }
                set
                {
                    _settings[DisplayName] = value;
                    _settings.Save();
                }
            }
        }
        public ObservableCollection<ServiceSelectionItem> Services { get; set; }
    }
}
