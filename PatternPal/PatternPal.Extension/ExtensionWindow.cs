using System.Runtime.InteropServices;
using PatternPal.Extension.Resources;
using PatternPal.Extension.Stores;
using PatternPal.Extension.ViewModels;
using Microsoft.VisualStudio.Shell;

namespace PatternPal.Extension
{
    /// <summary>
    ///     This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    ///     In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    ///     usually implemented by the package implementer.
    ///     <para>
    ///         This class derives from the ToolWindowPane class provided from the MPF in order to use its
    ///         implementation of the IVsUIElementPane interface.
    ///     </para>
    /// </remarks>
    [Guid("fcbb47e2-1474-417e-84ea-9cc1c6ee0617")]
    public class ExtensionWindow : ToolWindowPane
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtensionWindow" /> class.
        /// </summary>
        public ExtensionWindow() : base(null)
        {
            Caption = ExtensionUIResources.ExtensionName;
            var navigationStore = new NavigationStore();
            navigationStore.CurrentViewModel = new HomeViewModel(navigationStore);

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            Content = new ExtensionWindowControl {DataContext = new MainViewModel(navigationStore)};
        }
    }
}
