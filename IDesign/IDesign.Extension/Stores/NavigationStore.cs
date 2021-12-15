using System;
using IDesign.Extension.ViewModels;

namespace IDesign.Extension.Stores
{
    public class NavigationStore
    {
        private ViewModel _currentViewModel { get; set; }

        public ViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                CurrentViewModelChanged?.Invoke();
            }
        }

        public event Action CurrentViewModelChanged;
    }
}
