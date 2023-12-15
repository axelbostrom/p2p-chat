using ChatApp.Model;
using ChatApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
        private string _searchText;
        private string _chattingWithText;
        private string _message = string.Empty;

        private ObservableCollection<Message> _messages; // For Ui
        private ObservableCollection<UserHistoryInfo> _chats; // For Ui

        private MessageHistory _messageHistory;
        private UserHistoryInfo _selectedChat;

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
        public ICommand SendBuzzCommand { get { return new Command.SendBuzzCommand(this); } }
        public ICommand SearchCommand { get { return new Command.SearchCommand(this); } }

        public MainWindowViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _networkManager = new NetworkManager();
            _networkManager.EventOccured += NetworkManager_EventOccurred;
            _networkManager.MessageReceived += (sender, message) => NetworkManager_MessageReceived(message);
            _messages = new ObservableCollection<Message>();
            _chats = new ObservableCollection<UserHistoryInfo>();
            System.Diagnostics.Debug.WriteLine("Selected Chat =    " + SelectedChat + "     very cool");
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

        public struct UserHistoryInfo
        {
            public string UserName { get; set; }
            public DateTime TimeStamp { get; set; }
        }

        public ObservableCollection<UserHistoryInfo> Chats
        {
            get { return _chats; }
            private set
            {
                _chats = value;
                OnPropertyChanged(nameof(Chats));
            }
        }


        // TODO FIX EVENT HANDLING BETTER
        private void NetworkManager_EventOccurred(object? sender, string e)
        {
            if (e == "SERVER_BOOT_SUCCESS")
            {
                BootChatWindow();
            }
            else if (e == "CLIENT_BOOT_SUCCESS")
            {
                BootWaitWindow(); ;
            }
            else if (e == "ERROR_CONNECT_SERVER")
            {
                MessageBox.Show("No server online on this portnumber.");
            }
            else if (e == "ADDRESS_IN_USE")
            {
                MessageBox.Show("Server using this portnumber already exists.");
            }

        }

        private void BootChatWindow()
        {
            _mainWindow.Hide();
            _chatWindow = new ChatWindow(this);
            _messageHistory = new MessageHistory(Name);
            _messageHistory.UpdateOtherUser(_otherUser);
            LoadChatHistory();
            if (_networkManager.IsClient)
            {
                LoadOtherUserMessages();
            }
            _chatWindow.Show();
        }

        private void BootWaitWindow()
        {
            _mainWindow.Hide();
            _waitWindow = new WaitWindow(this);
            _waitWindow.Show();
        }

        public void LoadChatHistory()
        {
            Chats.Clear();

            Dictionary<string, DateTime> users = _messageHistory.GetChatUserHistory();

            var filteredUsers = users// select all usernames and timestamps
                .Where(pair => string.IsNullOrEmpty(_searchText) || // where either _searchText is null or empty (basically select ALL usernames/timestamps)
                               pair.Key.IndexOf(_searchText, StringComparison.OrdinalIgnoreCase) >= 0) // or the username contains (ignore upper/lowercase) _searchText
                .OrderByDescending(pair => pair.Value); // order by descending, i.e. most recent chat first

            foreach (var user in filteredUsers)
            {
                Chats.Add(new UserHistoryInfo { UserName = user.Key, TimeStamp = user.Value });
            }
        }

        private void LoadMessages()
        {
            Messages.Clear();

            string sChat = _selectedChat.UserName;

            ChattingWithText = "Your chat with " + sChat;
            IsSendButtonEnabled = false;

            foreach (Message msg in _messageHistory.GetChatHistory(sChat))
            {
                Messages.Add(msg);
            }
        }

        public void LoadOtherUserMessages()
        {
            Messages.Clear();
            if (_otherUser == null)
            {
                return;
            }
            List<Message> msgs = _messageHistory.GetChatHistory();
            ChattingWithText = "You are now chatting with " + _otherUser;
            IsSendButtonEnabled = true;
            System.Diagnostics.Debug.WriteLine(_messageHistory.GetChatHistory());
            foreach (Message msg in _messageHistory.GetChatHistory())
            {
                System.Diagnostics.Debug.WriteLine(msg.Content);
                Messages.Add(msg);
            }


        }

        public void onClose(object sender, CancelEventArgs e)
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
            Message msg = new Message(messageType, _user.Name, DateTime.Now, _message);
            Messages.Add(msg);
            if (!NetworkManager.IsClient)  _messageHistory.UpdateConversation(msg);
        }

        private void NetworkManager_MessageReceived(Message message)
        {
            _otherUser = message.Sender;

            switch (message.Type)
            {
                case MessageType.Message:
                    HandleMessageTypeMessage(message);
                    break;

                case MessageType.ConnectionEstablished:
                    HandleMessageTypeConnectionEstablished(message);
                    break;

                case MessageType.AcceptConnection:
                    HandleMessageTypeAcceptConnection(message);
                    break;

                case MessageType.DenyConnection:
                    HandleMessageTypeDenyConnection(message);
                    break;

                case MessageType.Disconnect:
                    HandleMessageTypeDisconnect(message);
                    break;

                case MessageType.Buzz:
                    HandleMessageTypeBuzz();
                    break;
            }
        }

        private void HandleMessageTypeMessage(Message message)
        {
            Messages.Add(message);
            if (!NetworkManager.IsClient) _messageHistory.UpdateConversation(message);
        }

        private void HandleMessageTypeConnectionEstablished(Message message)
        {
            if (!NetworkManager.IsClient)
            {
                UserConnectionText = _otherUser + " wants to connect with you";
                GridVisibility = Visibility.Visible;
            }

            System.Diagnostics.Debug.WriteLine("Connection request from " + _otherUser);
        }

        private void HandleMessageTypeAcceptConnection(Message message)
        {
            _waitWindow?.Hide();
            IsSendButtonEnabled = true;
            BootChatWindow();
        }

        private void HandleMessageTypeDenyConnection(Message message)
        {
            _waitWindow.Hide();
            _mainWindow.Show();
            MessageBox.Show(_otherUser + " denied your chat request.");
        }

        private void HandleMessageTypeDisconnect(Message message)
        {
            if (!NetworkManager.IsClient)
            {
                ChattingWithText = _otherUser + " has disconnected!";
                IsSendButtonEnabled = false;
                _otherUser = null;
                LoadChatHistory();
            }
            else
            {
                Disconnect();
                IsSendButtonEnabled = false;
                ChattingWithText = String.Empty;
                Messages.Clear();
                _otherUser = null;
                _chatWindow.Hide();
                _mainWindow.Show();
                MessageBox.Show("Server has disconnected!");
            }
        }
        private void HandleMessageTypeBuzz()
        {
            string soundFile = "Resources/go.mp3";

            try
            {
                string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, soundFile);

                MediaPlayer player = new MediaPlayer();
                player.Open(new Uri(soundPath));

                player.Position = TimeSpan.FromSeconds(0.8);

                player.Play();

                Task.Delay(TimeSpan.FromSeconds(2.5)).ContinueWith(_ =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        player.Stop();
                    });
                });
            }
            catch (FileNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error playing sound: File not found - {ex.Message}");
            }
            catch (UriFormatException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error playing sound: Invalid URI - {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error playing sound: {ex.GetType().Name} - {ex.Message}");
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

        public MessageHistory MessageHistory { get { return _messageHistory; } }

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
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                }
            }
        }
        public UserHistoryInfo SelectedChat
        {
            get
            {
                return _selectedChat;
            }
            set
            {
                _selectedChat = value;
                OnPropertyChanged(nameof(SelectedChat));

                if (_otherUser == null)
                {
                    LoadMessages();
                }
                else if (_otherUser.Equals(_selectedChat.UserName))
                {
                    System.Diagnostics.Debug.WriteLine("jupp");
                    return;
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure you want to disconnect and view the chat?", "Disconnect and view chat", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        NetworkManager.SendDisconnect();
                        LoadMessages();
                    }
                }

            }
        }
    }
}
