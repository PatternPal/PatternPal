using IDesign.Extension.Commands;
using IDesign.Extension.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IDesign.Extension.ViewModels
{
    public class StepByStepListViewModel : ViewModel
    {
        public override string Title => Resources.ExtensionUIResources.StepByStepTitle;

        public ICommand NavigateHomeCommand { get; }

        public StepByStepListViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(navigationStore, () => new HomeViewModel(navigationStore));
        }
    }
}
