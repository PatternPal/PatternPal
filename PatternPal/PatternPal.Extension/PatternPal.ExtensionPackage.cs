using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using EnvDTE;
using System.ComponentModel;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

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
    [ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideOptionPage(typeof(PatternPalOptionPageGrid),
    "PatternPal", "Privacy", 0, 0, true)]
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
                PatternPalOptionPageGrid page = (PatternPalOptionPageGrid)GetDialogPage(typeof(PatternPalOptionPageGrid));
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
        protected override async Task InitializeAsync(CancellationToken cancellationToken,
            IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            var dte = (DTE)GetService(typeof(DTE));
            SubscribeBuildEvents.Initialize(dte, this);
        }

        #endregion
    }
    public class PatternPalOptionPageGrid : DialogPage
    {
        private bool _doLogData = false;

        [Category("Privacy")]
        [DisplayName("Log data")] 
        [Description("Whether PatternPal can log your data. The data which gets logged are your actions and your source code. This is used for research. This option is turned off by default.")]
        public bool DoLogData
        {
            get { return _doLogData; }
            set { _doLogData = value; }
        }
    }

    public enum Mode
    {
        Default,
        StepByStep
    }
}
