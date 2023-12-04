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
            foreach (Message msg in _messages)
            {
                System.Diagnostics.Debug.WriteLine(msg.Content);
            }
            
        }

        internal void UpdateOtherUser(string otherUser)
        {
            _otherUser = otherUser;
        }

        private void FindHistory()
        {
            
            
        }
    }
}
