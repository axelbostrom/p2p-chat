using System;
using System.ComponentModel;
using System.IO;
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

                OnEventOccurred("CLIENT_BOOT_SUCCESS");

                _isConnected = true;

                Task.Run(ReceiveMessages);

            }
            catch (SocketException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Socket error connecting to server: {ex.Message}");
                _isConnected = false;
                OnEventOccurred("ERROR_CONNECT_SERVER");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error connecting to server: {ex.Message}");
                _isConnected = false;
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
                        _isConnected = false;
                        break;
                    }

                    string recievedMessage = String.Empty;

                    recievedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    Message message = JsonSerializer.Deserialize<Message>(recievedMessage);

                    OnMessageReceived(message);
                }
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network error handling client: {ex.Message}");
                _isConnected = false;
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deserializing JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error handling client: {ex.Message}");
            }
            finally
            {
                _isConnected = false;
                _tcpClient.Close();
            }
        }

        public async Task SendMessage(Message message)
        {
            await Task.Run(() =>
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
                catch (IOException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Network error sending message: {ex.Message}");
                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error serializing JSON: {ex.Message}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unexpected error sending message: {ex.Message}");
                }
            });
        }


        // Implement IDisposable to release resources.
        public void Disconnect()
        {
            System.Diagnostics.Debug.WriteLine("Client disconnect");
            _isConnected = false;

            try
            {
                if (_stream != null)
                {
                    _stream.Close();
                    _stream.Dispose();
                }

                _tcpClient?.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during disconnect: {ex.Message}");
            }
            finally
            {
                OnEventOccurred("CLIENT_DISCONNECTED");
            }

        }

        public TcpClient GetTcpClient() { return _tcpClient; }
    }
}
