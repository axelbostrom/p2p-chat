using ChatApp.Model;
using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class SendMessageCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private ChatViewModel _parent;

        public SendMessageCommand(ChatViewModel parent)
        {
            _parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _parent.NetworkManager.SendChar(_parent.Message);
        }
    }
}
