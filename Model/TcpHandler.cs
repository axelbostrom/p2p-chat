using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    internal class TcpHandler : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private TcpListener _tcpListener;

        public TcpHandler()
        {
            _tcpListener = new TcpListener(IPAddress.Loopback, 13000);
        }

        public TcpClient AcceptConnection()
        {
            _tcpListener.Start();
            return _tcpListener.AcceptTcpClient();
        }

        public TcpClient ConnectToServer()
        {
            var endPoint = new IPEndPoint(IPAddress.Loopback, 13000);
            var tcpClient = new TcpClient();
            tcpClient.Connect(endPoint);
            return tcpClient;
        }

        public void CloseListener()
        {
            _tcpListener.Stop();
        }
    }
}
