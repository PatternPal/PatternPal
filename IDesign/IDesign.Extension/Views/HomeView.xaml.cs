using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace IDesign.Extension.Views
{
    /// <summary>
    ///     Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
        }

        public void OnSettingsClickEvent(object sender, RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Guid guid = typeof(IDesignExtensionPackage).GUID;
            var vsShell = (IVsShell)ServiceProvider.GlobalProvider.GetService(typeof(IVsShell));
            if (vsShell.IsPackageLoaded(ref guid, out var myPackage)
               == Microsoft.VisualStudio.VSConstants.S_OK)
            {
                ((IDesignExtensionPackage)myPackage).ShowOptionPage(typeof(IDesignOptionPageGrid));
            }
        }

        private void StepByStep_OnClick(object sender, RoutedEventArgs e)
        {
            IDesignExtensionPackage.CurrentMode = Mode.StepByStep;
        }
    }
}
