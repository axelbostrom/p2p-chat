﻿using System.ComponentModel;
using System.Net;

namespace ChatApp.Model
{
    public class User : INotifyPropertyChanged
    {

        public enum UserType
        {
            Client,
            Server
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private string _name;
        private int _port;
        private IPAddress _address;
        private UserType _type; // User type, server or client.

        public User(string name, IPAddress address, int port)
        {
            _name = name;
            _address = address;
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

        public UserType Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
}
