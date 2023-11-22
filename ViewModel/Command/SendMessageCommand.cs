using System;
using System.Windows;
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
            Visibility v = _parent.GridVisibility;
            _parent.GridVisibility = v;
            _parent.AddMessage();
            _parent.NetworkManager.SendUserMessage(_parent.Message);
        }
    }
}
