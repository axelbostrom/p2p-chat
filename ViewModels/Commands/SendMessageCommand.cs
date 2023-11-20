using System;
using System.Windows.Input;

namespace ChatApp.ViewModels.Commands
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

        }
    }
}
