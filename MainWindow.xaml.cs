using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ChatApp.Model;
using ChatApp.ViewModel;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPAddress ip;
        private int port;
        private string name;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(new NetworkManager());
        }

        private void TextBox_Name(object sender, TextChangedEventArgs e)
        { 
            if (sender is TextBox textBox)
            {
                name = textBox.Text;
            }
        }

        private void TextBox_Port(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (int.TryParse(textBox.Text, out int parsedPort))
                {
                    port = parsedPort;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Not a valid port number!");
                }
            }
            
        }

        private void TextBox_Ip(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                //ip = textBox.Text;
            }
        }
    }
}
