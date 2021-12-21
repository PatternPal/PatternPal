using System;
using System.Windows.Input;
using IDesign.Extension.Stores;
using IDesign.Extension.ViewModels;

namespace IDesign.Extension.Commands
{
    public class NavigateCommand<T> : ICommand where T : ViewModel
    {
        public NavigateCommand(NavigationStore navigationStore, Func<T> getViewModel)
        {
            _navigationStore = navigationStore;
            _getViewModel = getViewModel;
        }

        private NavigationStore _navigationStore { get; }
        private Func<T> _getViewModel { get; }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = _getViewModel();
        }
    }
}
