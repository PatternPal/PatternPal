using IDesign.Extension.Stores;
using IDesign.Extension.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDesign.Extension.Commands
{
    public class NavigateCommand<T> : ICommand where T : ViewModel
    {
        public event EventHandler CanExecuteChanged;
        
        private NavigationStore _navigationStore { get; }
        private Func<T> _getViewModel { get; }

        public NavigateCommand(NavigationStore navigationStore, Func<T> getViewModel)
        {
            _navigationStore = navigationStore;
            _getViewModel = getViewModel;
        }

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
