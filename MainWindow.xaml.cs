using ChatApp.Model;
using ChatApp.ViewModel;
using System;
using System.Net;
using System.Windows;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NetworkManager networkManager;
        private IPAddress ip;
        private int port;
        private string name;
        User user;

        public MainWindow()
        {
            InitializeComponent();
            networkManager = new NetworkManager();
            this.DataContext = new MainWindowViewModel(networkManager);
        }

    }
}
