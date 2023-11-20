using ChatApp.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        public string _name = "Zlatan";
        private string _ip = "127.0.0.1";
        private string _port = "3000";
        private NetworkManager _networkManager;
        private ChatViewModel chat;

        private ICommand _startServerCommand;
        private string text;
        public event PropertyChangedEventHandler PropertyChanged;

        private User _currentUser;

        public string MyText
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged("MyText");
            }

        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MainWindowViewModel(NetworkManager networkManager)
        {
            _networkManager = networkManager;
            networkManager.PropertyChanged += MyModel_PropertyChanged;
            networkManager.EventOccured += NetworkManager_EventOccurred;
        }

        private void MyModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Message")
            {
                var message = _networkManager.Message;



                //chat.AddMessage(sender.ToString(), message);
            }
        }

        private void NetworkManager_EventOccurred(object? sender, string e)
        {
            // TODO: Add messagebox shown depending on error that occured

            if (e == "Connected!")
            {
                StartChatViewModel();
            }
            else if (e == "Error connecting to server!")
            {
                MessageBox.Show("Server not started!");
            }

        }

        public void StartServer(User user)
        {
            _currentUser = user;
            _networkManager.StartServer(_currentUser);
        }

        public void StartClient(User user)
        {
            _currentUser = user;
            _networkManager.StartClient(_currentUser);

        }

        //TODO: REMOVE/MOVE/FIX LOGIC
        public void StartConnectionAction(object param)
        {
            string userType = param as string;

            System.Diagnostics.Debug.WriteLine(userType);


            if (userType != null)
            {
                // TODO: ADD USER need to get input first...
                //currentUser = new User();
            }
        }

        // TODO: When server and client have entered correct info and pressed respective button => start chat for both
        public void StartChatViewModel()
        {
            chat = new ChatViewModel();
            chat.ShowDialog();
        }

        public ICommand StartServerCommand
        {
            get
            {
                return new Command.StartServerCommand(this);
            }
            set { }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                System.Diagnostics.Debug.WriteLine(_name);
            }
        }

        public string Ip
        {
            get { return _ip; }
            set
            {
                _ip = value;
                OnPropertyChanged(nameof(Ip));
                System.Diagnostics.Debug.WriteLine(_ip);
            }
        }

        public string Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));
                System.Diagnostics.Debug.WriteLine(_port);
            }
        }

    }
}