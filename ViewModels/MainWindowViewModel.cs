namespace ChatApp.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private NetworkManager _networkManager;
        private ICommand _startServerCommand;
        private string text;
        public event PropertyChangedEventHandler PropertyChanged;

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
                this.MyText = message;
            }
        }

        private void NetworkManager_EventOccurred(object? sender, string e)
        {
            // TODO: Add messagebox shown depending on error that occured

            if (e == "Connected!")
            {
                startChatViewModel();
            }
            else if (e == "Error connecting to server!")
            {
                MessageBox.Show("Server not started!");
            }

        }

        public void StartServer(User user)
        {

            _networkManager.StartServer(user);
        }

        public void StartClient(User user)
        {

            _networkManager.StartClient(user);

        }

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
        public void startChatViewModel()
        {

            ChatViewModel chat = new ChatViewModel();
            chat.DataContext = this;
            chat.ShowDialog();
        }

        public void sendMessage()
        {
            _networkManager.SendChar(MyText);
        }

        public ICommand StartServerCommand
        {
            get
            {
                return new Command.StartServerCommand(this);
            }
            set { }
        }

        public ICommand SendCommand
        {
            get { return _sendCommand; }
            set { _sendCommand = value; }
        }


    }
}