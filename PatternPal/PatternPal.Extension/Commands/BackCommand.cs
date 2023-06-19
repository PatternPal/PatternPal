#region

using System;
using System.Windows.Input;

using PatternPal.Extension.Stores;

#endregion

namespace PatternPal.Extension.Commands
{
    /// <summary>
    /// BackCommand is an ICommand that navigates back in the NavigationStore. This can easily be used in views and can be seen in the ExtensionWindowControl.xaml file.
    /// </summary>
    public class BackCommand : ICommand
    {
        private NavigationStore NavigationStore { get; }

        // This warning is disabled because the event is required by the ICommand interface
#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067
        /// <summary>
        /// Constructor for BackCommand
        /// </summary>
        /// <param name="navigationStore">NavigationStore that keeps the history to allow going back</param>
        public BackCommand(
            NavigationStore navigationStore)
        {
            NavigationStore = navigationStore;
        }
        /// <summary>
        /// Returns true because the BackCommand can always be executed and is required by the ICommand interface
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(
            object parameter)
        {
            return true;
        }
        /// <summary>
        /// Executes the BackCommand by calling the Back method on the NavigationStore
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(
            object parameter)
        {
            NavigationStore.Back();
        }
    }
}
