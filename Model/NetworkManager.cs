using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatApp.Model;

public class NetworkManager : INotifyPropertyChanged
{
    private NetworkStream _stream;
    private Client _client;
    private Server _server;
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<string> EventOccured;
    public event EventHandler<Message> MessageReceived;

    private Message _message;

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

    private void OnMessageReceived(Message message)
    {
        System.Diagnostics.Debug.WriteLine("Message recieved in NwM " + message.Content);
        MessageReceived?.Invoke(this, message);
    }

    public Message Message
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
            _server = new Server(user);
            _server.EventOccured += (sender, errorMessage) => OnEventOccurred(errorMessage);
            _server.MessageReceived += (sender, message) => OnMessageReceived(message);
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
            _client = new Client(user);
            _client.EventOccured += (sender, errorMessage) => OnEventOccurred(errorMessage);
            _client.MessageReceived += (sender, message) => OnMessageReceived(message);
            await Task.Run(() =>
            {
                _client.Connect();
            });

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting client: {ex.Message}");
            return false;
        }
    }

    public void SendUserMessage(Message message)
    {
        _client?.SendMessage(message);
        _server?.SendMessage(message);
    }

    public Client Client { get { return _client; } }

    public Server Server { get { return _server; } }
}
