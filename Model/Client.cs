using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace ChatApp.Model
{
    public class Client : INotifyPropertyChanged
    {
        private IPAddress _ipAddress;
        private int _port;
        private TcpClient _tcpClient;
        private NetworkStream _stream;
        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<string> EventOccured;


        public Client(IPAddress ipAddress, int port)
        {
            _ipAddress = ipAddress;
            _port = port;
        }

        private void OnEventOccurred(string eventMessage)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                EventOccured?.Invoke(this, eventMessage);
            });
        }

        public void Connect()
        {
            try
            {
                // FROM https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=net-7.0
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer
                // connected to the same address as specified by the server, port
                // combination.

                // Prefer a using declaration to ensure the instance is Disposed later.
                System.Diagnostics.Debug.WriteLine("Client is looking for server to connect to...");

                _tcpClient = new TcpClient(_ipAddress.ToString(), _port);

                // Translate the passed message into ASCII and store it as a Byte array.
                // Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                _stream = _tcpClient.GetStream();

                // Notify subscribers that the connection is successful
                System.Diagnostics.Debug.WriteLine("Client connected!");
                OnEventOccurred("Booted up succesfully!");

                // Send the message to the connected TcpServer.
                // _stream.Write(data, 0, data.Length);

                // Console.WriteLine("Sent: {0}", message);

                // Receive the server response.

                // Buffer to store the response bytes.
                // data = new Byte[256];

                // String to store the response ASCII representation.
                // String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                // Int32 bytes = _stream.Read(data, 0, data.Length);
                // responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                // System.Diagnostics.Debug.WriteLine("Received: {0}", responseData);
            }
            catch (Exception ex)
            {

                System.Diagnostics.Debug.WriteLine($"Error connecting to server: {ex.Message}");
                OnEventOccurred("Error connecting to server!");
            }
        }

        public TcpClient GetTcpClient() { return _tcpClient; }
    }
}
