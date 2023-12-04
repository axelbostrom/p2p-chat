using ChatApp.Model;
using ChatApp.View;
using System;
using System.Collections.Generic;
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
        public string _otherUser;
        private string _userConnectionText;
        private string _message = string.Empty;

        private ObservableCollection<Message> _messages; // For Ui
        private List<Message> _messageList; // For history
        private MessageHistory _messageHistory;

        private string _chattingWithText;

        private NetworkManager _networkManager;
        public NetworkManager NetworkManager { get { return _networkManager; } }

        private ChatWindow _chatWindow;
        private MainWindow _mainWindow;
        private WaitWindow _waitWindow;

        private bool _isSendButtonEnabled = false;

        public event PropertyChangedEventHandler PropertyChanged;

        private Visibility _gridVisibility = Visibility.Hidden;


        public ICommand StartServerCommand { get { return new Command.StartServerCommand(this); } }
        public ICommand StartClientCommand { get { return new Command.StartClientCommand(this); } }
        public ICommand SendMessageCommand { get { return new Command.SendMessageCommand(this); } }
        public ICommand AcceptConnectionCommand { get { return new Command.AcceptConnectionCommand(this); } }
        public ICommand DenyConnectionCommand { get { return new Command.DenyConnectionCommand(this); } }
        public ICommand DisconnectCommand { get { return new Command.DisconnectCommand(this); } }

        public MainWindowViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _networkManager = new NetworkManager();
            _networkManager.EventOccured += NetworkManager_EventOccurred;
            _networkManager.MessageReceived += (sender, message) => NetworkManager_MessageReceived(message);
            _messages = new ObservableCollection<Message>();
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

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
            if (e == "Server booted up successfully!")
            {
                BootChatWindow();
            }
            else if (e == "Client booted up successfully!")
            {
                BootWaitWindow(); ;
            }
            else if (e == "Error connecting to server!")
            {
                MessageBox.Show("Server not started!");
            }

        }

        private void BootChatWindow()
        {
            _mainWindow.Hide();
            _chatWindow = new ChatWindow(this);
            _messageHistory = new MessageHistory(Name);
            _messageHistory.UpdateOtherUser(_otherUser);
            _chatWindow.Show();
        }

        private void BootWaitWindow()
        {
            _mainWindow.Hide();
            _waitWindow = new WaitWindow(this);
            _waitWindow.Show();
        }

        public void onClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Disconnect();
            Environment.Exit(0);
        }

        public void Disconnect()
        {
            NetworkManager.SendDisconnect();
            NetworkManager.Server?.StopServer();
            NetworkManager.Client?.Disconnect();
        }

        public void AddMessage()
        {
            System.Diagnostics.Debug.WriteLine("Add Message " + _message + " for user " + _user.Name);
            MessageType messageType = MessageType.Message;
            Message messageToSend = new Message(messageType, _user.Name, DateTime.Now, _message);
            Messages.Add(messageToSend);
            _messageList.Add(messageToSend);
            _messageHistory.UpdateConversation(_messageList);
        }

        private void NetworkManager_MessageReceived(Message message)
        {
            _otherUser = message.Sender;

            if (message.Type == MessageType.Message)
            {
                Messages.Add(message);
            }
            else if (message.Type == MessageType.ConnectionEstablished)
            {

                if (NetworkManager.Server != null) // fake news, it can be null. This is just for server.
                {
                    UserConnectionText = _otherUser + " wants to connect with you";
                    GridVisibility = Visibility.Visible;
                }

                System.Diagnostics.Debug.WriteLine("Connection request from " + _otherUser);
            }
            else if (message.Type == MessageType.AcceptConnection)
            {
                _waitWindow?.Hide();
                IsSendButtonEnabled = true;
                ChattingWithText = "You are now chatting with " + _otherUser;
                BootChatWindow();
            }
            else if (message.Type == MessageType.DenyConnection)
            {
                _waitWindow.Hide();
                _mainWindow.Show();
                string textDeny = _otherUser + " denied your chat request.";
                MessageBox.Show(textDeny);
            }

            else if (message.Type == MessageType.Disconnect)
            {
                if (NetworkManager.Server != null)
                {
                    ChattingWithText = _otherUser + " has disconnected!";
                    IsSendButtonEnabled = false;
                    // TODO: CLEAR ACTIVE MESSAGES IN CHAT WINDOW
                }
                else
                {
                    MessageBox.Show("Server has disconnected!");
                    Disconnect();
                    IsSendButtonEnabled = false;
                    _chatWindow.Hide();
                    _mainWindow.Show();
                }
            }

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

        public string ChattingWithText
        {
            get { return _chattingWithText; }
            set
            {
                _chattingWithText = value;
                OnPropertyChanged(nameof(ChattingWithText));
            }
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
                OnPropertyChanged(nameof(IsSendButtonEnabled));
            }
        }

        public string UserConnectionText
        {
            get { return _userConnectionText; }
            set
            {
                _userConnectionText = value;
                OnPropertyChanged(nameof(UserConnectionText));
            }
        }
    }
}
