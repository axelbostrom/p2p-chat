using ChatApp.Model;
using ChatApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
        public ICommand SendBuzzCommand { get { return new Command.SendBuzzCommand(this); } }

        public MainWindowViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _networkManager = new NetworkManager();
            _networkManager.EventOccured += NetworkManager_EventOccurred;
            _networkManager.MessageReceived += (sender, message) => NetworkManager_MessageReceived(message);
            _messages = new ObservableCollection<Message>();
            _messageList = new List<Message>();
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


        // TODO FIX EVENT HANDLING BETTER
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
                MessageBox.Show("Error connecting to server!");
            }
            else if (e == "Error creating server!")
            {
                MessageBox.Show("Server already exists!");
            }

        }

        private void BootChatWindow()
        {
            _mainWindow.Hide();
            _chatWindow = new ChatWindow(this);
            _messageHistory = new MessageHistory(Name);
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

            if (NetworkManager.Server != null)
            {
                _messageList.Add(message);
                _messageHistory.UpdateConversation(_messageList);
            }
        }

        private void HandleMessageTypeConnectionEstablished(Message message)
        {
            if (NetworkManager.Server != null)
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
            ChattingWithText = "You are now chatting with " + _otherUser;
            BootChatWindow();
        }

        private void HandleMessageTypeDenyConnection(Message message)
        {
            _waitWindow.Hide();
            _mainWindow.Show();
            string textDeny = _otherUser + " denied your chat request.";
            MessageBox.Show(textDeny);
        }

        private void HandleMessageTypeDisconnect(Message message)
        {
            if (NetworkManager.Server != null)
            {
                ChattingWithText = _otherUser + " has disconnected!";
                IsSendButtonEnabled = false;
                // TODO: CLEAR ACTIVE MESSAGES IN CHAT WINDOW
                // NO, CLEAR BEFORE NEW CHAT OR WHEN CHANGING TO CHAT FROM HISTORY
            }
            else
            {
                // TODO: NO POP-UP, TEXT?
                MessageBox.Show("Server has disconnected!");
                Disconnect();
                IsSendButtonEnabled = false;
                _chatWindow.Hide();
                _mainWindow.Show();
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
    }
}
