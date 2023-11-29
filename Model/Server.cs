using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp.Model
{

    // The Server class is responsible for listening for incoming connections and managing client sessions.
    public class Server : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private TcpListener _tcpListener;
        private User _user;
        public event EventHandler<string> EventOccured;
        public event EventHandler<Message> MessageReceived;
        private TcpClient _client;

        public bool acceptOrDeny;
        private TaskCompletionSource<bool> _userResponse = new TaskCompletionSource<bool>();

        public Server(User user)
        {
            _user = user;
        }

        private void OnEventOccurred(string eventMessage)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                EventOccured?.Invoke(this, eventMessage);
            });
        }

        private void OnMessageReceived(Message message)
        {
            MessageReceived?.Invoke(this, message);
        }

        private void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        public void StartListening()
        {
            try
            {
                _tcpListener = new TcpListener(_user.Address, _user.Port);
                _tcpListener.Start();
                System.Diagnostics.Debug.WriteLine("Server is waiting for client to connect...");

                OnEventOccurred("Booted up succesfully!");
                _client = _tcpListener.AcceptTcpClient();
                Task.Factory.StartNew(() => RecieveMessages(_client));
                System.Diagnostics.Debug.WriteLine("Server connected!");
                OnEventOccurred("Connected!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error connecting to client: {ex.Message}");
                OnEventOccurred("Error connecting to client!");
            }
            finally
            {
                _tcpListener.Stop();
            }
        }

        private void RecieveMessages(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                OnEventOccurred("New Client Connection");

                    // Buffer to store the response bytes.
                    byte[] data = new byte[256];

                    // String to store the response ASCII representation.
                    string recievedMessage = String.Empty;

                    while (true)
                    {

                        // Read the first batch of the client's data.
                        int bytesRead = stream.Read(data, 0, data.Length);

                        if (bytesRead <= 0)
                        {
                            // The client has disconnected
                            break;
                        }

                        recievedMessage = Encoding.ASCII.GetString(data, 0, bytesRead);
                        System.Diagnostics.Debug.WriteLine($"Received from client: {recievedMessage}");

                        // Deserialize the received JSON message
                        Message message = JsonSerializer.Deserialize<Message>(recievedMessage);

                        // Handle the received message
                        OnMessageReceived(message);
                    }
                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling client: {ex.Message}");
                OnEventOccurred("Error handling client.");
            }
            finally
            {
                client.Close();
            }
        }

        public void SendMessage(Message message)
        {
            // Check if a client is connected before attempting to send a message
            // System.Diagnostics.Debug.WriteLine(_client.Connected);
            if (_client != null && _client.Connected)
            {
                NetworkStream stream = _client.GetStream();

                try
                {
                    string jsonMessage = JsonSerializer.Serialize(message);

                    var buffer = Encoding.UTF8.GetBytes(jsonMessage);

                    //System.Diagnostics.Debug.WriteLine("Sending message to stream " + message);
                    stream.Write(buffer, 0, buffer.Length);
                }
                catch (Exception)
                {
                    System.Diagnostics.Debug.WriteLine("Error sending message from server: {ex.Message}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No connected clients to send a message to.");
            }
        }
    }
}
