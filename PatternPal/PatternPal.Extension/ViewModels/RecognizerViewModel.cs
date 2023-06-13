#region

using System.Windows.Input;
using PatternPal.Extension.Commands;
using PatternPal.Extension.Stores;

#endregion

namespace PatternPal.Extension.ViewModels
{
    // TODO comment
    public class RecognizerViewModel : ViewModel
    {
        public override string Title => Resources.ExtensionUIResources.DetectorTitle;
        public ICommand NavigateHomeCommand { get; }
        public RecognizerViewModel(NavigationStore navigationStore) 
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(navigationStore, () => new HomeViewModel(navigationStore));
        }
    }
}
