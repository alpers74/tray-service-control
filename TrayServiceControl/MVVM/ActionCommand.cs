using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace TrayServiceControl.MVVM
{
    public class ActionCommand : ICommand
    {
        private readonly Action _theCommand;
        private bool _enabled;

        public ActionCommand(Action theCommand)
        {
            _theCommand = theCommand;
            _enabled = true;
        }

        public ActionCommand(Action theCommand, bool enabled)
        {
            _theCommand = theCommand;
            _enabled = enabled;
        }

        public bool CanExecute(object parameter)
        {
            return _enabled;
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                var evt = CanExecuteChanged;
                if (evt != null)
                    evt(this, new EventArgs());
            }
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _theCommand();
        }
    }
}
