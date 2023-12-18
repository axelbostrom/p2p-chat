using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ChatApp.Model
{
    public class MessageHistory
    {
        private string _userName;
        private string _otherUser;
        private List<Message> _messages;
        private int _chatId;

        public MessageHistory(string userName)
        {
            _userName = userName;
            _messages = new List<Message>();
        }

        public int GetLatestChatId()
        {
            string jsonData = File.ReadAllText("history.json");

            List<Conversation> conversations = new List<Conversation>();

            if (!string.IsNullOrWhiteSpace(jsonData))
            {
                conversations = JsonSerializer.Deserialize<List<Conversation>>(jsonData);
            }

            if (conversations.Count > 0)
            {
                int latestChatId = conversations
                    .Max(c => c.chatId);

                return latestChatId;
            }

            return 0; // The lowest is 1
        }

        public List<Message> GetMessages()
        {
            string jsonData = File.ReadAllText("history.json");

            List<Conversation> conversations = new List<Conversation>();

            if (!string.IsNullOrWhiteSpace(jsonData))
            {
                conversations = JsonSerializer.Deserialize<List<Conversation>>(jsonData);
            }
            
            Conversation targetConversation = conversations
                .FirstOrDefault(c => c.chatId == _chatId);

            if (targetConversation != null)
            {
                return targetConversation.Messages;
            }
            

            return new List<Message>(); // Return an empty list if conversation/messages not found
        }


        public class Conversation
        {
            public int chatId { get; set; }
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

            List<Conversation> existingConversations = new List<Conversation>();

            string existingJson = File.ReadAllText("history.json");

            if (!string.IsNullOrWhiteSpace(existingJson))
            {
                existingConversations = JsonSerializer.Deserialize<List<Conversation>>(existingJson);
            }

            Conversation conversationToUpdate = existingConversations.FirstOrDefault(c => c.chatId == _chatId);

            if (conversationToUpdate != null)
            {
                conversationToUpdate.Messages.Add(message);
            }
            else
            {
                var newConversation = new Conversation
                {
                    chatId = _chatId,
                    User1 = _userName,
                    User2 = _otherUser,
                    Messages = new List<Message> { message }
                };
                existingConversations.Add(newConversation);
            }

            string updatedJson = JsonSerializer.Serialize(existingConversations,
                new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText("history.json", updatedJson);
        }



        public int UpdateOtherUser(string otherUser)
        {
            _otherUser = otherUser;
            _messages.Clear();
            _chatId = GetLatestChatId() + 1;
            return _chatId;
        }

        public Dictionary<DateTime, (string, int)> GetChatUserHistory()
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

            Dictionary<DateTime, (string, int)> userLastMessageTimestamp = new Dictionary<DateTime, (string, int)>();

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
                    userLastMessageTimestamp[lastMessageTime] = (conv.User2, conv.chatId);
                    continue;
                }
                if (conv.User2.Equals(_userName))
                {
                    userLastMessageTimestamp[lastMessageTime] = (conv.User1, conv.chatId);
                }
            }

            return userLastMessageTimestamp;
        }

        public List<Message> GetChatHistory(int id)
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
                if (conv.chatId == id) 
                {
                    return conv.Messages;
                }
            }

            return Enumerable.Empty<Message>().ToList();
        }
    }
}
