﻿#region

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using EnvDTE;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

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
        "#110",
        "#112",
        "1.0",
        IconResourceID = 400)] // Info on this package for Help/About
    [ProvideMenuResource(
        "Menus.ctmenu",
        1)]
    [ProvideToolWindow(typeof( ExtensionWindow ))]
    [Guid(PackageGuidString)]
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class ExtensionWindowPackage : AsyncPackage
    {
        /// <summary>
        ///     ExtensionWindowPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "ca375a3b-ff54-4156-937d-cbddc605b23c";

        internal static ExtensionWindowPackage PackageInstance { get; private set; }

        #region Package Members

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
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await ExtensionWindowCommand.InitializeAsync(this);

            PackageInstance = this;
            GrpcBackgroundServiceHelper.StartBackgroundService();
        }

        internal static void Main()
        {
        }

        public override IVsAsyncToolWindowFactory GetAsyncToolWindowFactory(
            Guid toolWindowType)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (toolWindowType == typeof( ExtensionWindow ).GUID)
            {
                return this;
            }

            return base.GetAsyncToolWindowFactory(toolWindowType);
        }

        protected override string GetToolWindowTitle(
            Type toolWindowType,
            int id)
        {
            if (toolWindowType == typeof( ExtensionWindow ))
            {
                return "ExtensionWindow loading";
            }

            return base.GetToolWindowTitle(
                toolWindowType,
                id);
        }

        #endregion
    }
}
