using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDesign.Extension.Commands;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace IDesign.Extension
{
    public static class SubscribeBuildEvents
    {
        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private static IDesignExtensionPackage package;
        public static void Initialize(EnvDTE.DTE dte, IDesignExtensionPackage package)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            dte.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;
            SubscribeBuildEvents.package = package;
        }

        private static async void BuildEvents_OnBuildDone(EnvDTE.vsBuildScope Scope, EnvDTE.vsBuildAction Action)
        {
            if (package.DoLogData)
            {
                try { await LoggingApiClient.PostActionAsync(Action); }
                catch (Exception ex) { }
            }
        }
    }
}
