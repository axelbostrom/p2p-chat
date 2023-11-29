using System;
using System.ComponentModel;

namespace ChatApp.Model
{


    public class Message : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _sender;
        private string _content;
        private DateTime _timestamp;
        private MessageType _type;

        public Message(MessageType type, string sender, DateTime timestamp, string content)
        {
            _type = type;
            _sender = sender;
            _timestamp = timestamp;
            _content = content;
        }

        public Message(MessageType type, string sender)
        {
            _type = type;
            _sender = sender;
        }

        public Message()
        {

        }

        public void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public MessageType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        public string Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                _sender = value;
                OnPropertyChanged("Sender");
            }
        }

        public DateTime Timestamp
        {
            get
            {
                return _timestamp;
            }
            set
            {
                _timestamp = value;
                OnPropertyChanged("Timestamp");
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                OnPropertyChanged("Content");
            }
        }
    }
}