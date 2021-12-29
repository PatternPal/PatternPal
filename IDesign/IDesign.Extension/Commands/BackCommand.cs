using System;
using System.Windows.Input;
using IDesign.Extension.Stores;

namespace IDesign.Extension.Commands
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
            _navigationStore.CurrentViewModel = _navigationStore.Back();
        }
    }
}
