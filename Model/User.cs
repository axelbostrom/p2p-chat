using System.ComponentModel;
using System.Net;

namespace ChatApp.Model
{
    internal class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _name;
        private int _port;
        private IPAddress _address;
        private string _type; // User type, server or client.

        public User(string name, IPAddress address, int port, string type)
        {
            _name = name;
            _address = address;
            _type = type;
            _port = port;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public IPAddress Address
        {
            get { return _address; }
            set { _address = value; }
        }

        // TODO: Remove type from User
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}
