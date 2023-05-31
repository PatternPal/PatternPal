using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using PatternPal.Extension.Resources;
using PatternPal.Extension.Stores;
using PatternPal.Extension.ViewModels;
using Microsoft.VisualStudio.Shell;
using PatternPal.Extension.Grpc;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Imaging;

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
    public class ExtensionWindow : BaseToolWindow<ExtensionWindow>
    {
        public override string GetTitle(int toolWindowId) => ExtensionUIResources.ExtensionName;

        public override Type PaneType => typeof(Pane);

        public override async Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            await Task.Delay(10);
            NavigationStore navigationStore = new NavigationStore();
            navigationStore.CurrentViewModel = new HomeViewModel(navigationStore);

            return new ExtensionWindowControl { DataContext = new MainViewModel(navigationStore)};
        }

        public override void SetPane(ToolWindowPane pane, int toolWindowId)
        {
            base.SetPane(pane, toolWindowId);
        }

        [Guid("99574a6e-e671-4dc3-83e6-755a30839b32")]
        internal class Pane : ToolWindowPane
        {
            /// <summary>
            ///     Initializes a new instance of the <see cref="ExtensionWindow" /> class.
            /// </summary>
            public Pane()
            {
                BitmapImageMoniker = KnownMonikers.AnalyzeTrace;
            }

            protected override void OnClose()
            {
                GrpcBackgroundServiceHelper.KillBackgroundService();
                base.OnClose();
            }
        }
}
}
