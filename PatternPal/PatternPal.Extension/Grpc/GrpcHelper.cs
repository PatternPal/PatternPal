#region

using System.Diagnostics;
using System.Net.Http;

using Community.VisualStudio.Toolkit;

using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

using PatternPal.Protos;

#endregion

namespace PatternPal.Extension.Grpc
{
    internal static class GrpcHelper
    {
        internal static GrpcChannel Channel { get; }
        internal static RecognizerService.RecognizerServiceClient RecognizerClient { get; }
        internal static StepByStepService.StepByStepServiceClient StepByStepClient { get; }

        static GrpcHelper()
        {
            Channel = GrpcChannel.ForAddress(
                "http://localhost:5000",
                new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(new HttpClientHandler()),
                });

            RecognizerClient = new RecognizerService.RecognizerServiceClient(Channel);
            StepByStepClient = new StepByStepService.StepByStepServiceClient(Channel);
        }

        /// <summary>
        /// Creates a notification in the PatternPal window notifying the user that an error has
        /// occurred. If the background process has crashed, the user gets the option to restart it.
        /// </summary>
        internal static void ShowErrorMessage(
            string errorMessage)
        {
            const string LOG_SOURCE = "PatternPal";
            const string RESTART = "Restart";

            ThreadHelper.ThrowIfNotOnUIThread();

            // Create the infobar model.
            InfoBarModel infoBarModel = Process.GetProcessesByName("PatternPal").Length > 0
                ? new InfoBarModel(
                    new[ ]
                    {
                        new InfoBarTextSpan(errorMessage)
                    },
                    KnownMonikers.StatusError)
                : new InfoBarModel(
                    new[ ]
                    {
                        new InfoBarTextSpan("PatternPal has crashed")
                    },
                    new[ ]
                    {
                        new InfoBarHyperlink(RESTART)
                    },
                    KnownMonikers.StatusError);

            ThreadHelper.JoinableTaskFactory.Run(
                async () =>
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    InfoBar infoBar = await VS.InfoBar.CreateAsync(
                        typeof( ExtensionWindow.Pane ).GUID.ToString(),
                        infoBarModel);

                    // Log the error if we can't create the infobar to show it in.
                    if (infoBar == null)
                    {
                        VsShellUtilities.LogError(
                            LOG_SOURCE,
                            $"Failed to find tool window to attach error: {errorMessage}");
                        return;
                    }

                    // Restart the background service if necessary.
                    infoBar.ActionItemClicked += (
                                                     sender,
                                                     e) =>
                                                 {
                                                     ThreadHelper.ThrowIfNotOnUIThread();
                                                     if (e.ActionItem.Text == RESTART)
                                                     {
                                                         GrpcBackgroundServiceHelper.StartBackgroundService();
                                                     }
                                                     infoBar.Close();
                                                 };

                    await infoBar.TryShowInfoBarUIAsync();
                });
        }
    }
}
