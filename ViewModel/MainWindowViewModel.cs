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
        private ICommand _startClientCommand;
        private string text;
        public event PropertyChangedEventHandler PropertyChanged;

        public NetworkManager NetworkManager { get { return _networkManager; } }

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



                chat.AddMessage(sender.ToString(), message);
            }
        }

        private void NetworkManager_EventOccurred(object? sender, string e)
        {
            // TODO: Add messagebox shown depending on error that occured

            if (e == "Server started succesfully!")
            {
                StartChatViewModel();
            }
            else if (e == "Connected!")
            {
                //StartChatViewModel();
            }
            else if (e == "Error connecting to server!")
            {
                MessageBox.Show("Server not started!");
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

        public ICommand StartClientCommand
        {
            get
            {
                return new Command.StartClientCommand(this);
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