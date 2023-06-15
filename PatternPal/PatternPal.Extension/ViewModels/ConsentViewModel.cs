#region

using System.Windows.Input;
using PatternPal.Extension.Commands;
using PatternPal.Extension.Stores;

#endregion

namespace PatternPal.Extension.ViewModels
{
/// <summary>
/// ViewModel for the consent screen.
/// </summary>
public class ConsentViewModel : ViewModel
{
    /// <summary>
    /// Gets the title of the consent screen.
    /// </summary>
    public override string Title => Resources.ExtensionUIResources.ExtensionName;

    /// <summary>
    /// Gets the command to navigate to the home screen.
    /// </summary>
    public ICommand NavigateHomeCommand { get; }

    /// <summary>
    /// Gets or sets the privacy configuration.
    /// </summary>
    public Privacy ConfigPrivacy { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsentViewModel"/> class.
    /// </summary>
    /// <param name="navigationStore">The navigation store.</param>
    /// <param name="configPrivacy">The privacy configuration.</param>
    public ConsentViewModel(NavigationStore navigationStore, Privacy configPrivacy)
    {
        NavigateHomeCommand = new NavigateCommand<HomeViewModel>(navigationStore,
                                () => new HomeViewModel(navigationStore));
        ConfigPrivacy = configPrivacy;
    }
}
}
