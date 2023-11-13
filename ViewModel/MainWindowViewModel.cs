using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using ChatApp.Model;
using ChatApp.View;
using ChatApp.ViewModel.Command;

namespace ChatApp.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private NetworkManager NetworkManager { get; set; }
        private ICommand startGame;
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

        public ICommand StartGame
        {
            get
            {
                if (startGame == null)
                    startGame = new StartGameCommand(this);
                return startGame;
            }
            set
            {
                startGame = value;
            }
        }

        private bool startConnection()
        {
            return NetworkManager.StartConnection();
        }

        public void startGameBoard()
        {

            if (startConnection())
            {
                GameBoard board = new GameBoard();
                board.DataContext = this;
                board.ShowDialog();
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

        public void showGameBoard()
        {
            GameBoard gameBoard = new GameBoard();
            gameBoard.DataContext = this;
            gameBoard.ShowDialog();
        }
    }
}