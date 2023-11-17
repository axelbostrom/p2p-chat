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
            networkManager.EventOccured += NetworkManager_EventOccurred;
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            // Call the StartServer method in NetworkManager
            if (ParseInputs())
            {
                user = new User(name, ip, port, "server");
                networkManager.StartServer(user);
            }
        }

        private void StartClient_Click(object sender, RoutedEventArgs e)
        {
            // call Command/StartClientCommand.cs
            
            // Call the StartClient method in NetworkManager
            if (ParseInputs())
            {
                user = new User(name, ip, port, "client");
                networkManager.StartClient(user);
            }
        }

        private bool ParseInputs()
        {
            if (!int.TryParse(TextBox_Port.Text, out int parsedPort))
            {
                MessageBox.Show("Invalid port number!");
                return false;
            }

            if (!IPAddress.TryParse(TextBox_Ip.Text, out IPAddress parsedIp))
            {
                MessageBox.Show("Invalid IP address!");
                return false;
            }

            port = parsedPort;
            ip = parsedIp;
            name = TextBox_Name.Text;

            if (String.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a name!");
                return false;
            }

            return true;
        }
        private void NetworkManager_EventOccurred(object? sender, string e)
        {
            // TODO: Add messagebox shown depending on error that occured
            MessageBox.Show("Server not started!");
        }
    }
}
