using ChatApp.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    /// <summary>
    /// Interaction logic for ChatViewModel.xaml
    /// </summary>
    public partial class ChatViewModel : Window
    {
        private ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();
        private ICommand _sendMessageCommand;

        public ChatViewModel()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ICommand SendMessageCommand
        {
            get
            {
                return new Command.SendMessageCommand(this);
            }
            set { }
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        internal void SendMessage()
        {
            string message = MessageTextBox.Text;
            System.Diagnostics.Debug.WriteLine(message);

            // Add message to chatbox here
            // AddMessage(message);

            // Clear messagebox
            MessageTextBox.Text = string.Empty;
        }

        internal void AddMessage(string sender, string message)
        {
            System.Diagnostics.Debug.WriteLine("Add Message " + message);
            // Messages.Add(new Message { Sender = sender, Timestamp = DateTime.Now, Content = message });
        }
    }
}
