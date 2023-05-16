using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using PatternPal.Extension.Commands;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.RpcContracts.Commands;
using System.Management.Instrumentation;
using System.Reflection.Metadata;
using System.Runtime.Remoting.Contexts;
using static Microsoft.VisualStudio.Shell.ThreadedWaitDialogHelper;

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
        UIContextGuids80.SolutionExists,
        PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(
        UseManagedResourcesOnly = true,
        AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideOptionPage(
        typeof(PatternPalOptionPageGrid),
        "PatternPal",
        "Privacy",
        0,
        0,
        true)]
    public sealed class PatternPalExtensionPackage : AsyncPackage
    {
        /// <summary>
        ///     PatternPal.ExtensionPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "99085877-4676-4a93-8c40-630784cf71f6";

        /// <summary>
        ///     The current mode the user is using (currently either "Default" or "Step by Step")
        /// </summary>
        public static Mode CurrentMode = Mode.Default;

        #region Package Members

        public bool DoLogData
        {
            get
            {
                PatternPalOptionPageGrid page =
                    (PatternPalOptionPageGrid)GetDialogPage(typeof(PatternPalOptionPageGrid));
                return page.DoLogData;
            }
        }

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
            IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            DTE dte = (DTE)await GetServiceAsync(typeof(DTE));
            SubscribeEvents.Initialize(
                dte,
                this, cancellationToken);
        }

        /// <summary>
        /// Overrides a package method executed when the extension is closed.
        /// </summary>
        /// <param name="canClose"></param>
        /// <returns></returns>
        protected override int QueryClose(out bool canClose)
        {
            if (DoLogData)
            {
                SubscribeEvents.OnSessionEnd();
            }

            canClose = true;
            return VSConstants.S_OK;
        }

        #endregion
    }

    public class PatternPalOptionPageGrid : DialogPage
    {
        private bool _doLogData;

        [Category("Privacy")]
        [DisplayName("Log data")]
        [Description(
            "Whether PatternPal can log your data. The data which gets logged are your actions and your source code. This is used for research. This option is turned off by default.")]
        public bool DoLogData
        {
            get { return _doLogData; }
            set
            {
                // Set is triggered both when changing the field value, as well as when clicking on the OK button.
                // This prevents the code from being triggered twice.
                if (value == _doLogData) return;

                ThreadHelper.JoinableTaskFactory.Run(async () =>
                {
                    await (value
                        ? SubscribeEvents.SubscribeEventHandlersAsync()
                        : SubscribeEvents.UnsubscribeEventHandlersAsync());
                });

                // The SessionId has to be reset if the option for logging data is changed to prevent logging without a session id. 
                // In other words, a new session has to be started.
                if (value)
                {
                    SubscribeEvents.OnSessionStart();
                }
                else
                {
                    SubscribeEvents.OnSessionEnd();
                }

                _doLogData = value;
            }
        }
    }


    public enum Mode
    {
        Default,
        StepByStep
    }
}
