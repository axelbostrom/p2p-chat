using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model;

internal class NetworkManager : INotifyPropertyChanged
{
    private NetworkStream _stream;
    private TcpClient _tcpClient;
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

    public bool StartConnection()
    {
        try
        {
            Task.Factory.StartNew(() =>
            {
                _server = new Server();
                _server.StartListening();
            });


            // TODO: Client is started always, need to make this happen when the "Start Client" button is pressed.
            _tcpClient = new TcpClient();

            // Replace "127.0.0.1" and 13000 with your data from input
            _tcpClient.Connect("127.0.0.1", 13000);

            Task.Factory.StartNew(() => HandleConnection(_tcpClient));

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting server and client: {ex.Message}");
            return false;
        }
    }

    private void HandleConnection(TcpClient endPoint)
    {
        _stream = endPoint.GetStream();
        while (true)
        {
            var buffer = new byte[1024];
            int received = _stream.Read(buffer, 0, 1024);
            var message = Encoding.UTF8.GetString(buffer, 0, received);
            this.Message = message;
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
