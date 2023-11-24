using ChatApp.Model;
using ChatApp.View;
using System.Collections.ObjectModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public string _name = "Zlatan";
        private string _ip = "127.0.0.1";
        private string _port = "3000";
        private User _user;
        private string _message;

        private NetworkManager _networkManager;
        public NetworkManager NetworkManager { get { return _networkManager; } }

        private ChatWindow _chatWindow;
        private MainWindow _mainWindow;
        
        private ICommand _startServerCommand;
        private ICommand _startClientCommand;
        
        public event PropertyChangedEventHandler PropertyChanged;

        private Visibility _gridVisibility = Visibility.Hidden;


        public ICommand StartServerCommand { get { return new Command.StartServerCommand(this); } }
        public ICommand StartClientCommand { get { return new Command.StartClientCommand(this); } }
        public ICommand SendMessageCommand { get { return new Command.SendMessageCommand(this); } }
        public ICommand AcceptConnectionCommand { get { return new Command.AcceptConnectionCommand(this); } }
        public ICommand DenyConnectionCommand { get { return new Command.DenyConnectionCommand(this); } }

        public MainWindowViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _networkManager = new NetworkManager();
            _networkManager.EventOccured += NetworkManager_EventOccurred;
            _networkManager.MessageReceived += (sender, message) => NetworkManager_MessageReceived(message);
            Messages = new ObservableCollection<Message>();
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Ip
        {
            get { return _ip; }
            set
            {
                _ip = value;
                OnPropertyChanged(nameof(Ip));
            }
        }

        public string Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        public User User
        {
            get { return _user; }
            set { _user = value; }
        }


        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private ObservableCollection<Message> _messages;

        private ObservableCollection<Message> Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                OnPropertyChanged(nameof(Messages));
            }
        }

        private void NetworkManager_EventOccurred(object? sender, string e)
        {
            // TODO: Add messagebox shown depending on error that occured

            if (e == "Booted up succesfully!")
            {
                _mainWindow.Hide();
                StartChatViewModel();
            }
            else if (e == "Connected!")
            {
               // StartChatViewModel();
            }
            else if (e == "Error connecting to server!")
            {
                MessageBox.Show("Server not started!");
            }
            else if (e == "xd")
            {
                Visibility v = GridVisibility;
                GridVisibility = Visibility.Visible;
            }

        }

        // TODO: When server and client have entered correct info and pressed respective button => start chat for both AND Change name
        public void StartChatViewModel()
        {
            _chatWindow = new ChatWindow(this);
            _chatWindow.Show();
        }

        public void AddMessage()
        {
            System.Diagnostics.Debug.WriteLine("Add Message " + _message + " for user " + _user.Name);
            Message messageToSend = new Message(_user.Name, DateTime.Now, _message);
            Messages.Add(messageToSend);
        }

        
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
    }
}
