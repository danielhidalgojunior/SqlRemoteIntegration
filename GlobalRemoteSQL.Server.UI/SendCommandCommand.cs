using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GlobalRemoteSQL.Server.UI
{
    public class SendCommandCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        private MainViewModel _vm;

        public SendCommandCommand(MainViewModel vm)
        {
            _vm = vm;
        }

        public bool CanExecute(object parameter)
        {
            return 
                _vm.IsAbleToSendCommand && 
                _vm.ConnectedClients.Count > 0 && 
                !string.IsNullOrEmpty(_vm.SelectedClient?.Name) && 
                !string.IsNullOrEmpty(_vm.Command);
        }

        public void Execute(object parameter)
        {
            _vm.DgResults = parameter as DataGrid;
            _vm.SendCommand();
        }
    }
}
