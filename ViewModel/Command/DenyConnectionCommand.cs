using System;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class DenyConnectionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private ChatViewModel _parent;

        public DenyConnectionCommand(ChatViewModel parent)
        {
            _parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _parent.NetworkManager.Server.DenyClientConnection();
            _parent.GridVisibility = Visibility.Hidden;
        }
    }
}
