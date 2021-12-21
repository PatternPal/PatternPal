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
    public class StepByStepInstructionsViewModel : ViewModel
    {
        public override string Title => "Strategy 1/15";

        public ICommand NavigateHomeCommand { get; }

        public StepByStepInstructionsViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(navigationStore, () => new HomeViewModel(navigationStore));
        }
    }
}