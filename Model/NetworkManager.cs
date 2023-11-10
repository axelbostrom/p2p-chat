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
            var ipEndPoint = new IPEndPoint(IPAddress.Loopback, 13000);
            TcpListener server = new TcpListener(ipEndPoint);
            TcpClient endPoint = null;

            try
            {
                server.Start();
                System.Diagnostics.Debug.WriteLine("Start Listening...");
                endPoint = server.AcceptTcpClient();
                System.Diagnostics.Debug.WriteLine("Connection established!");
                handleConnection(endPoint);
            }
            catch
            {
                secondTry = true;
            }

            if (secondTry)
            {
                endPoint = new TcpClient();
                try
                {
                    System.Diagnostics.Debug.WriteLine("Connecting to the server...");
                    endPoint.Connect(ipEndPoint);
                    System.Diagnostics.Debug.WriteLine("Connection established!");
                    handleConnection(endPoint);
                }
                finally
                {
                    endPoint.Close();

                }
            }
        });
        return true;
    }

    private void handleConnection(TcpClient endPoint)
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
    public void sendChar(string str)
    {
        Task.Factory.StartNew(() =>
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            _stream.Write(buffer, 0, str.Length);
        });
    }
}
