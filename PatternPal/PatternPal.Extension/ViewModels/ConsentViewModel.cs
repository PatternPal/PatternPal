#region

using System.Windows.Input;
using PatternPal.Extension.Commands;
using PatternPal.Extension.Stores;

#endregion

namespace PatternPal.Extension.ViewModels
{
    // TODO Comment
    public class ConsentViewModel : ViewModel
    {
        public override string Title => Resources.ExtensionUIResources.ExtensionName;

        public ICommand NavigateHomeCommand { get; }

        public Privacy ConfigPrivacy { get; set; }
        public ConsentViewModel(NavigationStore navigationStore, Privacy configPrivacy)
        {
            NavigateHomeCommand = new NavigateCommand<HomeViewModel>(navigationStore,
                                () => new HomeViewModel(navigationStore));
            ConfigPrivacy = configPrivacy;
        }

    }
}
