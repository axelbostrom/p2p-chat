using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        }

        public class Conversation
        {
            public string User1 { get; set; }
            public string User2 { get; set; }
            public List<Message> Messages { get; set; }
        }

        public string UserName
        {
            get { return _userName; }
        }

        public List<Message> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        internal void UpdateConversation(List<Message> messageList)
        {
            if (_otherUser == null)
            {
                return;
            }
            _messages = messageList;
            var newConversation = new Conversation
            {
                User1 = _userName,
                User2 = _otherUser,
                Messages = _messages
            };

            List<Conversation> existingConversations = new List<Conversation>();


            string existingJson = File.ReadAllText("history.json");

            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                existingConversations = JsonSerializer.Deserialize<List<Conversation>>(existingJson);
            }

            existingConversations.Add(newConversation);

            string updatedJson = JsonSerializer.Serialize(existingConversations,
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("history.json", updatedJson);
            Console.WriteLine("Conversation appended to JSON file.");

        }

        internal void UpdateOtherUser(string otherUser)
        {
            _otherUser = otherUser;
        }

        public List<string> GetChatUserHistory()
        {
            string[] lines = File.ReadAllLines("history.json");
            List<Conversation> existingConversations = new List<Conversation>();
            List<string> conversationUsers = new List<string>();

            string existingJson = File.ReadAllText("history.json");

            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                existingConversations = JsonSerializer.Deserialize<List<Conversation>>(existingJson);
            }

            foreach (Conversation conv in existingConversations)
            {
                if (conv.User1.Equals(_userName))
                {
                    conversationUsers.Add(conv.User2);
                }

                if (conv.User2.Equals(_userName))
                {
                    if(!conversationUsers.Contains(conv.User1)) conversationUsers.Add(conv.User1);
                }
            }

            return conversationUsers;
        }

        public List<Message> GetChatHistory(string user)
        {
            string[] lines = File.ReadAllLines("history.json");
            List<Conversation> existingConversations = new List<Conversation>();


            string existingJson = File.ReadAllText("history.json");

            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                existingConversations = JsonSerializer.Deserialize<List<Conversation>>(existingJson);
            }

            foreach (Conversation conv in existingConversations)
            {
                if ((conv.User1.Equals(_userName) && conv.User2.Equals(user)) ||
                    (conv.User2.Equals(_userName) && conv.User1.Equals(user)))
                {
                    return conv.Messages;
                }
            }

            return null;
        }

        public List<Message> GetChatHistory()
        {
            string[] lines = File.ReadAllLines("history.json");
            List<Conversation> existingConversations = new List<Conversation>();


            string existingJson = File.ReadAllText("history.json");

            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                existingConversations = JsonSerializer.Deserialize<List<Conversation>>(existingJson);
            }

            foreach (Conversation conv in existingConversations)
            {
                if ((conv.User1.Equals(_userName) && conv.User2.Equals(_otherUser)) ||
                    (conv.User2.Equals(_userName) && conv.User1.Equals(_otherUser)))
                {
                    return conv.Messages;
                }
            }
            return null;
        }
    }
}
