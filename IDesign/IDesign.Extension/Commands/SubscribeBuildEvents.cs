using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDesign.Extension.Commands;

namespace IDesign.Extension
{
    public static class SubscribeBuildEvents
    {
        public static void Initialize(EnvDTE.DTE dte)
        {
            Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
            dte.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;
        }

        private static async void BuildEvents_OnBuildDone(EnvDTE.vsBuildScope Scope, EnvDTE.vsBuildAction Action)
        {
            try { await LoggingApiClient.PostActionAsync(Action); }
            catch (Exception ex) { }
        }
    }
}
