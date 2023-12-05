using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp.Model
{

    // The Client class is responsible for initiating connections to the server.
    public class Client : INotifyPropertyChanged
    {
        private User _user;
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<string> EventOccured;
        public event EventHandler<Message> MessageReceived;

        private bool _isConnected;


        public Client(User user)
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

        public void Connect()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Client is looking for server to connect to...");

                _tcpClient = new TcpClient(_user.Address.ToString(), _user.Port);

                _stream = _tcpClient.GetStream();

                OnEventOccurred("Client booted up successfully!");

                _isConnected = true;

                Task.Run(ReceiveMessages);

            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Error connecting to server: {ex.Message}");
                OnEventOccurred("Error connecting to server!");
            }
        }

        // Method to continuously receive messages from the server.
        private void ReceiveMessages()
        {
            try
            {
                while (_isConnected)
                {
                    System.Diagnostics.Debug.WriteLine("Recieving");
                    byte[] buffer = new byte[1024];
                    int bytesRead = _stream.Read(buffer, 0, buffer.Length);

                    if (bytesRead <= 0)
                    {
                        // The client has disconnected
                        break;
                    }

                    string recievedMessage = String.Empty;

                    recievedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // Deserialize the received JSON message
                    Message message = JsonSerializer.Deserialize<Message>(recievedMessage);

                    // Handle the received message
                    OnMessageReceived(message);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error receiving messages: {ex.Message}");
            }
            finally
            {
                //Dis(); // Close the client when the loop exits
            }
        }

        public void SendMessage(Message message)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (_stream != null && _stream.CanWrite && _isConnected)
                    {
                        string jsonMessage = JsonSerializer.Serialize(message);

                        var buffer = Encoding.UTF8.GetBytes(jsonMessage);

                        _stream.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Error: Stream is not ready for writing.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error sending message: {ex.Message}");
                }
            });
        }


        // Implement IDisposable to release resources.
        public void Disconnect()
        {
            _isConnected = false;
            _stream?.Dispose();
            _tcpClient?.Close();

        }

        public TcpClient GetTcpClient() { return _tcpClient; }
    }
}
