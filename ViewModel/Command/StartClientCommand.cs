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
        private MainWindowViewModel _parent;

        public StartClientCommand(MainWindowViewModel parent)
        {
            _parent = parent;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!int.TryParse(_parent.Port, out int parsedPort))
            {
                MessageBox.Show("Invalid port number!");
                return;
            }

            if (!IPAddress.TryParse(_parent.Ip, out IPAddress parsedIp))
            {
                MessageBox.Show("Invalid IP address!");
                return;
            }

            int port = parsedPort;
            IPAddress ip = parsedIp;
            string name = _parent.Name;

            if (String.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a name!");
                return;
            }

            var user = new User(name, ip, port);
            _parent.User = user;
            _parent.NetworkManager.StartClient(user);
        }
    }
}