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
    private TcpHandler _tcpHandler;
    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(string propertyName = null)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private string message;

    public string Message
    {
        get { return message; }
        set { message = value; OnPropertyChanged("Message"); }
    }

    public bool StartConnection()
    {
        Task.Factory.StartNew(() =>
        {
            bool secondTry = false;
            _tcpHandler = new TcpHandler();

            try
            {
                System.Diagnostics.Debug.WriteLine("Connecting to the server...");
                TcpClient endPoint = _tcpHandler.AcceptConnection();
                System.Diagnostics.Debug.WriteLine("Connection established!");
                HandleConnection(endPoint);
            }
            catch
            {
                secondTry = true;
            }

            if (secondTry)
            {
                System.Diagnostics.Debug.WriteLine("Connecting to the server...");
                TcpClient endPoint = _tcpHandler.AcceptConnection();
                System.Diagnostics.Debug.WriteLine("Connection established!");
                HandleConnection(endPoint);
            }
        });

        return true;
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
            _stream.Write(buffer, 0, str.Length);
        });
    }
}
