using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model;

internal class NetworkManager : INotifyPropertyChanged
{
    private NetworkStream _stream;
    private Client _client;
    private Server _server;
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<string> EventOccured;

    private string _message;

    private void OnPropertyChanged(string propertyName = null)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private void OnEventOccurred(string errorMessage)
    {
        EventOccured?.Invoke(this, errorMessage);
    }

    public string Message
    {
        get { return _message; }
        set { _message = value; OnPropertyChanged("Message"); }
    }

    // TODO: Remove nestled try/catch?
    public async Task<bool> StartServer(User user)
    {
        try
        {
            // TODO: add checks of address and port before calling server or check in server?
            _server = new Server(user.Address, user.Port);
            _server.EventOccured += (sender, errorMessage) => OnEventOccurred(errorMessage);
            await Task.Run(() => _server.StartListening());
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting server: {ex.Message}");
            return false;
        }
    }

    // TODO: Remove nestled try/catch?
    public async Task<bool> StartClient(User user) 
    {
        try
        {
            // TODO: add checks of address and port before calling client or check in client?
            _client = new Client(user.Address, user.Port);
            _client.EventOccured += (sender, errorMessage) => OnEventOccurred(errorMessage);
            await Task.Run(() => _client.Connect());

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting client: {ex.Message}");
            return false;
        }
    }

    private void HandleConnection(TcpClient endPoint)
    {
        try {
            _stream = endPoint.GetStream();

            while (true)
            {
                var buffer = new byte[1024];
                int received = _stream.Read(buffer, 0, 1024);
                var message = Encoding.UTF8.GetString(buffer, 0, received);
                this.Message = message;
                System.Diagnostics.Debug.WriteLine(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling connection: {ex.Message}");
        }

    }

    public void SendChar(string str)
    {
        Task.Factory.StartNew(() =>
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            _stream.Write(buffer, 0, buffer.Length);
        });
    }
}
