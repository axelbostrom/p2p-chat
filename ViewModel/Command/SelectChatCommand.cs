using ChatApp.Model;
using System;
using System.Windows;
using System.Windows.Input;
using static ChatApp.ViewModel.MainWindowViewModel;

namespace ChatApp.ViewModel.Command
{
    internal class SelectChatCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainWindowViewModel _parent;

        public SelectChatCommand(MainWindowViewModel parent)
        {
            _parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object selectedChat)
        {
            if (selectedChat is UserHistoryInfo chat)
            {
                _parent.SelectedChat = chat;
            }

            if (_parent._otherUser == null)
            {
                _parent.LoadMessages();
            }
            else if (_parent.SelectedChat.ChatId == _parent._chatId)
            {
                return;
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to disconnect and view the chat?", "Disconnect and view chat", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _parent.NetworkManager.SendDisconnect();
                    _parent._otherUser = null;
                    _parent.LoadMessages();
                    _parent.LoadChatHistory();
                }
            }
        }
    }
}