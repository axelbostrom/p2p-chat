﻿using ChatApp.Model;
using ChatApp.ViewModel;
using System.Windows;

namespace ChatApp.View
{
    /// <summary>
    /// Interaction logic for ChatViewModel.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        public ChatWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            this.Closing += viewModel.onClose;
        }
    }
}
