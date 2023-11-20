using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ChatApp.ViewModel
{


    internal partial class ChatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _message;

        public ICommand SendMessageCommand
        {
            get
            {
                return new Command.SendMessageCommand(this);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        internal void SendMessage()
        {
            string message = Message;
            System.Diagnostics.Debug.WriteLine(message);

            // Add message to chatbox here
            // AddMessage(message);

            // Clear messagebox
            Message = string.Empty;
        }

        internal void AddMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine("Add Message " + message);
            // Messages.Add(new Message { Sender = sender, Timestamp = DateTime.Now, Content = message });
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
                System.Diagnostics.Debug.WriteLine(_message);
            }
        }
    }
}
