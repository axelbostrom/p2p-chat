using ChatApp.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace ChatApp.ViewModels
{
    /// <summary>
    /// Interaction logic for ChatViewModel.xaml
    /// </summary>
    public partial class ChatViewModel : Window
    {
        private ObservableCollection<Message> Messages = new ObservableCollection<Message>();

        public ChatViewModel()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }

        private void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            // Call the StartServer method in NetworkManager
            string message = MessageTextBox.Text;
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
