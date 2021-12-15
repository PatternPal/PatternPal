using System.Windows.Input;
using IDesign.Extension.Commands;
using IDesign.Extension.Stores;

namespace IDesign.Extension.ViewModels
{
    public class StepByStepListViewModel : ViewModel
    {
        public StepByStepListViewModel(NavigationStore navigationStore)
        {
            NavigateHomeCommand =
                new NavigateCommand<HomeViewModel>(navigationStore, () => new HomeViewModel(navigationStore));
        }

        public override string Title => "Step By Step Instructions";

        public ICommand NavigateHomeCommand { get; }
    }
}
