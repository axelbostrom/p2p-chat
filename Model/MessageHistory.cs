using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ChatApp.Model
{
    public class MessageHistory
    {
        private string _userName;
        private string _otherUser;
        private List<Message> _messages;

        public MessageHistory(string userName)
        {
            _userName = userName;
            FindHistory();
        }

        public string UserName { get { return _userName; } }

        public List<Message> Messages { get { return _messages; } set { _messages = value; } }

        internal void UpdateConversation(List<Message> messageList)
        {
            _messages = messageList;
        }

        internal void UpdateOtherUser(string otherUser)
        {
            _otherUser = otherUser;
        }

        private void FindHistory()
        {
            string filePath = "";

            if (Directory.Exists(filePath))
            {
                string[] files = Directory.GetFiles(filePath, $"{_userName}*.json");

                _messages = new List<Message>();
                foreach (string file in files)
                {
                    string json = File.ReadAllText(file);
                    List<Message> messagesFromFile = JsonSerializer.Deserialize<List<Message>>(json);

                    _messages.AddRange(messagesFromFile);
                }
            }
        }
    }
}
