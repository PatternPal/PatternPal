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
    public class NavigateHomeCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        
        private NavigationStore _navigationStore { get; }

        public NavigateHomeCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = new HomeViewModel(_navigationStore);
        }
    }
}
