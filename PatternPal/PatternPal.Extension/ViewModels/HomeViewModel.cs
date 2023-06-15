#region

using System.Windows.Input;

using PatternPal.Extension.Resources;
using PatternPal.Extension.Stores;

#endregion

namespace PatternPal.Extension.ViewModels
{
    /// <summary>
    /// ViewModel for the home screen.
    /// </summary>
    public class HomeViewModel : ViewModel
    {
        /// <summary>
        /// Gets the title of the home screen.
        /// </summary>
        public override string Title => ExtensionUIResources.ExtensionName;

        /// <summary>
        /// Gets the command to navigate to the step-by-step list screen.
        /// </summary>
        public ICommand NavigateStepByStepListCommand { get; }

        /// <summary>
        /// Gets the command to navigate to the detector screen.
        /// </summary>
        public ICommand NavigateDetectorCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeViewModel"/> class.
        /// </summary>
        /// <param cref="navigationStore" name="navigationStore">The navigation store.</param>
        public HomeViewModel(
            NavigationStore navigationStore)
        {
            NavigateStepByStepListCommand = new NavigateCommand< StepByStepListViewModel >(
                navigationStore,
                () => new StepByStepListViewModel(navigationStore));
            NavigateDetectorCommand =
                new NavigateCommand< RecognizerViewModel >(
                    navigationStore,
                    () => new RecognizerViewModel());
        }
    }
}
