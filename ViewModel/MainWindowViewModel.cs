using ChatApp.Model;
using ChatApp.View;
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
        private ChatWindow chat;

        private ICommand _startServerCommand;
        private ICommand _startClientCommand;
        private string text;
        public event PropertyChangedEventHandler PropertyChanged;

        public NetworkManager NetworkManager { get { return _networkManager; } }
        private User _currentUser;

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

            if (e == "Server started succesfully!")
            {
                StartChatViewModel();
            }
            else if (e == "Connected!")
            {
                StartChatViewModel();
            }
            else if (e == "Error connecting to server!")
            {
                MessageBox.Show("Server not started!");
            }

        }

        public ICommand StartServerCommand { get { return new Command.StartServerCommand(this); } }
        public ICommand StartClientCommand { get { return new Command.StartClientCommand(this); } }

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
            chat = new ChatWindow();
            chat.ShowDialog();
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

    }
}