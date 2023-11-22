using ChatApp.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{


    internal class ChatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _message;
        private NetworkManager _networkManager;
        private User _user;

        private Visibility _gridVisibility = Visibility.Hidden;
        public Visibility GridVisibility
        {
            get => _gridVisibility;
            set
            {
                if (_gridVisibility != value)
                {
                    _gridVisibility = value;
                    OnPropertyChanged(nameof(GridVisibility));
                }
            }
        }

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
            networkManager.MessageReceived += (sender, message) => NetworkManager_MessageReceived(message);
            _networkManager.EventOccured += NetworkManager_EventOccurred;
            _user = user;
            Messages = new ObservableCollection<Message>();
        }

        private void NetworkManager_EventOccurred(object? sender, string e)
        {
            // TODO: Add messagebox shown depending on error that occured

            if (e == "xd")
            {
                Visibility v = GridVisibility;
                GridVisibility = Visibility.Visible;
            }
            

        }

        public NetworkManager NetworkManager { get { return _networkManager; } }

        private void NetworkManager_MessageReceived(string message)
        {
            // Parse the message to extract sender's name, timestamp, etc.
            string senderName = "Zlatan"; //ADD NAME
            DateTime timestamp = DateTime.Now; // ADD DATE

            // Create a new Message object
            Message newMessage = new Message(senderName, timestamp, message);

            // Add the new message to your Messages collection
            Messages.Add(newMessage);
        }

        public ICommand SendMessageCommand { get { return new Command.SendMessageCommand(this); } }
        public ICommand AcceptConnectionCommand { get { return new Command.AcceptConnectionCommand(this); } }
        public ICommand DenyConnectionCommand { get { return new Command.DenyConnectionCommand(this); } }

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

        public void WindowClosing()
        {
            // Perform actions needed when the window is closing
            Application.Current.Shutdown();
        }

    }
}
