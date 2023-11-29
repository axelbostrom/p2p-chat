using ChatApp.Model;
using ChatApp.View;
using System;
using System.Collections.ObjectModel;
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
        private string _otherUser;
        private string _message;

        private NetworkManager _networkManager;
        public NetworkManager NetworkManager { get { return _networkManager; } }

        private ChatWindow _chatWindow;
        private MainWindow _mainWindow;

        private ICommand _startServerCommand;
        private ICommand _startClientCommand;

        private bool _isSendButtonEnabled = false;

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
            _messages = new ObservableCollection<Message>();
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

        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
            private set
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
            else if (e == "New Client Connection")
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
            MessageType messageType = MessageType.Message;
            Message messageToSend = new Message(messageType, _user.Name, DateTime.Now, _message);
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

        public bool IsSendButtonEnabled
        {
            get { return _isSendButtonEnabled; }
            set
            {
                _isSendButtonEnabled = value;
                OnPropertyChanged(nameof(IsSendButtonEnabled)); // Implement INotifyPropertyChanged
            }
        }


        private void NetworkManager_MessageReceived(Message message)
        {
            if (message.Type == MessageType.Message)
            {
                Messages.Add(message);
            }
            else if (message.Type == MessageType.ConnectionEstablished)
            {
                // CONNECTION REQUEST DO WHAT SHOULD BE DONE, MORE FOR GETTING USERNAME ETC
                // MAYBE CALL HELLO WORLD?
                // Update name in chatview etc
                _otherUser = message.Sender;
                System.Diagnostics.Debug.WriteLine("Connection request from " + _otherUser);

            }

        }
    }
}
