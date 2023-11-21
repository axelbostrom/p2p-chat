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
            if (v == Visibility.Visible)
            {
                v = Visibility.Hidden;
            }
            else
            {
                v = Visibility.Visible;
            }
            _parent.GridVisibility = v;
            _parent.AddMessage();
            _parent.NetworkManager.SendUserMessage(_parent.Message);
        }
    }
}
