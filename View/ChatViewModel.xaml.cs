using ChatApp.Model;
using ChatApp.ViewModel;
using System.Windows;
using System.Xml.Linq;

namespace ChatApp.ViewModels
{
    /// <summary>
    /// Interaction logic for ChatViewModel.xaml
    /// </summary>
    public partial class ChatViewModel : Window
    {
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
