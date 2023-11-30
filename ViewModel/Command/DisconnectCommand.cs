using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class DisconnectCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainWindowViewModel _parent;

        public DisconnectCommand(MainWindowViewModel parent)
        {
            _parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _parent.NetworkManager.Disconnect();
        }
    }
}
