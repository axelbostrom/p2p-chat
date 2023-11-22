﻿using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp.Model
{

    // The Server class is responsible for listening for incoming connections and managing client sessions.
    public class Server : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private TcpListener _tcpListener;
        private IPAddress _ipAddress;
        private int _port;
        public event EventHandler<string> EventOccured;
        public event EventHandler<string> MessageReceived;
        private TcpClient _client;

        public Server(IPAddress ipAddress, int port)
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

        private void OnMessageReceived(string message)
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
                _tcpListener = new TcpListener(_ipAddress, _port);
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

                OnEventOccurred("xd");

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
                    Application.Current.Dispatcher.Invoke(() => OnMessageReceived(recievedMessage));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling client: {ex.Message}");
                OnEventOccurred("Error handling client.");
            }
        }

        public void SendMessage(string message)
        {
            // Check if a client is connected before attempting to send a message
            System.Diagnostics.Debug.WriteLine(_client.Connected);
            if (_client != null && _client.Connected)
            {
                NetworkStream stream = _client.GetStream();

                try
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    System.Diagnostics.Debug.WriteLine("Sending message to stream " + message);
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
