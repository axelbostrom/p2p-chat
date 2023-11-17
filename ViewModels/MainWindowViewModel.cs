﻿using ChatApp.Model;
using ChatApp.ViewModel.Command;
using ChatApp.ViewModels;
using ChatApp.ViewModels.Command;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private NetworkManager NetworkManager { get; set; }
        private ICommand startChat;
        private ICommand startConnection;
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

        public ICommand StartConnection
        {
            get
            {
                if (startConnection == null)
                {
                    startConnection = new RelayCommand(param => StartConnectionAction(param));
                }
                return startConnection;
            }
            set
            {
                startConnection = value;
            }
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
            //if (startConnection())
            //{
            //    ChatViewModel chat = new ChatViewModel();
            //    chat.DataContext = this;
            //    chat.ShowDialog();
            //}
            //else
            //{
            //    MessageBox.Show("Cannot start connection!");
            //}
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