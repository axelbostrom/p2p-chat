using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System;
using System.Runtime.Intrinsics.X86;

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
            var newConversation = new
            {
                user1 = _userName,
                user2 = _otherUser,
                messages = _messages
            };

            string json = JsonSerializer.Serialize(newConversation);

            string[] lines = File.ReadAllLines("history.json");

            int exists = ConversationExists();

            if (exists == -1)
            {
                lines = lines.Append(json).ToArray();
            }
            else
            {
                lines[exists] = json;
            }

            File.WriteAllLines("history.json", lines);

        }

        internal void UpdateOtherUser(string otherUser)
        {
            _otherUser = otherUser;
        }

        private void FindHistory()
        {
            string[] lines = File.ReadAllLines("history.json");
            List<string> conversations = new List<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(_userName)) conversations.Add(lines[i]);
            }
        }

        private int ConversationExists()
        {

            string[] lines = File.ReadAllLines("history.json");

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("{\"user1\":\"" + _otherUser + "\",\"user2\":\"" + _userName) || 
                    lines[i].StartsWith("{\"user1\":\"" + _userName + "\",\"user2\":\"" + _otherUser))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
