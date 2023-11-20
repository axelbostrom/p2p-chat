using ChatApp.Model;
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace ChatApp.ViewModel.Command
{
    internal class StartClientCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainWindowViewModel parent;

        public StartClientCommand(MainWindowViewModel parent)
        {
            this.parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!int.TryParse(parent.Port, out int parsedPort))
            {
                MessageBox.Show("Invalid port number!");
                return;
            }

            if (!IPAddress.TryParse(parent.Ip, out IPAddress parsedIp))
            {
                MessageBox.Show("Invalid IP address!");
                return;
            }

            int port = parsedPort;
            IPAddress ip = parsedIp;
            string name = parent.Name;

            if (String.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a name!");
                return;
            }

            var user = new User(name, ip, port, "client");
            parent.NetworkManager.StartClient(user);
        }
    }
}