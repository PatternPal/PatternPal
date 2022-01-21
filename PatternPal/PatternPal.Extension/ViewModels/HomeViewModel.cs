using System.Windows.Input;
using PatternPal.Extension.Commands;
using PatternPal.Extension.Resources;
using PatternPal.Extension.Stores;

namespace PatternPal.Extension.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        public override string Title => Resources.ExtensionUIResources.ExtensionName;
        public ICommand NavigateStepByStepListCommand { get; }
        public ICommand NavigateDetectorCommand { get; }

        public HomeViewModel(NavigationStore navigationStore)
        {
            NavigateStepByStepListCommand = new NavigateCommand<StepByStepListViewModel>(navigationStore,
                () => new StepByStepListViewModel(navigationStore));
            NavigateDetectorCommand =
                new NavigateCommand<DetectorViewModel>(navigationStore, () => new DetectorViewModel(navigationStore));
        }

    }
}
