using IDesign.Extension.Stores;

namespace IDesign.Extension.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public MainViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        /// <summary>
        ///     Navigation Store used for switching between views
        /// </summary>
        private NavigationStore _navigationStore { get; }

        public ViewModel CurrentViewModel
        {
            get => _navigationStore.CurrentViewModel;
            set => _navigationStore.CurrentViewModel = value;
        }

        public override string Title => "IDesign";

        /// <summary>
        ///     Update view when the CurrentViewModel is changed
        /// </summary>
        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }
}
