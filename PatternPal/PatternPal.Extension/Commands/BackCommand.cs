using System;
using System.Windows.Input;
using PatternPal.Extension.Stores;

namespace PatternPal.Extension.Commands
{
    public class BackCommand : ICommand
    {

        private NavigationStore _navigationStore { get; }

        public event EventHandler CanExecuteChanged;

        public BackCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _navigationStore.Back();
        }
    }
}
