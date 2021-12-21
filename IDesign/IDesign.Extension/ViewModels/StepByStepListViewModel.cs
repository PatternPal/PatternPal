using System.Windows.Input;
using IDesign.Extension.Commands;
using IDesign.Extension.Stores;

namespace IDesign.Extension.ViewModels
{
    public class StepByStepListViewModel : ViewModel
    {
        public override string Title => Resources.ExtensionUIResources.StepByStepTitle;

        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateStepByStepInstructionsCommand { get; }


        public StepByStepListViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(navigationStore, () => new HomeViewModel(navigationStore));
            NavigateStepByStepInstructionsCommand = new NavigateCommand<StepByStepInstructionsViewModel>(navigationStore, () => new StepByStepInstructionsViewModel(navigationStore));

        }
    }
}
