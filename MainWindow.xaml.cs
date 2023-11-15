using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using ChatApp.Model;
using ChatApp.ViewModel;

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

        public MainWindow()
        {
            InitializeComponent();
            networkManager = new NetworkManager();
            this.DataContext = new MainWindowViewModel(networkManager);
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            // Call the StartConnection method in NetworkManager for the server
            if (ParseInputs())
            {
                User user = new User(name, ip, port, "server");
                System.Diagnostics.Debug.WriteLine(user);
                networkManager.StartConnection(user);
            }
        }

        private void StartClient_Click(object sender, RoutedEventArgs e)
        {
            // Call the StartConnection method in NetworkManager for the client
            if (ParseInputs())
            {
                networkManager.StartConnection(new User(name, ip, port, "client"));
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

            return true;
        }
    }
}
