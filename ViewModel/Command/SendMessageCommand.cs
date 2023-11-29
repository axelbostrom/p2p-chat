using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class SendMessageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainWindowViewModel _parent;

        public SendMessageCommand(MainWindowViewModel parent)
        {
            _parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _parent.AddMessage();
            //_parent.NetworkManager.SendUserMessage(_parent.Message);
        }
    }
}
