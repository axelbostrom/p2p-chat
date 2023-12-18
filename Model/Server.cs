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

                OnEventOccurred("SERVER_BOOT_SUCCESS");
                while (true)
                {
                    _client = _tcpListener.AcceptTcpClient();
                    Task.Factory.StartNew(() => ReceiveMessages(_client));
                    System.Diagnostics.Debug.WriteLine("Server connected!");
                }
            }
            catch (SocketException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Socket error: {ex.ErrorCode}");
                switch (ex.SocketErrorCode)
                {
                    case SocketError.AddressAlreadyInUse:
                        OnEventOccurred("ADDRESS_IN_USE");
                        break;
                    default:
                        OnEventOccurred("Error connecting to server!");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
            }
            finally
            {
                _tcpListener.Stop();
            }
        }

        private void ReceiveMessages(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                byte[] data = new byte[256];

                string recievedMessage = String.Empty;

                while (true)
                {

                    int bytesRead = stream.Read(data, 0, data.Length);

                    if (bytesRead <= 0)
                    {
                        break;
                    }

                    recievedMessage = Encoding.ASCII.GetString(data, 0, bytesRead);

                    Message message = JsonSerializer.Deserialize<Message>(recievedMessage);

                    OnMessageReceived(message);
                }

            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Network error handling client: {ex.Message}");
                OnEventOccurred("Error handling client.");
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
                client.Close();
            }
        }

        public async Task SendMessage(Message message)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (_client != null && _client.Connected)
                    {
                        NetworkStream stream = _client.GetStream();

                        string jsonMessage = JsonSerializer.Serialize(message);

                        var buffer = Encoding.UTF8.GetBytes(jsonMessage);

                        stream.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No connected clients to send a message to.");
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

        public void DenyClientConnection()
        {
            _client.Close();
        }

        public void StopServer()
        {
            try
            {
                _client?.GetStream()?.Dispose();
            }
            catch (SocketException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            _client?.Close();
            _tcpListener?.Stop();
        }

    }
}
