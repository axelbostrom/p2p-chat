using ChatApp.Model;
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

        public ChatViewModel(NetworkManager networkManager, User user)
        {
            _networkManager = networkManager;
            _user = user;
        }

        public NetworkManager NetworkManager { get { return _networkManager; }}

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

        internal void AddMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine("Add Message " + message);
            // Messages.Add(new Message { Sender = sender, Timestamp = DateTime.Now, Content = message });
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
