using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ChatApp.Model;
using ChatApp.ViewModel.Command;
using ChatApp.ViewModels;

namespace ChatApp.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private NetworkManager NetworkManager { get; set; }
        private ICommand startChat;
        private string text;
        public event PropertyChangedEventHandler PropertyChanged;

        private string name;
        private int port;
        private IPAddress address;

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
            NetworkManager = networkManager;
            networkManager.PropertyChanged += myModel_PropertyChanged;
        }

        private void myModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Message")
            {
                var message = NetworkManager.Message;
                this.MyText = message;
            }
        }

        public ICommand StartChat
        {
            get
            {
                if (startChat == null)
                    startChat = new StartChatCommand(this);
                return startChat;
            }
            set
            {
                startChat = value;
            }
        }

        private bool startConnection()
        {
            return NetworkManager.StartConnection();
        }

        public void startChatViewModel()
        {

            if (startConnection())
            {
                ChatViewModel chat = new ChatViewModel();
                chat.DataContext = this;
                chat.ShowDialog();
            }
            else
            {
                MessageBox.Show("Cannot start connection!");
            }
        }

        private ICommand enterCommand;
        public ICommand EnterCommand
        {
            get
            {
                if (enterCommand == null)
                {
                    return new Command.KeyEnterCommand(this);
                }
                else
                {
                    return enterCommand;

                }
            }
            set { enterCommand = value; }
        }

        public void sendMessage()
        {
            NetworkManager.SendChar(MyText);
        }

        public void showChatViewModel()
        {
            ChatViewModel chatViewModel = new ChatViewModel();
            chatViewModel.DataContext = this;
            chatViewModel.ShowDialog();
        }
    }
}