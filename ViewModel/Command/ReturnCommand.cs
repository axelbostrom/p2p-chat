using ChatApp.Model;
using System;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class ReturnCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainWindowViewModel _parent;

        public ReturnCommand(MainWindowViewModel parent)
        {
            _parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _parent.ChattingWithText = "You are now chatting with " + _parent._otherUser;
            _parent.LoadOtherUserMessages();
        }
    }
}