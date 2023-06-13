#region

using System;
using System.Windows.Input;

using PatternPal.Extension.Stores;

#endregion

namespace PatternPal.Extension.Commands
{
    // TODO Comment
    public class BackCommand : ICommand
    {
        private NavigationStore NavigationStore { get; }

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public BackCommand(
            NavigationStore navigationStore)
        {
            NavigationStore = navigationStore;
        }

        public bool CanExecute(
            object parameter)
        {
            return true;
        }

        public void Execute(
            object parameter)
        {
            NavigationStore.Back();
        }
    }
}
