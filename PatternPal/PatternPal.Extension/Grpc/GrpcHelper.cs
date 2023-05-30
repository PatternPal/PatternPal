#region

using System.Net.Http;

using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

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

        internal static void ShowErrorMessage(
            string errorMessage)
        {
            const string LOG_SOURCE = "PatternPal";

            ThreadHelper.ThrowIfNotOnUIThread();

            IVsInfoBarUIFactory infoBarFactory = (IVsInfoBarUIFactory)Package.GetGlobalService(typeof( SVsInfoBarUIFactory ));
            IVsUIShell shell = (IVsUIShell)Package.GetGlobalService(typeof( SVsUIShell ));

            IVsInfoBar infoBar = new InfoBarModel(
                new[ ]
                {
                    new InfoBarTextSpan(errorMessage)
                },
                KnownMonikers.StatusError);

            if (ErrorHandler.Failed(
                    shell.FindToolWindow(
                        (uint)__VSFINDTOOLWIN.FTW_fFrameOnly,
                        typeof( ExtensionWindow ).GUID,
                        out IVsWindowFrame frame))
                || frame is null)
            {
                VsShellUtilities.LogError(
                    LOG_SOURCE,
                    $"Failed to find tool window to attach error: {errorMessage}");
                return;
            }

            if (ErrorHandler.Failed(
                frame.GetProperty(
                    (int)__VSFPROPID7.VSFPROPID_InfoBarHost,
                    out object infoBarHost)))
            {
                VsShellUtilities.LogError(
                    LOG_SOURCE,
                    $"Failed to find info bar host to attach error: {errorMessage}");
                return;
            }

            ((IVsInfoBarHost)infoBarHost).AddInfoBar(infoBarFactory.CreateInfoBar(infoBar));
        }
    }
}
