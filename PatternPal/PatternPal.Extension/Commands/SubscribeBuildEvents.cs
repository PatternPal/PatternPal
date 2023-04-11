#region

using System;

using EnvDTE;

using Microsoft.VisualStudio.Shell;

#endregion

namespace PatternPal.Extension.Commands
{
    public static class SubscribeBuildEvents
    {
        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private static PatternPalExtensionPackage _package;

        public static void Initialize(
            DTE dte,
            PatternPalExtensionPackage package)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            dte.Events.BuildEvents.OnBuildDone += BuildEvents_OnBuildDone;
            _package = package;
        }

        private static void BuildEvents_OnBuildDone(
            vsBuildScope Scope,
            vsBuildAction Action)
        {
            ThreadHelper.JoinableTaskFactory.Run(
                async () =>
                {
                    if (_package.DoLogData)
                    {
                        try { await LoggingApiClient.PostActionAsync(Action); }
                        catch (Exception) { }
                    }
                });
        }
    }
}
