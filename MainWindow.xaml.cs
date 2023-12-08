using System;
using ChatApp.Model;
using ChatApp.ViewModel;
using System.Windows;

namespace ChatApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel viewModel = new MainWindowViewModel(this);
            this.DataContext = viewModel;
            this.Closing += onClose;
        }

        private void onClose(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
