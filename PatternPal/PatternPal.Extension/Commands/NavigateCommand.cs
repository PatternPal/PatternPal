#region

using System;
using System.Windows.Input;

using PatternPal.Extension.Stores;
using PatternPal.Extension.ViewModels;

#endregion

namespace PatternPal.Extension.Commands
{
    public class NavigateCommand< T > : ICommand
        where T : ViewModel
    {
        private NavigationStore NavigationStore { get; }
        private Func< T > GetViewModel { get; }

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public NavigateCommand(
            NavigationStore navigationStore,
            Func< T > getViewModel)
        {
            NavigationStore = navigationStore;
            GetViewModel = getViewModel;
        }

        public bool CanExecute(
            object parameter)
        {
            return true;
        }

        public void Execute(
            object parameter)
        {
            NavigationStore.CurrentViewModel = GetViewModel();
        }
    }
}
