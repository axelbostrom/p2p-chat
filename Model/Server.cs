﻿using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp.Model
{
    public class Server : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private TcpListener _tcpListener;
        private IPAddress _ipAddress;
        private int _port;
        public event EventHandler<string> EventOccured;

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

        public void StartListening()
        {
            try
            {
                _tcpListener = new TcpListener(_ipAddress, _port);
                _tcpListener.Start();
                System.Diagnostics.Debug.WriteLine("Server is waiting for client to connect...");

                OnEventOccurred("Booted up succesfully!");

                TcpClient client = _tcpListener.AcceptTcpClient();
                Task.Factory.StartNew(() => HandleClient(client));
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

        private void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                // Buffer to store the response bytes.
                byte[] data = new byte[256];

                // String to store the response ASCII representation.
                string responseData = String.Empty;

                // Read the first batch of the client's data.
                int bytesRead = stream.Read(data, 0, data.Length);

                while (bytesRead > 0)
                {
                    responseData = Encoding.ASCII.GetString(data, 0, bytesRead);
                    System.Diagnostics.Debug.WriteLine($"Received from client: {responseData}");

                    // Process the data as needed

                    // Echo the data back to the client
                    stream.Write(data, 0, bytesRead);
                    System.Diagnostics.Debug.WriteLine($"Sent to client: {responseData}");

                    bytesRead = stream.Read(data, 0, data.Length);
                }

                // Close the client connection
                client.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling client: {ex.Message}");
                OnEventOccurred("Error handling client.");
            }
        }
    }
}
