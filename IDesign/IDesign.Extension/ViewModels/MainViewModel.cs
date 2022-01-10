using System.Windows;
using System.Windows.Input;
using IDesign.Extension.Stores;
using IDesign.Extension.Commands;

namespace IDesign.Extension.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public MainViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            BackCommand = new BackCommand(navigationStore);
        }

        /// <summary>
        /// Navigation Store used for switching between views
        /// </summary>
        private NavigationStore _navigationStore { get; }

        public ViewModel CurrentViewModel
        {
            get => _navigationStore.CurrentViewModel;
            set => _navigationStore.CurrentViewModel = value;
        }

        public override string Title => "IDesign";

        public ICommand BackCommand { get; }
        public Visibility BackButtonVisibility
        {
            get => CurrentViewModel.GetType() == typeof(HomeViewModel)
                ? Visibility.Hidden
                : Visibility.Visible;
        }

        /// <summary>
        /// Update view and back button when the CurrentViewModel is changed
        /// </summary>
        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
            OnPropertyChanged(nameof(BackButtonVisibility));
        }
    }
}
