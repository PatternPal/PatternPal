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
    public class NavigateStepByStepListCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private NavigationStore _navigationStore { get; }

        public NavigateStepByStepListCommand(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _navigationStore.CurrentViewModel = new StepByStepListViewModel(_navigationStore);
        }
    }
}
