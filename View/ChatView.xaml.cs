using ChatApp.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace ChatApp.ViewModel
{
    /// <summary>
    /// Interaction logic for ChatViewModel.xaml
    /// </summary>
    public partial class ChatViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Message> Messages { get; set; } = new ObservableCollection<Message>();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ChatViewModel()
        {
            InitializeComponent();
        }

        public ICommand SendMessageCommand
        {
            get
            {
                return new Command.SendMessageCommand(this);
            }
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

        internal void AddMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine("Add Message " + message);
            // Messages.Add(new Message { Sender = sender, Timestamp = DateTime.Now, Content = message });
        }
    }
}
