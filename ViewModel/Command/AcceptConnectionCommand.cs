using ChatApp.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class AcceptConnectionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainWindowViewModel _parent;

        public AcceptConnectionCommand(MainWindowViewModel parent)
        {
            _parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _parent.GridVisibility = Visibility.Hidden;
            _parent.IsSendButtonEnabled = true;
            _parent.ChattingWithText = "You are now chatting with " + _parent._otherUser;
            _parent._chatId = _parent.MessageHistory.UpdateOtherUser(_parent._otherUser);
            _parent.Messages.Clear();
            _parent.NetworkManager.SendConnectionAccepted();
        }
    }
}
