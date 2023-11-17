using System;
using System.Windows.Input;

namespace ChatApp.ViewModel.Command
{
    internal class StartChatCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private MainWindowViewModel parent = null;


        public StartChatCommand(MainWindowViewModel parent)
        {
            this.parent = parent;

        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            parent.startChatViewModel();
        }
    }
}
