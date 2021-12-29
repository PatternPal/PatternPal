using System;
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

        private void HomeView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ButtonsPanel.Orientation = e.NewSize.Width >= ButtonsPanel.Children.Count*190 ? Orientation.Horizontal : Orientation.Vertical;
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
    }
}
