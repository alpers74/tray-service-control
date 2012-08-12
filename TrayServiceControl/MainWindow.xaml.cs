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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using TrayServiceControl.Properties;

namespace TrayServiceControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon;
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            var settings = new Settings();

            _viewModel = new MainViewModel(settings);
            _viewModel.QuitEvent += new Action(() =>
            {
                _notifyIcon.Visible = false;
                System.Windows.Application.Current.Shutdown();

            });

            LeftClickMenu.DataContext = _viewModel;
            RightClickMenu.DataContext = _viewModel;
            DataContext = _viewModel;
        }

        private void ToggleVisibility()
        {
            Dispatcher.BeginInvoke(
                new Action(
                    () => Visibility = Visibility == Visibility.Visible ? 
                        System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        private void OnInitialized(object sender, EventArgs e)
        {
            _notifyIcon = new NotifyIcon();

            _notifyIcon.Icon = Properties.Resources.Tray;
            _notifyIcon.Visible = true;
            _notifyIcon.ShowBalloonTip(500, 
                "Services Control",
                "Open Options from right click menu to select services to include in the main menu.\n",
                ToolTipIcon.None);
            _notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(TrayMouseClick);
        }

        void TrayMouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                RightClickMenu.IsOpen = true;
            }
            else if (e.Button == MouseButtons.Left)
            {
                _viewModel.RefreshMenu();

                if (_viewModel.FilteredServices.Count == 0)
                {
                    _notifyIcon.ShowBalloonTip(500,
                        "Services Control",
                        (_viewModel.ServicesPopulated) ? 
                        "Open Options from right click menu to select services to include in the main menu.\n" :
                        "Getting services info. Please wait...",
                        ToolTipIcon.None);
                }
                else
                {
                    LeftClickMenu.IsOpen = true;
                }
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(
                new Action(
                    () => Visibility = Visibility == Visibility.Visible ?
                        System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible));
        }

        private void AttachClickHack(object sender, RoutedEventArgs e)
        {
            var button = ((System.Windows.Controls.Button)sender);
            button.ContextMenu.DataContext = button.DataContext;
            ContextMenuService.SetContextMenu(button, button.ContextMenu);
            button.ContextMenu.PlacementTarget = button;
            button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            button.SetValue(ContextMenuService.ContextMenuProperty, button.ContextMenu);
            button.SetValue(ContextMenuService.PlacementProperty, System.Windows.Controls.Primitives.PlacementMode.Bottom);
            button.ContextMenu.IsOpen = true;
         }
    }
}
