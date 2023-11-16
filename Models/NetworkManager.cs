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

    private void OnPropertyChanged(string propertyName = null)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private string _message;

    public string Message
    {
        get { return _message; }
        set { _message = value; OnPropertyChanged("Message"); }
    }

    public async Task<bool> StartServer(User user)
    {
        try
        {
            // TODO: try and catch clause to reassure address or port are correct/not NULL
            _server = new Server(user.Address, user.Port);
            await Task.Run(() => _server.StartListening());


            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting server: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> StartClient(User user) 
    {
        try
        {
            _client = new Client(user.Address, user.Port);
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
