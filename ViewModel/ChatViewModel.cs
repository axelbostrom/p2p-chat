using ChatApp.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ChatApp.ViewModel
{


    internal class ChatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _message;
        private NetworkManager _networkManager;
        private User _user;

        public ObservableCollection<Message> _messages;

        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        public ChatViewModel(NetworkManager networkManager, User user)
        {
            _networkManager = networkManager;
            _user = user;
            Messages = new ObservableCollection<Message>();
        }

        public NetworkManager NetworkManager { get { return _networkManager; } }

        public ICommand SendMessageCommand
        {
            get
            {
                return new Command.SendMessageCommand(this);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal void AddMessage()
        {
            System.Diagnostics.Debug.WriteLine("Add Message " + _message + " for user " + _user.Name);
            Message messageToSend = new Message(_user.Name, DateTime.Now, _message);
            Messages.Add(messageToSend);

        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
                System.Diagnostics.Debug.WriteLine(_message);
            }
        }
    }
}
