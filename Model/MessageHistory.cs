using System.Collections.ObjectModel;

namespace ChatApp.Model
{
    public class MessageHistory
    {
        private string _userName;
        private ObservableCollection<Message> _messages;

        public MessageHistory(string userName, ObservableCollection<Message> messages)
        {
            _userName = userName;
            _messages = messages;
        }

        public string UserName { get { return _userName; } }

        public ObservableCollection<Message> Messages { get { return _messages; } }

        public void AddMessage(Message message)
        {
            _messages.Add(message);
        }
    }
}
