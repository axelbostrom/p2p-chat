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
            _messages = new List<Message>();
        }

        public class Conversation
        {
            public string User1 { get; set; }
            public string User2 { get; set; }
            public List<Message> Messages { get; set; }
        }

        internal void UpdateConversation(Message message)
        {
            if (_otherUser == null)
            {
                return;
            }
            _messages.Add(message);

            List<Conversation> existingConversations = new List<Conversation>();

            string existingJson = File.ReadAllText("history.json");

            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                existingConversations = JsonSerializer.Deserialize<List<Conversation>>(existingJson);
            }

            bool conversationExists = false;
            foreach (var conversation in existingConversations)
            {
                if ((conversation.User1 == _userName && conversation.User2 == _otherUser) ||
                    (conversation.User1 == _otherUser && conversation.User2 == _userName))
                {
                    conversation.Messages.Add(message);
                    conversationExists = true;
                    break;
                }
            }

            if (!conversationExists)
            {
                var newConversation = new Conversation
                {
                    User1 = _userName,
                    User2 = _otherUser,
                    Messages = _messages
                };
                existingConversations.Add(newConversation);
            }

            string updatedJson = JsonSerializer.Serialize(existingConversations,
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("history.json", updatedJson);
        }


        internal void UpdateOtherUser(string otherUser)
        {
            _otherUser = otherUser;
        }

        public Dictionary<string, DateTime> GetChatUserHistory()
        {
            if (!File.Exists("history.json"))
            {
                File.Create("history.json");
            }

            string existingJson = File.ReadAllText("history.json");

            List<Conversation> existingConversations = new List<Conversation>();

            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                existingConversations = JsonSerializer.Deserialize<List<Conversation>>(existingJson);
            }

            Dictionary<string, DateTime> userLastMessageTimestamp = new Dictionary<string, DateTime>();

            foreach (Conversation conv in existingConversations)
            {
                DateTime lastMessageTime = DateTime.MinValue;

                foreach (Message msg in conv.Messages)
                {
                    if (msg.Timestamp > lastMessageTime)
                    {
                        lastMessageTime = msg.Timestamp;
                    }
                }

                if (conv.User1.Equals(_userName))
                {
                    userLastMessageTimestamp.Add(conv.User2, lastMessageTime);
                    continue;
                }
                if (conv.User2.Equals(_userName))
                {
                    userLastMessageTimestamp.Add(conv.User1, lastMessageTime);
                }
            }

            return userLastMessageTimestamp;
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

            return Enumerable.Empty<Message>().ToList();
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
            return Enumerable.Empty<Message>().ToList();
        }
    }
}
