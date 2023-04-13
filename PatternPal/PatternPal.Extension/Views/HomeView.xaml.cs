#region

using System;
using System.Windows;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

#endregion

namespace PatternPal.Extension.Views
{
    /// <summary>
    ///     Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView
    {
        public HomeView()
        {
            InitializeComponent();
        }

        private void OnSettingsClickEvent(
            object sender,
            RoutedEventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Guid guid = typeof( PatternPalExtensionPackage ).GUID;
            IVsShell vsShell = (IVsShell)ServiceProvider.GlobalProvider.GetService(typeof( IVsShell ));
            if (null != vsShell
                && vsShell.IsPackageLoaded(
                    ref guid,
                    out IVsPackage myPackage)
                == VSConstants.S_OK)
            {
                ((PatternPalExtensionPackage)myPackage).ShowOptionPage(typeof( PatternPalOptionPageGrid ));
            }
        }

        private void StepByStep_OnClick(
            object sender,
            RoutedEventArgs e)
        {
            PatternPalExtensionPackage.CurrentMode = Mode.StepByStep;
        }
    }
}
