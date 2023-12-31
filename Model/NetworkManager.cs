using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp.Model;

public class NetworkManager
{
    private Client _client;
    private Server _server;
    private User _user;
    public event EventHandler<string> EventOccured;
    public event EventHandler<Message> MessageReceived;
    private bool _isClient = false;

    private void OnEventOccurred(string errorMessage)
    {
        EventOccured?.Invoke(this, errorMessage);
    }

    private void OnMessageReceived(Message message)
    {
        Application.Current.Dispatcher.Invoke(() => MessageReceived?.Invoke(this, message));
    }

    public async Task<bool> StartServer(User user)
    {
        try
        {
            _user = user;
            _server = new Server(user);
            _server.EventOccured += (sender, errorMessage) => OnEventOccurred(errorMessage);
            _server.MessageReceived += (sender, message) => OnMessageReceived(message);
            IsClient = false;
            await Task.Run(() => _server.StartListening());
            SendConnectionEstablished();

            return true;
        }
        catch (ArgumentNullException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error starting client: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> StartClient(User user)
    {
        try
        {
            _user = user;
            _client = new Client(user);
            _client.EventOccured += (sender, errorMessage) => OnEventOccurred(errorMessage);
            _client.MessageReceived += (sender, message) => OnMessageReceived(message);
            IsClient = true;
            await Task.Run(() => _client.Connect());
            SendConnectionEstablished();
            
            return true;
        }
        catch (ArgumentNullException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error starting client: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
            return false;
        }
    }

    internal void SendChatMessage(string message)
    {
        Message messageToSend = new Message(MessageType.Message, _user.Name, DateTime.Now, message);
        _client?.SendMessage(messageToSend);
        _server?.SendMessage(messageToSend);
    }

    private void SendConnectionEstablished()
    {
        Message connectionRequestMessage = new Message(MessageType.ConnectionEstablished, _user.Name);

        _client?.SendMessage(connectionRequestMessage);
        _server?.SendMessage(connectionRequestMessage);
    }

    internal void SendConnectionAccepted()
    {
        Message connectionAcceptMessage = new Message(MessageType.AcceptConnection, _user.Name);

        _server?.SendMessage(connectionAcceptMessage);
    }

    internal async Task SendConnectionDeniedAsync()
    {
        Message connectionDenyMessage = new Message(MessageType.DenyConnection, _user.Name);

        await _server?.SendMessage(connectionDenyMessage);
        _server?.DenyClientConnection();
    }

    internal async Task SendDisconnect()
    {
        Message disconnectMessage = new Message(MessageType.Disconnect, _user.Name);

        Task serverSendTask = _server?.SendMessage(disconnectMessage);
        Task clientSendTask = _client?.SendMessage(disconnectMessage);

        if (serverSendTask != null) await serverSendTask;
        if (clientSendTask != null) await clientSendTask;
    }

    internal void SendBuzzMessage()
    {
        Message sendBuzzMessage = new Message(MessageType.Buzz, _user.Name);

        _server?.SendMessage(sendBuzzMessage);
        _client?.SendMessage(sendBuzzMessage);
    }

    public bool IsClient
    {
        get { return _isClient; }
        set { _isClient = value;  }
    }

    public Client Client { get { return _client; } }

    public Server Server { get { return _server; } }
}
