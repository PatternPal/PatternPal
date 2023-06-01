#region

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using Community.VisualStudio.Toolkit;

using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

using PatternPal.Extension.Commands;
using PatternPal.Extension.Grpc;

#endregion

namespace PatternPal.Extension
{
    /// <summary>
    ///     This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The minimum requirement for a class to be considered a valid package for Visual Studio
    ///         is to implement the IVsPackage interface and register itself with the shell.
    ///         This package uses the helper classes defined inside the Managed Package Framework (MPF)
    ///         to do it: it derives from the Package class that provides the implementation of the
    ///         IVsPackage interface and uses the registration attributes defined in the framework to
    ///         register itself and its components with the shell. These attributes tell the pkgdef creation
    ///         utility what data to put into .pkgdef file.
    ///     </para>
    ///     <para>
    ///         To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...
    ///         &gt; in .vsixmanifest file.
    ///     </para>
    /// </remarks>
    [ProvideAutoLoad(
        VSConstants.UICONTEXT.SolutionOpening_string,
        PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(
        UseManagedResourcesOnly = true,
        AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(
        Vsix.Name,
        Vsix.Description,
        Vsix.Id,
        IconResourceID = 400)] // Info on this package for Help/About
    [ProvideToolWindow(
        typeof( ExtensionWindow.Pane ),
        Style = VsDockStyle.Float)]
    [ProvideMenuResource(
        "Menus.ctmenu",
        1)]
    [Guid(PackageGuids.ExtensionViewString)]
    [ProvideOptionPage(
        typeof( OptionsProvider.PrivacyOptions ),
        "PatternPal",
        "Privacy",
        0,
        0,
        true)]
    [ProvideProfile(
        typeof( OptionsProvider.PrivacyOptions ),
        "PatternPal",
        "Privacy",
        0,
        0,
        true)]
    public sealed class ExtensionWindowPackage : ToolkitPackage
    {
        /// <summary>
        ///     ExtensionWindowPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "ca375a3b-ff54-4156-937d-cbddc605b23c";

        internal static ExtensionWindowPackage PackageInstance { get; private set; }

        public static Mode CurrentMode = Mode.Default;

        /// <summary>
        ///     Initialization of the package; this method is called right after the package is sited, so this is the place
        ///     where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A cancellation token to monitor for initialization cancellation, which can occur when
        ///     VS is shutting down.
        /// </param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>
        ///     A task representing the async work of package initialization, or an already completed task if there is none.
        ///     Do not return null from this method.
        /// </returns>
        protected override async Task InitializeAsync(
            CancellationToken cancellationToken,
            IProgress< ServiceProgressData > progress)
        {
            this.RegisterToolWindows();
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await this.RegisterCommandsAsync();

            PackageInstance = this;
            GrpcBackgroundServiceHelper.StartBackgroundService();

            DTE dte = (DTE)await GetServiceAsync(typeof( DTE ));
            SubscribeEvents.Initialize(
                dte,
                this,
                cancellationToken);
        }

        internal static void Main()
        {
        }

        /// <summary>
        /// Overrides a package method executed when the extension is closed.
        /// </summary>
        /// <param name="canClose"></param>
        /// <returns></returns>
        protected override int QueryClose(
            out bool canClose)
        {
            if (Privacy.Instance.DoLogData)
            {
                SubscribeEvents.OnSessionEnd();
            }

            canClose = true;
            return VSConstants.S_OK;
        }
    }

    public enum Mode
    {
        Default,
        StepByStep
    }
}
