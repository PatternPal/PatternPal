using IDesign.Extension.Commands;
using IDesign.Extension.Stores;
using System.Windows.Input;

namespace IDesign.Extension.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        public ICommand NavigateStepByStepListCommand { get; }

        public HomeViewModel(NavigationStore navigationStore)
        {
            NavigateStepByStepListCommand = new NavigateStepByStepListCommand(navigationStore);
        }
    }
}
