using IDesign.Extension.Commands;
using IDesign.Extension.Stores;
using System.Windows.Input;

namespace IDesign.Extension.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        public ICommand NavigateStepByStepListCommand { get; }
        public ICommand NavigateDetectorCommand { get; }

        public override string Title => "IDesign";

        public HomeViewModel(NavigationStore navigationStore)
        {
            NavigateStepByStepListCommand = new NavigateCommand<StepByStepListViewModel>(navigationStore, () => new StepByStepListViewModel(navigationStore));
            NavigateDetectorCommand = new NavigateCommand<DetectorViewModel>(navigationStore, () => new DetectorViewModel());
        }
    }
}
