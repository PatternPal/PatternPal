#region

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using PatternPal.Extension.Stores;
using PatternPal.Extension.ViewModels;
using Process = System.Diagnostics.Process;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

#endregion

namespace PatternPal.Extension.Commands
{
    public class NavigateCommand<T> : ICommand
        where T : ViewModel
    {
        private NavigationStore NavigationStore { get; }
        private Func<T> GetViewModel { get; }

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067

        public NavigateCommand(
            NavigationStore navigationStore,
            Func<T> getViewModel)
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
            T viewModel = GetViewModel();
            NavigationStore.CurrentViewModel = viewModel;
        }
    }
}
