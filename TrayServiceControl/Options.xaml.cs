using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TrayServiceControl
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private OptionsViewModel _optionsViewModel;

        public Options(OptionsViewModel optionsViewModel)
        {
            _optionsViewModel = optionsViewModel;
            
            InitializeComponent();

            DataContext = optionsViewModel;
        }

        // TODO: Why dont the bindings work
        private void CheckedHack(object sender, RoutedEventArgs e)
        {
            var svm = (OptionsViewModel.ServiceSelectionItem)((System.Windows.Controls.CheckBox)sender).DataContext;
            var isChecked = ((System.Windows.Controls.Primitives.ToggleButton)(e.Source)).IsChecked;
            svm.ShowInMenu = isChecked.GetValueOrDefault();
        }

        private void OnClosed(object sender, EventArgs e)
        {
            IsClosed = true;
        }

        public bool IsClosed { get; set; }
    }
}
